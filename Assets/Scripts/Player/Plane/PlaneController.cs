using UnityEngine;

public class PlaneController : MonoBehaviour
{
    PlayerController _controller;
    Transform _tr;
    Rigidbody _rb;

    [Header("Camera")]
    Camera _cam;
    [SerializeField]
    Transform _camTarget;
    [SerializeField]
    [Range(0f, 1f)]
    float _camSpring = .96f;

    [Header("Movement")]
    [SerializeField]
    float _minThrust = 600f;
    [SerializeField]
    float _maxThrust = 1200f;
    float _currentThrust;
    [SerializeField]
    float _thrustIncreaseSpeed = 400f;

    float _deltaPitch;
    [SerializeField]
    float _pitchIncreaseSpeed = 300f;
    float _deltaRoll;
    [SerializeField]
    float _rollIncreaseSpeed = 300f;

    private void Start()
    {
        _tr = transform;
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        _cam.transform.SetParent(null);
        _controller = GetComponent<PlayerController>();
        _camTarget = GetComponent<Transform>();
    }

    private void Update()
    {
        ControlPlane(_controller.MoveDir, _controller.Up, _controller.Down);
    }
    private void FixedUpdate()
    {
        ConstantForces();
        PlaneCamera();
    }
    void ControlPlane(Vector2 moveDir, bool up, bool down)
    {
        float thrustDelta = 0;
        if(up)
            thrustDelta += _thrustIncreaseSpeed;
        if(down)
            thrustDelta -= _thrustIncreaseSpeed;

        _currentThrust += thrustDelta * Time.deltaTime;
        _currentThrust = Mathf.Clamp( _currentThrust, _minThrust, _maxThrust );

        _deltaPitch = 0f;
        _deltaPitch += _pitchIncreaseSpeed * moveDir.y;
        _deltaPitch *= Time.deltaTime;

        _deltaRoll = 0f;
        _deltaRoll += _rollIncreaseSpeed * moveDir.x;
        _deltaRoll *= Time.deltaTime;

    }
    void ConstantForces()
    {
        var localRotation = _tr.localRotation;
        localRotation *= Quaternion.Euler(0f, 0f, _deltaRoll);
        localRotation *= Quaternion.Euler(_deltaPitch, 0f,0f);  
        _tr.localRotation = localRotation;
        _rb.linearVelocity = _tr.forward * (_currentThrust * Time.fixedDeltaTime);
    }
    void PlaneCamera()
    {
        Vector3 camTargetPos = _tr.position + _tr.forward * -8f + new Vector3(0f, 3f, 0f);
        var camTransform = _cam.transform;
        camTransform.position = camTransform.position * _camSpring + camTargetPos * (1 - _camSpring);
        _cam.transform.LookAt( _camTarget);
    }
}
