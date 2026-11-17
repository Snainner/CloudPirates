using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    PlayerSettings _settings;
    public PlayerSettings Settings { get { return _settings; }}

    GameObject _planeGo;
    GameObject _gunGo;
    GameObject _camGo;
    GameObject _playerCompGo; //General player specific components such as damage, upgrades, etc. 

    AudioSource _audioSource;
    public Health Health { get; private set; } 
    GunItem _gun;
    FollowCam _followCam;
    CaptureClouds _captureClouds;
    
    //Rigidbody _rb;
    public AdvancedPlaneController Plane { get; private set; }
    PlaneInput _planeInput;
    

    private void Awake()
    {
        Plane = GetComponentInChildren<AdvancedPlaneController>();
        _followCam = GetComponentInChildren<FollowCam>();
        _gun = GetComponentInChildren<GunItem>();
        Health = GetComponentInChildren<Health>();
        _audioSource = GetComponent<AudioSource>();

        _planeGo = Plane.gameObject;
        _planeInput = Plane.Input;
        //_rb = _planeGo.GetComponent<Rigidbody>();
        
        _gunGo = _gun.gameObject;
        _camGo = _followCam.gameObject;
        _playerCompGo = Health.gameObject;

        _captureClouds = _playerCompGo.GetComponent<CaptureClouds>();

        _captureClouds.ApplySettings(Settings.CloudCaptureRadius);
        Plane.ApplySettings(_settings.PlaneSettings);
        _followCam.ApplySettings(_settings.CameraSettings);
    }
    private void OnEnable()
    {
        EventManager.AddListener<LevelUpEvent>(OnLevelUp);
        EventManager.AddListener<ApplyUpgradeEvent>(OnApplyUpgrade);
        Health.OnDeath += OnDeath;

    }
    private void OnDisable()
    {
        EventManager.RemoveListener<LevelUpEvent>(OnLevelUp);
        EventManager.RemoveListener<ApplyUpgradeEvent>(OnApplyUpgrade);
        Health.OnDeath -= OnDeath;
 

    }
    private void Update()
    {
        //Remove gun input from the plane controller
        Plane.HandleInput();
        FireGun();
        _captureClouds.Capture(Plane.transform.position);
    }
    private void FixedUpdate()
    {
        Plane.ApplyInput();
        _followCam.Follow(_planeGo.transform, Plane.CurrentThrottle);

        //Should handle this with a signal 
        Plane.HandleAudio();
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
    void OnDeath()
    {
        UIManager.Instance.CreditCanvas.SetActive(true);
    }
    void FireGun()
    {
        _gun.AutomaticFire(_planeInput.IsShooting);
    }

}
