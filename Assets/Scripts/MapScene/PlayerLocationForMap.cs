using System.Collections;
using CesiumForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PlayerLocationForMap : MonoBehaviour
{
    enum TrackingMode
    {
        Idle,
        Walking,
        Encounter
    }

    [SerializeField] TextMeshProUGUI _coordinateTextField;
    [SerializeField] TextMeshProUGUI _modeTextField;

    [SerializeField] TextMeshProUGUI _targetTextFieldLat;
    [SerializeField] TextMeshProUGUI _targetTextFieldLong;

    [SerializeField] TextMeshProUGUI _targetTextFieldDistance;

    [SerializeField] Image _strollNorthButton;
    [SerializeField] Image _strollEastButton;

    [SerializeField] CesiumGeoreference _cesiumGeoRef;
    [SerializeField] CameraAdjuster _cameraAdjuster;
    [SerializeField] PlayerAvatar _playerAvatar;

    Vector3 _playerCoords;
    Vector2 _targetCoords;

    [SerializeField] float _movementThreshold = 0.1f;
    private Vector3 lastAcceleration;

    [SerializeField] bool _simpleDistance = true;

    [SerializeField] bool _useMockLocation = true;

    [SerializeField] float _mockAlt;
    [SerializeField] float _mockHeading = 0;

    bool _strollEast = false;
    bool _strollNorth = false;

    const float _metersPerDegreeLat = 111320f; // 111.320m = 111,32km 


    TrackingMode _currentTrackingMode;

    Vector2 _mockedPlayerPosition;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetCoords = new Vector2(49.41399f, 8.65110f);
        _targetTextFieldLat.text = _targetCoords.y.ToString();
        _targetTextFieldLong.text = _targetCoords.x.ToString();

        _modeTextField.text = "Mode: " + _currentTrackingMode.ToString();


        if (_useMockLocation)
            StartMockLocationSerice();
        else
            StartCoroutine("StartLocationService");


    }

    void Update()
    {
        UpdateAcceleration();
    }

    public Vector3 GetPlayerCoords()
    {
        return _playerCoords;
    }


    void StartMockLocationSerice()
    {
        SetPos1();
        InvokeRepeating("UpdatePlayerPositionMock", 1f, 1f);
    }

    IEnumerator StartLocationService()
    {
        Input.location.Start(10f, 10f);
        Input.compass.enabled = true;

        // Check if permission is already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            // Wait for the user to click "Allow"
            yield return new WaitForSeconds(2);
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has GPS turned off in Android Settings");
            yield break;

        }

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
            Debug.Log(maxWait);
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            Debug.Log("Location ready");
            InvokeRepeating("UpdatePlayerPosition", 1f, 1f);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Shit!");
        }
    }

    void UpdatePlayerPosition()
    {
        _playerCoords.x = Input.location.lastData.latitude;
        _playerCoords.y = Input.location.lastData.longitude;
        _playerCoords.z = Input.location.lastData.altitude;

        _coordinateTextField.text = "Lat: " + _playerCoords.x + "\nLong: " + _playerCoords.y;
        UpdateGeoReference();
        UpdateHeading();
        UpdateDistance();
    }

    void UpdatePlayerPositionMock()
    {
        if (_strollEast)
            _mockedPlayerPosition.y += (60f / 111320f);

        if (_strollNorth)
            _mockedPlayerPosition.x += (1f / 111320f);

        _playerCoords.x = _mockedPlayerPosition.x;
        _playerCoords.y = _mockedPlayerPosition.y;
        _playerCoords.z = _mockAlt;

        _coordinateTextField.text = "Lat: " + _playerCoords.x + "\nLong: " + _playerCoords.y;
        UpdateGeoReference();
        UpdateHeading();
        UpdateDistance();
    }

    public void OverwriteMockPlayerPosition(Vector2 newPosition)
    {
        _mockedPlayerPosition = newPosition;
    }

    void UpdateGeoReference()
    {
        _cesiumGeoRef.SetOriginLongitudeLatitudeHeight(_playerCoords.y, _playerCoords.x, _playerCoords.z);
        _cameraAdjuster.AdjustHeight();
        _playerAvatar.AdjustHeight();
    }

    void UpdateHeading()
    {
        float heading;
        if (_useMockLocation)
            heading = _mockHeading;
        else
            heading = Input.compass.trueHeading; // 0° = Nord, 90° = Ost

        _playerAvatar.SetLookDirection(heading);
    }

    void UpdateAcceleration()
    {
        Vector3 current = Input.acceleration;
        float delta = (current - lastAcceleration).magnitude;
        lastAcceleration = current;

        if (delta > _movementThreshold)
            _playerAvatar.SetPlayerState(PlayerAvatar.PlayerState.Walking);
        else
            _playerAvatar.SetPlayerState(PlayerAvatar.PlayerState.Idle);
    }

    public void ToggleStrollNorth()
    {
        _strollNorth = !_strollNorth;
        _strollNorthButton.color = _strollNorth ? Color.green : Color.white;
    }
    public void ToggleStrollEast()
    {
        _strollEast = !_strollEast;
        _strollEastButton.color = _strollEast ? Color.green : Color.white;
    }

    public void SetTargetCoords(float targetLat, float targetLong)
    {
        _targetCoords.x = targetLat;
        _targetCoords.y = targetLong;
    }

    void SetTrackingMode(TrackingMode mode)
    {
        Input.location.Stop();

        switch (mode)
        {
            case TrackingMode.Idle:
                Input.location.Start(100f, 50f);
                break;
            case TrackingMode.Walking:
                Input.location.Start(10f, 10f);
                break;
            case TrackingMode.Encounter:
                Input.location.Start(5f, 2f);
                break;
        }
    }

    public void SwapTrackingMode()
    {
        //if we are at last Enum State, go to beginning
        if (_currentTrackingMode == TrackingMode.Encounter)
            _currentTrackingMode = TrackingMode.Idle;
        else
            // else just increase enum
            _currentTrackingMode++;

        SetTrackingMode(_currentTrackingMode);
        _modeTextField.text = "Mode: " + _currentTrackingMode.ToString();
    }

    void SetMockPlayerPos(float lat, float lon)
    {
        _mockedPlayerPosition = new Vector2(lat, lon);
    }


    public void SetPos1()
    {
        SetMockPlayerPos(49.41399f, 8.65110f);
    }

    public void SetPos2()
    {
        SetMockPlayerPos(49.414239f, 8.65092f);
    }

    public void SetPos3()
    {
        SetMockPlayerPos(49.37888f, 8.67521f);
    }

    void UpdateDistance()
    {
        float distance;
        if (_simpleDistance)
            distance = GetSimpleDistanceForPositions(_playerCoords, _targetCoords);
        else
            distance = GetComplexDistanceForPositions(_playerCoords, _targetCoords);


        _targetTextFieldDistance.text = "Distance " + distance.ToString() + "m";
    }

    float GetSimpleDistanceForPositions(Vector2 positionA, Vector2 positionB)
    {
        float differenceDegree = Vector2.Distance(positionA, positionB);
        float distanceMeters = differenceDegree * _metersPerDegreeLat;
        return distanceMeters;
    }


    float GetComplexDistanceForPositions(Vector2 positionA, Vector2 positionB)
    {
        // Wir bekommen aus GoogleMaps Lat/Long
        // Und LONG -> W/E, also Unity x-Achse
        // position.x = latitude
        // position.y = longitude

        float latA = positionA.x;
        float latB = positionB.x;
        float lonA = positionA.y;
        float lonB = positionB.y;


        // first find average Latitude
        float averageLat = (positionA.x + positionB.x) / 2;

        // calculate meters per degree
        // Für lat nehmen wir die Konstante const float _metersPerDegreeLat = 111320f;
        // Für long berechnen wir die Meter nach der Formel:  111.32 * cos(lat)
        // Wichtig! Mathf.Cos erwartet Radians! Deswegen umrechnen!
        float metersPerDegreeLong = 111320f * Mathf.Cos(averageLat * Mathf.Deg2Rad);

        // Differenz in Grad
        float dLat = latB - latA;
        float dLon = lonB - lonA;

        // In Meter umrechnen
        float x = dLon * metersPerDegreeLong;
        float y = dLat * _metersPerDegreeLat;


        // Anschließend euklidische Distanz 
        return Mathf.Sqrt(x * x + y * y);
    }
}
