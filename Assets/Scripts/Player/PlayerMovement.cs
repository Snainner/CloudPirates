using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public DebugTools _dTools;
    public Rigidbody Rb;
    Camera _cam;
    PlayerController _controller;

    public float MoveSpeed = 15f;

    [Header("Camera")]
    float _camCurrentXAngle;
    float _camCurrentYAngle;
    [SerializeField]
    float _camSpeed;


    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        _controller = GetComponent<PlayerController>();

        _cam.transform.position = transform.position;
        _cam.transform.localEulerAngles = Vector3.zero;

    }
    private void Update()
    {
        RotateCamera(_controller.LookDir);
    }
    private void LateUpdate()
    {
        HandleMovement(_controller.MoveDir);
        VerticalMovement(_controller.Up, _controller.Down);
        //if( _controller.DownHeld )
        //    Rb.AddForce(Vector3.down * MoveSpeed);
        //if(_controller.UpHeld)
        //    Rb.AddForce(Vector3.up * MoveSpeed);

    }
    public void VerticalMovement(bool up, bool down)
    {
        if(up)
            Rb.AddForce(Vector3.up * MoveSpeed);
        if(down)
            Rb.AddForce(Vector3.down * MoveSpeed);

    }
    public void RotateCamera(Vector2 lookInput, bool smoothRotation = false)
    {
        float cameraSmoothingFactor = 1f;
        if (smoothRotation)
        {
            lookInput.y = Mathf.Lerp(0, lookInput.y, Time.deltaTime * cameraSmoothingFactor);
            lookInput.x = Mathf.Lerp(0, lookInput.x, Time.deltaTime * cameraSmoothingFactor);
        }

        _camCurrentXAngle += -lookInput.y * _camSpeed * Time.deltaTime;
        _camCurrentYAngle += lookInput.x * _camSpeed * Time.deltaTime;

        _cam.transform.localRotation = Quaternion.Euler(_camCurrentXAngle, _camCurrentYAngle, 0);
    }

    public void HandleMovement(Vector2 inputDir)
    {
        Vector3 dir = CalculateMovementDirFromCamAngle(inputDir);

        Rb.AddForce(dir * MoveSpeed);
    }

    //Probably better to avoid camera spin, but will probably suck for air combat
    Vector3 CalculateMovementDirFromCamAngle(Vector2 InputDir)
    {
        Vector3 direction = _cam.transform == null ?
            transform.right * InputDir.x + transform.forward * InputDir.y :
            Vector3.ProjectOnPlane(_cam.transform.right, transform.up).normalized * InputDir.x +
            Vector3.ProjectOnPlane(_cam.transform.forward, transform.up).normalized * InputDir.y;

        return direction.magnitude > 1f ? direction.normalized : direction;
    }

    //Can cause the camera to spin
    Vector3 CalculateMoveDir(Vector2 inputDir)
    {
        Vector3 moveDir = transform.right * inputDir.x + transform.forward * inputDir.y;

        //moveDir.y = 0f;
        moveDir.Normalize();

        return moveDir * MoveSpeed;
    }
}
