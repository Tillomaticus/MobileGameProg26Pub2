using UnityEngine;
using UnityEngine.InputSystem;

public class InputDemo : MonoBehaviour
{

    [SerializeField]
    private PlayerInputSystem _playerInput;

    void Awake()
    {
        _playerInput = new PlayerInputSystem();
    }


    void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Player.Debug.performed += OnDebugAction;
    }


    void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Player.Debug.performed -= OnDebugAction;
    }

    void OnDebugAction(InputAction.CallbackContext context)
    {
        Debug.Log("Yo yo yo");
    }
}
