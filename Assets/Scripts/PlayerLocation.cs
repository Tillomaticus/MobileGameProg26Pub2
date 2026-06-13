using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class PlayerLocation : MonoBehaviour
{
    enum TrackingMode
    {
        Idle,
        Walking,
        Encounter
    }

    [SerializeField] TextMeshProUGUI _coordinateTextField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("StartLocationService");
    }

    IEnumerator StartLocationService()
    {
        Input.location.Start(10f, 10f);

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

        float latitude = Input.location.lastData.latitude;
        float longitude = Input.location.lastData.longitude;
        float altitude = Input.location.lastData.altitude;


        _coordinateTextField.text = "Lat: " + latitude + "\n Long: " + longitude + "\n Alt: " + altitude;
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

}

