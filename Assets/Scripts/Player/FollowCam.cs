using PrimeTween;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [Header("Camera")]
    Camera _cam;
    [SerializeField]
    Transform _camTarget;
    [SerializeField]
    [Range(0f, 1f)]
    float _camSpring = .96f;
    [SerializeField]
    [Range(0f, 15f)]
    float _distance = 8f;
    [SerializeField]
    Vector3 _offset;

    [Header("Camera Roll")]
    [SerializeField]
    bool _rollEnabled = true;
    [SerializeField]
    [Range (0f, 60f)]
    float _maxRotation = 25f;
    [SerializeField]
    float _rotationSharpness = 5f;
    [SerializeField]
    bool _zeroRollBeyondLimit = true;
    [Header("Camera Shake")]
    [SerializeField]
    bool _shakeEnabled = true;
    [SerializeField]
    float _frequency = 30f;
    [SerializeField]
    [Range(0f, 1f)]
    float _intensity = .5f;
    [SerializeField]
    float _fovMultiplier = 1.1f;
    [SerializeField]
    float _minShakeThrottle = 30f;
    AdvancedPlaneController _player;
    float _basePoV;
    private void Awake()
    {
        _cam = Camera.main;
        _basePoV = _cam.fieldOfView;
        _camTarget = transform;
        _player = GetComponent<AdvancedPlaneController>();
    }

    private void FixedUpdate()
    {
        Follow(_player.transform.position, _player.transform, _player.CurrentThrottle);
    }


    public void Follow(Vector3 player, Transform target, float currentThrottle)
    {
        if (currentThrottle <= _minShakeThrottle)
            _shakeEnabled = false;
        else
            _shakeEnabled = true;

        Vector3 camTargetPos = target.position + target.forward * -_distance + new Vector3(0f, 3f, 0f);
        var camTransform = _cam.transform;
        Vector3 currentPos = _shakeEnabled ? camTransform.position + CameraShake(currentThrottle) : camTransform.position;
     
        camTransform.position = currentPos * CameraSpring(currentThrottle) + camTargetPos * (1 - CameraSpring(currentThrottle)); 

        Vector3 fwd = target.forward;
        Vector3 horizonUp = Vector3.ProjectOnPlane(Vector3.up, fwd);
        float roll = horizonUp.sqrMagnitude > 0.001f
            ? Vector3.SignedAngle(horizonUp, target.up, fwd)
            : 0f; // plane pointing straight up/down

        float targetRoll;
        if (Mathf.Abs(roll) > _maxRotation)
            targetRoll = _zeroRollBeyondLimit ? 0f : Mathf.Sign(roll) * _maxRotation;
        else
            targetRoll = roll;

        // Desired rotation: look at target, then roll around the camera's forward axis
        Vector3 toTarget = target.position - camTransform.position;
        Quaternion look = Quaternion.LookRotation(toTarget, Vector3.up);
        Quaternion desired = look * Quaternion.AngleAxis(targetRoll, Vector3.forward);

        // Frame-rate independent smoothing
        float t = 1f - Mathf.Exp(-_rotationSharpness * Time.deltaTime);
        camTransform.rotation = Quaternion.Slerp(camTransform.rotation, desired, t);

        float fovBonus = CamFoV(currentThrottle);
        _cam.fieldOfView = fovBonus == 0f ? _basePoV : _basePoV + fovBonus;
     
    }

    float CameraSpring(float currentThrottle)
    {
        float throttleModifier = currentThrottle / 1000f;
        float modifiedSpring = _camSpring - throttleModifier;

        return Mathf.Clamp01(modifiedSpring);
    }
    Vector3 CameraShake(float currentThrottle)
    {
        float throttleModifier = currentThrottle / 100f;
        float currentFrequency = _frequency * throttleModifier;

        return new Vector3(Mathf.PerlinNoise(0, Time.time * currentFrequency) * 2-1,0,0) * _intensity;
    }
    float CamFoV(float currentThrottle)
    {
        return _basePoV * (_fovMultiplier * (currentThrottle / 100));
    }
    public void ApplySettings(CameraSettings settings)
    {
        _camSpring = settings.CamSpring;
        _distance = settings.Distance;
        _offset = settings.Offset;
        _maxRotation = settings.MaxRotation;
        _rotationSharpness = settings.RotationSharpness;
        _zeroRollBeyondLimit = settings.ZeroRollBeyondLimit;
        _shakeEnabled = settings.ShakeEnabled;
        _frequency = settings.Frequency;
        _intensity = settings.Intensity;
        
    }

}
