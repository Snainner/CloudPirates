using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneInput : MonoBehaviour, PlayerInputMap.IPlaneInputActions
{
    PlayerInputMap _map;
    public float Pitch { get; private set; }
    public float Roll { get; private set; }
    public float Yaw { get; private set; }
    public float Throttle {  get; private set; }
    public bool IsShooting {  get; private set; }
    private void OnEnable()
    {
        if (_map == null)
        {
            _map = new PlayerInputMap();
            _map.PlaneInput.SetCallbacks(this);
        }
        _map.PlaneInput.Enable();
    }
    private void OnDisable()
    {
        _map.PlaneInput.Disable();
    }

    public void OnPitch(InputAction.CallbackContext c)
    {
        Pitch = c.ReadValue<float>();
    }


    public void OnRoll(InputAction.CallbackContext c)
    {
        Roll = c.ReadValue<float>();
    }

    public void OnThrottle(InputAction.CallbackContext c)
    {
        Throttle = c.ReadValue<float>();
    }

    public void OnYaw(InputAction.CallbackContext c)
    {
        Yaw = c.ReadValue<float>();
    }

    public void OnShoot(InputAction.CallbackContext c)
    {
        if(c.performed)
            IsShooting = true;
        if (c.canceled)
            IsShooting = false; 
    }

}
