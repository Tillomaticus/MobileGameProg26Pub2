using System.Collections.Generic;
using UnityEngine;

public class CreatureDatabase : MonoBehaviour
{
    public static CreatureDatabase Instance { get; private set; }

    // Hier ziehst du im Unity-Inspector ALLE deine Pokémon-ScriptableObjects rein
    [SerializeField] private List<ScriptableCreature> _allCreatures;

    private Dictionary<int, ScriptableCreature> _creatureDictionary;


    void Awake()
    {
        // Singleton-Pattern, damit du von überall darauf zugreifen kannst
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Wir packen die Liste in ein Dictionary für blitzschnellen Zugriff über die ID
        _creatureDictionary = new Dictionary<int, ScriptableCreature>();
        foreach (var creature in _allCreatures)
        {
            if (!_creatureDictionary.ContainsKey(creature.ID))
            {
                _creatureDictionary.Add(creature.ID, creature);
            }
        }
    }



    // Die Creature Daten bekommen:
    public ScriptableCreature GetCreatureByID(int id)
    {
        if (_creatureDictionary.TryGetValue(id, out ScriptableCreature data))
        {
            return data;
        }
        Debug.LogError($"Creature mit ID {id} wurde nicht in der Datenbank gefunden!");
        return null;
    }
}


