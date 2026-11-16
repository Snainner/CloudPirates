using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField]
    UpgradeUI _upgradeUI;
    [SerializeField]
    public GameObject CreditCanvas;
    public bool IsUpgradeUIActive { get {return _upgradeUI.IsActive; } }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

        }
        if (Instance != this)
        {
            Destroy(Instance);
            return;
        }
    }

}
