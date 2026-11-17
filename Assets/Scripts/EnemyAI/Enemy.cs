
using System;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    GunItem _gun;
    Health _health;
    [SerializeField]
    EngineAudio _engineAudio;
    AudioSource _audioSource;
    
    AdvancedPlaneController _player;
    BoxCollider _boxCollider;
    LayerMask _playerMask;
    DamageData _damageData;
    ShootingSight _shootCollider;
    

    [Header("Enemy")]
    [SerializeField]
    int _damage = 10;
    [SerializeField]
    Vector3 _gunOffset;
    [SerializeField]
    int _rayDist = 1000;
    [SerializeField]
    float _minEngageDist = 250f;
    [Header("Movement")]
    [SerializeField]
    float _speed;
    [SerializeField]
    float _minSpeed = 25f;
    [SerializeField]
    float _maxSpeed = 70f;
    [SerializeField]
    float _accel = 10f;
    [SerializeField]
    float _catchUpBonus = .3f; //extra speed per 100m gap
    [SerializeField]
    float _catchUpGap = 100f;
    [SerializeField]
    float _minAltitude = 0f;
    [Header("Turning")]
    [SerializeField]
    float _turnRate;
    [SerializeField]
    float _maxBank;
    [SerializeField]
    float _bankSmooth;

    [SerializeField]
    float _maxPitch = 45f;
    [SerializeField]
    float _leadTimeMax = 1f;

    float _currentBank;

    float _yaw;
    float _pitch;

    bool _isAttacking = false;
    float _currentSpeed;
    static float NormalizeAngle(float angle) => angle > 180f ? angle - 360f : angle;


    [SerializeField]
    ParticleSystem _explosion;
    //[SerializeField]
    //float _attackRadius = 15f;
    //[SerializeField]
    //float _predictionModifier;
    private void Awake()
    {
        _shootCollider = GetComponentInChildren<ShootingSight>();
        _gun = GetComponentInChildren<GunItem>();
        _health = GetComponent<Health>();
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        _yaw = euler.y;
        _pitch = NormalizeAngle(euler.x);


        _player = GameManager.Instance.Player.Plane;
        _playerMask = LayerMask.GetMask("Player");
        _damageData = new DamageData(10, this);
    }
    private void FixedUpdate()
    {
        _gun.AutomaticFire(_shootCollider.IsAttacking);
        _currentSpeed = Speed(_player.transform.position);
        BetterFlyTo(_player.transform.position, _currentSpeed);
        _engineAudio.UpdateEngineSound(_audioSource, _currentSpeed, _maxSpeed, _minSpeed);
    }

    private void OnEnable()
    {
        _shootCollider.OnTrigger += OnChildTrigger;
        _health.OnDeath += OnDeath;
        _engineAudio.StartEngineSound(_audioSource);
    }
    private void OnDisable()
    {
        _shootCollider.OnTrigger -= OnChildTrigger;
        _health.OnDeath -= OnDeath;
        _engineAudio.StopEngineSound(_audioSource);
    }

    void BetterFlyTo(Vector3 target, float targetSpeed)
    {
        //Vector3 targetDelta = target - transform.position;
        //bool targetBehind = Vector3.Dot(transform.forward, targetDelta.normalized) < 0f;
        //bool tooClose = targetDelta.magnitude < _minEngageDist;
        //if (tooClose && !targetBehind)
        //{

        //}

        //Lead the target instead of cutting it off for tail chases
        float dist = Vector3.Distance(transform.position, target);
        float leadTime = Mathf.Clamp(dist / Mathf.Max(_speed, 1f), 0f, _leadTimeMax);
        Vector3 aimPoint = target + _player.Rb.linearVelocity * leadTime;

        Vector3 dir = (aimPoint - transform.position).normalized;

        //Calculate desired yaw/pitch from direction 
        float desiredYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float desiredPitch = -Mathf.Asin(Mathf.Clamp(dir.y, -1f, 1f)) * Mathf.Rad2Deg;
        
        //Prevent pitching down when too low
        if(transform.position.y < _minAltitude)
            desiredPitch = Mathf.Min(desiredPitch, 0f);
        else
            desiredPitch = Mathf.Clamp(desiredPitch, -_maxPitch, _maxPitch);

        //Turn toward target at a limited rate
        float step = _turnRate * Time.deltaTime;
        float yawBefore = _yaw;
        _yaw = Mathf.MoveTowardsAngle(_yaw, desiredYaw, step);
        _pitch = Mathf.MoveTowards(_pitch, desiredPitch, step * 0.7f); //Slow down the pitch slightly for airplane feeling
        
        // Bank based on how hard we are yawing
        float yawRate = Mathf.DeltaAngle(yawBefore, _yaw) / step;       // -1..1
        float targetBank = Mathf.Clamp(-yawRate, -1f, 1f) * _maxBank;
        _currentBank = Mathf.Lerp(_currentBank, targetBank, _bankSmooth * Time.deltaTime);

        // Build rotation once from our own values
        transform.rotation = Quaternion.Euler(_pitch, _yaw, _currentBank);

        transform.position += transform.forward * targetSpeed * Time.deltaTime;
    }
    void FlyTo(Vector3 target)
    {
        //Rotate towards target
        Vector3 dir = (target - transform.position).normalized;
        Quaternion desiredRot = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion currentRot = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(currentRot, desiredRot, _turnRate * Time.deltaTime);

        //Bank towards target based on how hard we are turning sideways 
        float yawnDelta = Mathf.DeltaAngle(currentRot.eulerAngles.y, transform.eulerAngles.y);
        float targetBank = Mathf.Clamp(-yawnDelta / (_turnRate * Time.deltaTime), -1f, 1f) * _maxBank ;
        _currentBank = Mathf.Lerp(_currentBank, targetBank, _bankSmooth * Time.deltaTime);

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(euler.x, euler.y, _currentBank);

        //Add default forward momentum 
        transform.position += transform.forward * Speed(target)* Time.deltaTime;

    }

    float Speed(Vector3 targetPos)
    {
        float playerSpeed = _player.CurrentVelocity;
        float dist = Vector3.Distance(transform.position, targetPos);
        float desired = playerSpeed + (dist / _catchUpGap) * _catchUpBonus * playerSpeed;

        desired = Mathf.Clamp(desired, _minSpeed, _maxSpeed);
        _speed = Mathf.MoveTowards(_speed, desired, _accel * Time.deltaTime);

        return _speed;
    }


    void OnChildTrigger()
    {
        _gun.AutomaticFire(_shootCollider.IsAttacking);
    }

    
    void OnDeath()
    {
        var fx = Instantiate(_explosion, transform.position, Quaternion.identity);
        fx.Play();
        Destroy(fx.gameObject, fx.main.duration);
        Destroy(gameObject);
    }

    public void ApplySettings(EnemySettings settings)
    {
        throw new NotImplementedException();
    }
}

#region Depreciated
//void MoveTo(Vector3 location)
//{
//    Vector3 currentLoc = transform.position;
//    if (Vector3.Distance(currentLoc, location) > _attackRadius)
//        transform.position = Vector3.MoveTowards(currentLoc, location, _speed * Time.deltaTime);
//}
//private void OnTriggerEnter(Collider other)
//{
//    if(other.gameObject.layer == _playerMask)
//    {
//        _isAttacking = true;
//        StartCoroutine(StartShooting());    
//    }
//}

//private void OnTriggerExit(Collider other)
//{
//    if (other.gameObject.layer == _playerMask)
//    {
//        _isAttacking = false;
//        StopAllCoroutines();
//    }
//}
#endregion