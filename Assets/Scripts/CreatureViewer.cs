using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatureViewer : MonoBehaviour
{
    private PlayerInputSystem _playerInput;

    // Creature Selection Variables
   // public List<GameObject> CreatureModels = new List<GameObject>();
 //   GameObject _activeCreature;

    // Rotation Variables
    [SerializeField] float _creatureRotationSpeed = 1f;
    private float _oldXPosition;
    bool _isDragging;

    // Zooming Variables
    [SerializeField] Camera _camera;
    [SerializeField] float _zoomSpeed = 1f;
    [SerializeField] Vector2 _minMaxZoom;
    float _oldDistance = 0;
    bool _isZooming;


    void Awake()
    {
        _playerInput = new PlayerInputSystem();

        if (_camera == null)
            Debug.LogError("Camera not set");
    }

    void Update()
    {
        if (_isZooming)
            HandleZoom();

        // Nur HandleDrag aufrufen, wenn nicht beide Finger touchen -> sonst sind wir im Zoom, und wir wollen nicht beides gleichzeitig
        if (_isDragging && !_isZooming)
            HandleDrag();
    }


    void OnEnable()
    {
        // PlayerInput System anmachen
        _playerInput.Enable();
        // Funktionen mit Event-Callbacks verknüpfen
        _playerInput.Touch.PrimaryTouch.started += OnPrimaryTouchStart;
        _playerInput.Touch.PrimaryTouch.canceled += OnPrimaryTouchEnd;

        _playerInput.Touch.SecondaryTouch.started += OnSecondaryTouchStart;
        _playerInput.Touch.SecondaryTouch.canceled += OnSecondaryTouchEnd;

    }

    void OnDisable()
    {
        // Input Sytem wieder ausmachen
        _playerInput.Disable();

        // Funktionen von Event-Callbacks lösen, um keine leeren Referenzen beizubehalten
        _playerInput.Touch.PrimaryTouch.started -= OnPrimaryTouchStart;
        _playerInput.Touch.PrimaryTouch.canceled -= OnPrimaryTouchEnd;

        _playerInput.Touch.SecondaryTouch.started -= OnSecondaryTouchStart;
        _playerInput.Touch.SecondaryTouch.canceled -= OnSecondaryTouchEnd;
    }


    #region Pinch & Zoom
    void OnSecondaryTouchStart(InputAction.CallbackContext context)
    {
        _isZooming = true;

        // zwischenspeichern der Fingerpositionen 
        Vector2 firstFingerPosition = _playerInput.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
        Vector2 secondFingerPosition = _playerInput.Touch.SecondaryTouchPosition.ReadValue<Vector2>();

        // überschreiben des Wert für die alte Distanz um ein Snapping zu verhindern, wenn wir den Pinch neu ansetzen.
        _oldDistance = Vector2.Distance(firstFingerPosition, secondFingerPosition);
    }

    void OnSecondaryTouchEnd(InputAction.CallbackContext context)
    {
        _isZooming = false;
    }

    void HandleZoom()
    {
        // beide Finger positionen speichern
        Vector2 firstFingerPosition = _playerInput.Touch.PrimaryTouchPosition.ReadValue<Vector2>();
        Vector2 secondFingerPosition = _playerInput.Touch.SecondaryTouchPosition.ReadValue<Vector2>();

        // Distanz zwischen den Fingern berechnen
        float distance = Vector2.Distance(firstFingerPosition, secondFingerPosition);

        // delta zwischen aktueller und letzter Distanz
        float delta = distance - _oldDistance;

        // neue Camera-Size berechnen - statt plus um die zoomrichtung zu ändern
        float newSize = _camera.orthographicSize - delta * _zoomSpeed;
        //camera Size clampen um nicht zu klein/zu groß zu werden 
        newSize = Mathf.Clamp(newSize, _minMaxZoom.x, _minMaxZoom.y);

        _camera.orthographicSize = newSize;

        Debug.Log(_camera.orthographicSize);

        // aktuelle Distanz fürs nächste Frame speichern
        _oldDistance = distance;
    }
    #endregion


    #region Drag & Turn

    void OnPrimaryTouchStart(InputAction.CallbackContext context)
    {
        _isDragging = true;

        // überschreiben der altenPosition mit dem aktuellen Wert, um "springen" zu verhindern, wenn wir neu ansetzen
        _oldXPosition = _playerInput.Touch.PrimaryTouchPosition.ReadValue<Vector2>().x;
    }


    void OnPrimaryTouchEnd(InputAction.CallbackContext context)
    {
        _isDragging = false;

        //Auch das Zoomen beenden, weil wir plötzlich im nicht mehr beide Finger drauf haben
        _isZooming = false;
    }

    void HandleDrag()
    {
        //aktuelle X Position holen
        float currentXPostion = _playerInput.Touch.PrimaryTouchPosition.ReadValue<Vector2>().x;

        //Delta berechnen
        float delta = currentXPostion - _oldXPosition;

        //Creature um die Y-Achse rotieren mit Speed * delta. *-1 weil wir die Rotation invertiert wollen -> Finger zieht nach links, Creature rotiert links herum
        _:transformHandle.Rotate(Vector3.up, delta * _creatureRotationSpeed * -1);

        //speichern der aktuellen xPosition für das nächste Frame
        _oldXPosition = currentXPostion;
    }
    #endregion



    #region Select Creature
    void Start()
    {
        /* Alter Creature Select Code
        //Ist die Liste gefüllt? Wenn nein dann Error werfen
        if (CreatureModels.Count < 1)
        {
            Debug.LogError("Creature List Empty!");
            return;
        }
        //Aktive Creature setzen
        _activeCreature = CreatureModels[0];

        //aktive Creature anschalten
        _activeCreature.SetActive(true);
        */
    }


/*
    public void SelectCreature(int index)
    {
        if (index >= CreatureModels.Count)
        {
            Debug.LogError("Selected Creature not found!");
            return;
        }

        // altes Creature Gameobject ausmachen
        _activeCreature.SetActive(false);

        // aktive Creaute mit gewählter Creature überschreiben
        _activeCreature = CreatureModels[index];

        //Creature Rotation resetten (sieht cooler aus)
        _activeCreature.transform.rotation = Quaternion.Euler(0, 180, 0);

        // Neues active Creautre anschalten
        _activeCreature.SetActive(true);
    }
*/
    #endregion 

}
