using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDemoTouch : MonoBehaviour
{
    private PlayerInputSystem _playerInput;

    bool _isDragging = false;

    void Awake()
    {
        _playerInput = new PlayerInputSystem();
    }

    void Update()
    {
        if (_isDragging)
            HandleDrag();
    }

    void OnEnable()
    {
        // PlayerInput System anmachen
        _playerInput.Enable();
        // Funktionen mit Event-Callbacks verknüpfen
        _playerInput.Touch.PrimaryTouch.performed += OnPrimaryTouch;
        _playerInput.Touch.PrimaryTouch.started += OnPrimaryTouchStart;
        _playerInput.Touch.PrimaryTouch.canceled += OnPrimaryTouchEnd;

    }

    void OnPrimaryTouch(InputAction.CallbackContext context)
    {
        Debug.Log("I feel touched!");
    }

    void HandleDrag()
    {
        // Auslesen der FingerPosition via Vector2 Variable"
        Debug.Log("Finger at position " + _playerInput.Touch.PrimaryTouchPosition.ReadValue<Vector2>());
    }

    void OnPrimaryTouchStart(InputAction.CallbackContext context)
    {
        Debug.Log("They touched me");
        _isDragging = true;
    }


    void OnPrimaryTouchEnd(InputAction.CallbackContext context)
    {
        Debug.Log("Its over");
        _isDragging = false;
    }

    void OnDisable()
    {
        // Input Sytem wieder ausmachen
        _playerInput.Disable();

        // Funktionen von Event-Callbacks lösen, um keine leeren Referenzen beizubehalten
        _playerInput.Touch.PrimaryTouch.performed -= OnPrimaryTouch;
        _playerInput.Touch.PrimaryTouch.started -= OnPrimaryTouchStart;
        _playerInput.Touch.PrimaryTouch.canceled -= OnPrimaryTouchEnd;
    }
}
