using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour, PlayerInputMap.IPlayerActions
{    
    PlayerInputMap _inputMap;
    Vector2 _moveDir;

    public bool Up { get; private set; }
    public bool Down { get; private set; }
    public Vector2 MoveDir { get; private set; }
    public Vector2 LookDir { get; private set; }

    //private void OnEnable()
    //{
    //    if(_inputMap == null)
    //    {
    //        _inputMap = new PlayerInputMap();

    //        //Tell the action map that we want to know when actions get triggered
    //        _inputMap.Player.SetCallbacks(this);
    //    }

    //    //_inputMap.Player.Enable();
    //    _inputMap.PlaneInput.Enable();
    //}

    //private void OnDisable()
    //{
    //    _inputMap.PlaneInput.Disable();
    //    _inputMap.Player.Disable();
    //}

    public void OnMove(InputAction.CallbackContext c)
    {
        MoveDir = c.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        LookDir = c.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext c)
    {
        if(c.performed)
            Down = true;
        if(c.canceled)
            Down=false; 
    }

    public void OnJump(InputAction.CallbackContext c)
    {
        if(c.performed)
            Up = true;
        if(c.canceled)
            Up=false;
    }

    public void OnPrevious(InputAction.CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext c)
    {
        throw new System.NotImplementedException();
    }


}
