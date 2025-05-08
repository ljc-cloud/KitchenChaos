using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {


    private const string PLAYER_PREFS_BINDINGS = "InputBindings";


    public static GameInput Instance { get; private set; }
    

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    public GameFrameSyncManager.PlayerInputType LocalPlayerInputType { get; private set; }
    public Vector2 MoveVector { get; private set; }

    public enum Binding {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }


    private PlayerInputActions playerInputActions;


    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.canceled += Move_canceled;
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Interact.canceled += Interact_canceled;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.InteractAlternate.canceled += InteractAlternate_canceled;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }
    
    private void Move_canceled(InputAction.CallbackContext obj)
    {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.None;
        MoveVector = Vector2.zero;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.Move;
        MoveVector = obj.ReadValue<Vector2>();
    }

    private void OnDestroy() {
        if (playerInputActions == null) return;
        playerInputActions.Player.Move.performed -= Move_performed;
        playerInputActions.Player.Move.canceled -= Move_canceled;
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.Interact.canceled -= Interact_canceled;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.InteractAlternate.canceled -= InteractAlternate_canceled;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.InteractAlt;
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.None;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.Interact;
        Debug.Log("Interact_performed");
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    
    private void Interact_canceled(InputAction.CallbackContext obj)
    {
        LocalPlayerInputType = GameFrameSyncManager.PlayerInputType.None;
    }


    public Vector2 GetMovementVectorNormalized() {
        return MoveVector.normalized;
    }

    public string GetBindingText(Binding binding) {
        switch (binding) {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound) {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding) {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }

}