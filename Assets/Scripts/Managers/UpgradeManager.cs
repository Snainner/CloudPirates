using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;



public class UpgradeManager : MonoBehaviour
{
    public int Level;
    
    AdvancedPlaneController _plane;
    GunItem _gun;

    //UpgradeData[] _upgrades => new UpgradeData[3];
    List<(UpgradeData, Component)> _upgradePool;
    Upgradable[] _upgradables;
    List<(UpgradeData, Component)> _currentPicks = new List<(UpgradeData, Component)> ();

    int _selectionCount = UpgradeConstants.UpgradeSelectionCount;

    private void Awake()
    {

        _upgradables = GetComponents<Upgradable>();

    }
    private void OnEnable()
    {
        EventManager.AddListener<LevelUpEvent>(OnLevelUpEvent);
        EventManager.AddListener<ApplyUpgradeEvent>(OnApplyUpgradeEvent);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<LevelUpEvent>(OnLevelUpEvent);
        EventManager.RemoveListener<ApplyUpgradeEvent>(OnApplyUpgradeEvent);
    }
    private void Start()
    {
        if (_upgradables == null)
            Debug.Log("No upgradable components found");
        else
            _upgradePool = UpgradeFactory.CreateUpgradePool(_upgradables);
    }
    [Button]
    void TestLevelUpEvent()
    {
        EventManager.Broadcast(Events.LevelUpEvent);
    }

    void OnLevelUpEvent(LevelUpEvent e)
    {
        if (UIManager.Instance.IsUpgradeUIActive)
            return;

        _currentPicks.Clear();
        _currentPicks = UpgradeFactory.RollUpgrades(_upgradePool,_selectionCount );

        //Send data to UI 
        //Trigger UI'
        if(_currentPicks.Count < 2 )
        {
            Debug.Log("UpgradeManager: Current pick pool is less then 2");
            return;
        }
        string[] previews = new string[_selectionCount];
        previews[0] = _currentPicks[0].Item1.Preview; previews[1] = _currentPicks[1].Item1.Preview; previews[2] = _currentPicks[2].Item1.Preview;


        EventManager.Broadcast(new UpgradeUIEvent { UpgradePreviews = previews });
    }

    void OnApplyUpgradeEvent(ApplyUpgradeEvent e)
    {
        
        ApplyChosenUpgrade(e.ChosenUpgrade);
    }
    void ApplyChosenUpgrade(int index)
    {
        var chosen = _currentPicks[index];
        Debug.Log($"Applying {chosen.Item1.Preview}");
        chosen.Item1.Apply(chosen.Item2);
    }
}
