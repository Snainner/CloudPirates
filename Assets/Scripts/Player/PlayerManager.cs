using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    PlayerSettings _settings;
    public PlayerSettings Settings { get { return _settings; }}
    Rigidbody Rb;
    AdvancedPlaneController _plane;
    FollowCam _followCam;
    GunItem _gun;

    private void Awake()
    {
        Rb = GetComponentInChildren<Rigidbody>();
        _plane = GetComponentInChildren<AdvancedPlaneController>();
        _followCam = GetComponentInChildren<FollowCam>();
        _gun = GetComponentInChildren<GunItem>();

        _plane.ApplySettings(_settings.PlaneSettings);
        _followCam.ApplySettings(_settings.CameraSettings);
    }
    private void OnEnable()
    {
        EventManager.AddListener<LevelUpEvent>(OnLevelUp);
        EventManager.AddListener<ApplyUpgradeEvent>(OnApplyUpgrade);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<LevelUpEvent>(OnLevelUp);
        EventManager.RemoveListener<ApplyUpgradeEvent>(OnApplyUpgrade);
    }
    private void Update()
    {
        //Remove gun input from the plane controller
        _plane.HandleInput();
    }
    private void FixedUpdate()
    {
        _plane.ApplyInput();
        _followCam.Follow(transform.position, transform, _plane.CurrentThrottle);
    }

    void OnLevelUp(LevelUpEvent evt)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }    

    void OnApplyUpgrade(ApplyUpgradeEvent evt)
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;

    }
}
