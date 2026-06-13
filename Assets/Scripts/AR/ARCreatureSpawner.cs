using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCreatureSpawner : MonoBehaviour
{
    [SerializeField] private GameObject creaturePrefab; // Deine Kreatur hier reinziehen
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private float scanDuration = 4.0f; // Zeit in Sekunden für den Raumscan
    private GameObject spawnedCreature;



    private bool isScanning = true;
    private float scanTimer = 0f;


    void Start()
    {
        InvokeRepeating("ScanForPlanes", 0f, 1f);
    }

    void ScanForPlanes()
    {
        if (!isScanning || spawnedCreature != null) return;

        // "Timer" entsrpechend Invoke Repeating Frequenz erhöhen
        scanTimer += 1f;

        if (scanTimer >= scanDuration)
        {
            isScanning = false;
            CancelInvoke(nameof(ScanForPlanes)); // Schleife stoppen

            // Spawning starten
            SpawnProcess();
        }
    }



    private void SpawnProcess()
    {
        //early out if to prevent null refs
        if (GameplayManager.Instance == null)
        {
            Debug.LogError("Gameplay Manager is null");
            return;
        }

        if (GameplayManager.Instance.SelectedCreature  == null)
        {
            Debug.LogError("Selected Creature is null");
            return;
        }


        // Beste Plane suchen
        ARPlane bestPlane = null;
        float largestArea = 0f;

        // Wir gehen durch alle aktuell erkannten Flächen durch
        foreach (var plane in planeManager.trackables)
        {
            // Die Größe der Fläche berechnen (Breite * Länge des Extents)
            float planeArea = plane.extents.x * plane.extents.y;

            if (planeArea > largestArea)
            {
                largestArea = planeArea;
                bestPlane = plane;
            }
        }


        if (bestPlane == null)
        {
            Debug.LogWarning("Keine Plane gefunden. Starte Suche neu...");
            ResetScan(); // Scan neustarten
        }
        else
        {
            // Position auf der Plane bestimmen
            Vector3 spawnPosition = bestPlane.center;

            // Berechnen die Richtung zum Spieler (Kamera)
            Vector3 directionToCamera = Camera.main.transform.position - spawnPosition;

            // Wir nullen die Y-Achse, damit das Monster nicht nach oben/unten kippt, falls der Tisch tiefer liegt
            directionToCamera.y = 0;

            // Erzeugt eine Rotation, die exakt in diese Richtung blickt
            Quaternion spawnRotation = Quaternion.LookRotation(directionToCamera);

            // 1. Die Kreatur ganz normal in die Welt setzen
            spawnedCreature = Instantiate(creaturePrefab, spawnPosition, spawnRotation);
            spawnedCreature.GetComponent<CreatureLogic>().SetCreatureType(GameplayManager.Instance.SelectedCreature);

            // 2. Der "Magic Trick": Wir fügen der Kreatur einfach die ARAnchor-Komponente hinzu.
            // AR Foundation verwandelt das Monster dadurch automatisch im Hintergrund in einen festen Anker!
            spawnedCreature.AddComponent<ARAnchor>();

            Debug.Log("Kreatur erfolgreich und simpel verankert!");
            DisablePlaneVisuals();
        }
    }


    private void ResetScan()
    {
        isScanning = true;
        scanTimer = 0f;
        InvokeRepeating(nameof(ScanForPlanes), 0.5f, 1f);
    }

    private void DisablePlaneVisuals()
    {
        planeManager.enabled = false;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

}