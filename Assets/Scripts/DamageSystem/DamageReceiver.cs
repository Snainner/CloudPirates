using NaughtyAttributes;
using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamagable
{
    Health _health;
    public int DebugDamage = 50;

    public event System.Action<DamageData> OnDamage;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    [Button]
    void GiveDamageDebug()
    {
        TakeDamage(new DamageData(DebugDamage, this));
    }
    public void TakeDamage(DamageData damage)
    {
        if (_health == null)
        {
            Debug.Log("DamageReceiver: No Health object found");
            return;
        }
        if(_health.IsDead) 
            return;
        OnDamage?.Invoke(damage);
        _health.ApplyDamage(damage.Amount);
    }
}
