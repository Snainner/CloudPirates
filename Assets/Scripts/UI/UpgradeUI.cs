using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Toggle TogglePrefab;
    public GameObject UpgradeMenu;
    Button _applyButton;

    [SerializeField]
    Transform _selectionContainer;
    [SerializeField]
    ToggleGroup _toggleGroup;
    List<(Toggle, Text)> _toggles;
    int _toggleCount = 3;

    public bool IsActive => UpgradeMenu.activeSelf;
    private void Awake()
    {
        _toggleCount = UpgradeConstants.UpgradeSelectionCount;
       _applyButton = GetComponentInChildren<Button>();
        _toggleGroup = GetComponentInChildren<ToggleGroup>();
        if (_toggleGroup == null)
            Debug.Log("No toggle group found");
        
        if(TryCreateToggles(_toggleCount))
            UpgradeMenu.SetActive(false);

    }

    private void OnEnable()
    {
        EventManager.AddListener<UpgradeUIEvent>(OnUpgradeUIEvent);
        _applyButton.onClick.AddListener(OnApplyButton);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<UpgradeUIEvent>(OnUpgradeUIEvent);
        _applyButton.onClick.RemoveAllListeners();
    }

    void OnUpgradeUIEvent(UpgradeUIEvent e)
    {
        for (int i = 0; i < _toggles.Count; i++)
        {
            _toggles[i].Item2.text = e.UpgradePreviews[i];
        }
        UpgradeMenu.SetActive(true);
    }
    bool TryCreateToggles(int toggleCount)
    {
        if(_toggles != null)
        {
            //toggles are already created
            return true;
        }
        _toggles = new List<(Toggle, Text)>();
        for (int i = 0; i < toggleCount; i++)
        {
            var toggle = Instantiate(TogglePrefab, _selectionContainer);
            var text = toggle.GetComponentInChildren<Text>();
            if (text == null)
                Debug.Log("Toggle label not found");
            toggle.group = _toggleGroup;

            _toggles.Add((toggle, text));
        }
        return true;
    }

    void OnApplyButton()
    {
        var active = _toggleGroup.GetFirstActiveToggle();
        int index = _toggles.FindIndex(t => t.Item1 == active);

        EventManager.Broadcast(new ApplyUpgradeEvent { ChosenUpgrade = index });
        UpgradeMenu.SetActive(false);
    }
    void ToggleMenu(bool toggle)
    {
        //Set
    }

}
