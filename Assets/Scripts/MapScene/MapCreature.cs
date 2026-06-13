using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapCreature : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CesiumGlobeAnchor _cesiumGlobeAnchor;

    [SerializeField] float _minimumDistanceForCatching = 5f;
    [SerializeField] GameObject _catchingVisuals;

    [SerializeField] Transform _visualRoot;
    ScriptableCreature _scriptableCreature;




    bool _inCatchingRange = false;

    // For testing only
    // void Start()
    // {

    //     SpawnCreature(new Vector2(8.65150f, 49.41439f));
    // }

    void Update()
    {
        CalculateDistanceToPlayer();
    }

    void CalculateDistanceToPlayer()
    {
        Vector2 creature = new Vector2(this.transform.position.x, this.transform.position.z);
        float dist = creature.magnitude;

        if (dist <= _minimumDistanceForCatching)
        {
            SetCatchingVisualsActive(true);
            _inCatchingRange = true;
        }
        else
        {
            SetCatchingVisualsActive(false);
            _inCatchingRange = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_inCatchingRange)
            return;

Debug.Log(GameplayManager.Instance + " " + _scriptableCreature);
        GameplayManager.Instance.SetSelectedCreature(_scriptableCreature);
        SceneManager.LoadScene("ARCreatureCatch");
    }

    void SetCatchingVisualsActive(bool active)
    {
        _catchingVisuals.SetActive(active);
    }



    public void SpawnCreature(Vector2 coordinates, ScriptableCreature scriptableCreatureToSpawn)
    {
        // scriptableObject speichern
        _scriptableCreature = scriptableCreatureToSpawn;
        // Visuals instanziieren und unter die Root hängen
        GameObject creatureVisuals = Instantiate(_scriptableCreature.Model, _visualRoot);

        _cesiumGlobeAnchor.longitudeLatitudeHeight = new double3(coordinates.y, coordinates.x, 0);
        _cesiumGlobeAnchor.Sync();
        AdjustHeight();
    }

    void AdjustHeight()
    {
        Ray ray = new Ray(Vector3.up * 2000, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }
}
