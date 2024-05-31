using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //PUBLIC
    public static InputManager _INPUT_MANAGER; //SINGELTON

    //PRIVATE
    private InputActions playerInputs;

    ///VARIABLES
    
    //MOVE
        //Forward
    private bool pressedForward = false;
        //Reverse
    private bool pressedReverse = false;
        //TurnLeft
    private bool pressedTurnLeft = false;
        //TurnRight
    private bool pressedTurnRight = false;

    //HANDBRAKE
    private bool pressedHandbrake = false;

    private void Awake()
    {
        if (_INPUT_MANAGER != null && _INPUT_MANAGER != this)
        {
            Destroy(this.gameObject); //Destruir si ya existe un INPUT MANAGER
        }
        else
        {
            //Activar Input Actions
            playerInputs = new InputActions();
            playerInputs.Player.Enable();

            //MOVE
                //Forward
            playerInputs.Player.Forward.performed += ForwardPressed;
            playerInputs.Player.Forward.canceled += ForwardReleased;
                //Reverse
            playerInputs.Player.Reverse.performed += ReversePressed;
            playerInputs.Player.Reverse.canceled += ReverseReleased;
                //TurnLeft
            playerInputs.Player.TurnLeft.performed += TurnLeftPressed;
            playerInputs.Player.TurnLeft.canceled += TurnLeftReleased;
                //TurnRight
            playerInputs.Player.TurnRight.performed += TurnRightPressed;
            playerInputs.Player.TurnRight.canceled += TurnRightReleased;
            //HANDBRAKE
            playerInputs.Player.Handbrake.performed += handBrakePressed;
            playerInputs.Player.Handbrake.canceled += handBrakeReleased;
            
            //Dont destroy on load para cambiar de escenas
            _INPUT_MANAGER = this;
            DontDestroyOnLoad(this);
        }
    }

    //MOVE
        //Forward
    private void ForwardPressed(InputAction.CallbackContext context)
    {
        pressedForward = true;
    }
    private void ForwardReleased(InputAction.CallbackContext context)
    {
        pressedForward = false;
    }
        //Reverse
    private void ReversePressed(InputAction.CallbackContext context)
    {
        pressedReverse = true;
    }
    private void ReverseReleased(InputAction.CallbackContext context)
    {
        pressedReverse = false;

    }
        //TurnLeft
    private void TurnLeftPressed(InputAction.CallbackContext context)
    {
        pressedTurnLeft = true;
    }
    private void TurnLeftReleased(InputAction.CallbackContext context)
    {
        pressedTurnLeft = false;

    }
        //TurnRight
    private void TurnRightPressed(InputAction.CallbackContext context)
    {
        pressedTurnRight = true;
    }
    private void TurnRightReleased(InputAction.CallbackContext context)
    {
        pressedTurnRight = false;
    }

    //HANDBRAKE
    private void handBrakePressed(InputAction.CallbackContext context)
    {
        pressedHandbrake = true;
    }
    private void handBrakeReleased(InputAction.CallbackContext context)
    {
        pressedHandbrake = false;
    }

    //GET FUNCTIONS
        //MOVE
    public bool GetForwardButton()
    {
        return pressedForward;
    }
    public bool GetReverseButton()
    {
        return pressedReverse;
    }
    public bool GetTurnLeftButton()
    {
        return pressedTurnLeft;
    }
    public bool GetTurnRightButton()
    {
        return pressedTurnRight;
    }
        //HANDBRAKE
    public bool GetHandbrakeButton()
    {
        return pressedHandbrake;
    }
}
