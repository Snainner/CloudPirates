using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : Upgradable
{
    [SerializeField]
    int _baseHP = 100;
    [Header("Regen")]
    [SerializeField]
    bool _hasRegen = false; //Gets enabled if regen per second is above 0
    [SerializeField]
    float _regenPerSecond = 0f; 
    public int CurrentHP { get; private set; }
    public Stat MaxHP { get; private set; }
    int _maxHP => (int)MaxHP.Value;
    public bool IsDead => CurrentHP <= 0;
    
    Stat Regen = new Stat(1f);


    public event System.Action<int, int> OnHealthChange;
    public event System.Action<int> OnDamageTaken;
    public event System.Action OnDeath;

    float _minUpgradePerc => UpgradeConstants.MinUpgradePerc;
    float _maxUpgradePerc => UpgradeConstants.MaxUpgradePerc;
    private void Start()
    {
        SetMaxHP(_baseHP);

    }
    private void Update()
    {
        TickRegen();
    }
    public void SetMaxHP(int value)
    {
        _baseHP = value;
        MaxHP = new Stat(value);
        CurrentHP = value;
        OnHealthChange?.Invoke(CurrentHP, _maxHP);
    }
    public void SetRegen(float perSecond)
    {
        _regenPerSecond = perSecond;
        _hasRegen = perSecond > 1f;
    }
    public void ApplyDamage(int amount)
    {
        if (IsDead)
            return;
        if (amount <= 0)
            return;

        CurrentHP -= amount;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        OnHealthChange?.Invoke(CurrentHP, _maxHP);
        OnDamageTaken?.Invoke(amount);

        if (CurrentHP <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        if(IsDead)
            return;
        if(amount <= 0)
            return;
        CurrentHP += amount;
        CurrentHP = Mathf.Min(CurrentHP, _maxHP);

        OnHealthChange?.Invoke(CurrentHP, _maxHP);
    }
    void TickRegen()
    {
        if (!_hasRegen)
            return;
        if(IsDead)
            return;
        if (CurrentHP >= _maxHP)
            return;

        Heal(Mathf.RoundToInt(_regenPerSecond * Time.deltaTime));
    }
    void Die()
    {
        OnDeath?.Invoke();
    }

    protected override UpgradeData[] BuildUpgrades()
    {
        UpgradeData maxHealth = UpgradeFactory.Create<Health>("max_health", "Max Health", c => c.MaxHP.Bonus);
        UpgradeData regen = UpgradeFactory.Create<Health>("regen", "Regen", c => c.Regen.Bonus);
        return new UpgradeData[] { maxHealth, regen };
    }
}
