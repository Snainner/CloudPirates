
using UnityEngine;



public class AdvancedPlaneController : Upgradable
{
    public PlaneSettings Settings;
    Health _health;
    [SerializeField]
    Vector3 _centreOfMass;
    [SerializeField]
    float _throttleIncrement = 0.1f;
    [SerializeField]
    float _baseMaxThrust = 20f;
    [SerializeField]
    float _responsiveness = 10f;
    [SerializeField]
    float _rollModifier = 2f;
    //[SerializeField]
    //float _lift = 135f;

    [NaughtyAttributes.ReadOnly]
    [SerializeField]
    float _throttle;
    float _roll;
    float _pitch;
    float _yaw;

    float _responseModifier
    {
        get { return (Rb.mass / 10f) * _responsiveness; }
    }

    public Rigidbody Rb {get; private set;}
    public float CurrentThrottle { get { return _throttle; } }
    public float CurrentVelocity { get { return Rb.linearVelocity.magnitude; } }
    //public float SpeedPercentage { get { return } }
    PlaneInput _input;
    float _levelModifier = 1;
    public Stat Thrust;
    public Stat Acceleration;
    public float MaxThrust => _baseMaxThrust;

    GunItem _gun;
    EngineAudio _audio;

    private void Awake()
    {
        _health = GetComponent<Health>();
        Rb = GetComponent<Rigidbody>();
        Rb.centerOfMass = _centreOfMass;
        
        _input = GetComponent<PlaneInput>();
        _gun = GetComponent<GunItem>();
        _audio = GetComponent<EngineAudio>();

        if (Settings.MaxThrust != 0)
            ApplySettings(Settings);
        else
            CreateStats(_baseMaxThrust, _throttleIncrement);
    }
    private void OnEnable()
    {
        _health.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        _health.OnDeath -= OnDeath;

    }
    private void Update()
    {
        HandleInput();
        _audio.UpdateEngineSound(CurrentThrottle, MaxThrust, 10f);
    }
    private void FixedUpdate()
    {
        ApplyInput();
        
        //Lift force
        //_rb.AddForce(_rb.transform.up * _rb.linearVelocity.magnitude * _lift);
    }

    void OnDeath()
    {
        UIManager.Instance.CreditCanvas.SetActive(true);
    }
    public void ApplySettings(PlaneSettings settings)
    {
        _baseMaxThrust = settings.MaxThrust;
        _throttleIncrement = settings.ThrottleIncrement;
        _rollModifier = settings.RollModifier;
        _responsiveness = settings.Responsiveness;
    }
    void CreateStats(float baseThrust, float baseThrottleIncrement)
    {
        Thrust = new Stat(baseThrust);
        Acceleration = new Stat(baseThrottleIncrement);
    }
    public void HandleInput()
    {
        _gun.AutomaticFire(_input.IsShooting);

        _roll = _input.Roll; //A D
        _pitch = _input.Pitch;//S W
        _yaw = _input.Yaw;//Q E

        _throttle += (_throttleIncrement * _input.Throttle * Time.deltaTime);
        _throttle = Mathf.Clamp(_throttle, 10, 100);
    }

    public void ApplyInput()
    {
        Rb.AddForce(transform.forward * MaxThrust * _throttle);

        float speedModifier = (_throttle / 100f) * 2f;

        //Add rotational force
        Rb.AddTorque(transform.up * _yaw * (_responseModifier - speedModifier));  //QE
        Rb.AddTorque(-transform.right * _pitch * ((_responseModifier - speedModifier) * _rollModifier)); //SW
        Rb.AddTorque(-transform.forward * _roll * (_responseModifier - speedModifier)); //AD
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.localPosition + _centreOfMass, .2f);

    }

    protected override UpgradeData[] BuildUpgrades()
    {
        var speed = UpgradeFactory.Create<AdvancedPlaneController>("max_thrust", "Max Speed", c => c.Thrust.Bonus);
        var accel = UpgradeFactory.Create<AdvancedPlaneController>("accel", "Acceleration", c => c.Acceleration.Bonus);
        return new UpgradeData[] { speed };
    }
}
