using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawnerPoolVersion : MonoBehaviour
{



    float _metersPerDegreeLong;
    const float _metersPerDegreeLat = 111320f; // 111.320m = 111,32km 

    [SerializeField] PlayerLocationForMap _playerLocation;


    // in Unity Units = Meter
    [SerializeField] float _radius;

    [SerializeField] GameObject _creaturePrefab;

    // GameObject that is Child of CesiumGeoReference so we spawn the Creatures correctly.
    [SerializeField] Transform _parentTransform;

    Queue<MapCreature> _mapCreaturePool = new Queue<MapCreature>();
    [SerializeField] float _mapCreaturePoolSize = 5;

    [SerializeField] List<ScriptableCreature> _possibleSpawnableCreatures;


    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < _mapCreaturePoolSize; i++)
        {
            //Spawn Creature and add to Pool;
            MapCreature newCreature = Instantiate(_creaturePrefab, _parentTransform).GetComponent<MapCreature>();
            ReturnCreatureToPool(newCreature);
            newCreature.gameObject.SetActive(false);
        }
    }

    MapCreature TryGetCreatureFromPool()
    {
        if (_mapCreaturePool.Count == 0)
            return null;

        return _mapCreaturePool.Dequeue(); ;
    }

    public void ReturnCreatureToPool(MapCreature creature)
    {
        creature.gameObject.SetActive(false);
        _mapCreaturePool.Enqueue(creature);
    }


    void SpawnCreature()
    {
        MapCreature newMapCreature = TryGetCreatureFromPool();
        if (newMapCreature == null)
            return;
        else
        {
            newMapCreature.gameObject.SetActive(true);
  //          newMapCreature.SpawnCreature(GetRandomPointAroundPlayer());
        }
    }


    Vector2 GetRandomPointAroundPlayer()
    {
        // Zufälliger Punkt im Kreis
        Vector2 random = Random.insideUnitCircle * _radius;

        // Umberechnung der MeterProGrad Longitude, alternativ auch einfach 111320f nehmen
        _metersPerDegreeLong = 111320f * Mathf.Cos(_playerLocation.GetPlayerCoords().x * Mathf.Deg2Rad);

        // Umrechnung Meter → GPS °
        var lat = _playerLocation.GetPlayerCoords().x + (random.y / _metersPerDegreeLat);
        var lon = _playerLocation.GetPlayerCoords().y + (random.x / _metersPerDegreeLong);

        return new Vector2(lat, lon);
    }





}
