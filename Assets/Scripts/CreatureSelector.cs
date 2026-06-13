using UnityEngine;

public class CreatureSelector : MonoBehaviour
{
    private int _currentIndex = 0;       // Das aktuell angezeigte Monster
    private GameObject _currentModel;    // Referenz auf das gerade aktive 3D-Modell
    [SerializeField] private Transform _spawnPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameplayManager.Instance.CaughtCreatures.Count > 0)
        {
            _currentIndex = 0;
            ShowCreature(_currentIndex);
        }
        else
        {
            // Text oder Icon für "keine Creature gefangen"
            Debug.Log("Inventar ist leer, nichts zum Anzeigen da.");
        }
    }


    private void ShowCreature(int index)
    {
        // Sicherheits-Check
        if (index < 0 || index >=  GameplayManager.Instance.CaughtCreatures.Count) return;

        // 1. Altes Modell zerstören, falls eins da ist
        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        // 2. Die aktuellen Daten aus der Liste holen
        CreatureData currentData = GameplayManager.Instance.CaughtCreatures[index];

        // 3. Den Lookup in der Datenbank machen (Die Brücke zum ScriptableObject)
        ScriptableCreature scriptableCreature = CreatureDatabase.Instance.GetCreatureByID(currentData.ScriptableID);

        if (scriptableCreature != null && scriptableCreature.Model != null)
        {
            // 4. Neues Modell am Spawnpoint instanziieren
            _currentModel = Instantiate(scriptableCreature.Model, _spawnPoint.position, _spawnPoint.rotation, _spawnPoint);
        }
  
    }


    // WIRD VOM "NEXT"-BUTTON AUFGERUFEN
    public void OnNextButtonClicked()
    {
        if (GameplayManager.Instance.CaughtCreatures.Count == 0) return;

        _currentIndex++;

        // Loop-Effekt: Wenn wir am Ende sind, springe wieder zum ersten Monster (Index 0)
        if (_currentIndex >= GameplayManager.Instance.CaughtCreatures.Count)
        {
            _currentIndex = 0;
        }

        ShowCreature(_currentIndex);
    }

    // WIRD VOM "PREVIOUS"-BUTTON AUFGERUFEN
    public void OnPreviousButtonClicked()
    {
        if (GameplayManager.Instance.CaughtCreatures.Count == 0) return;

        _currentIndex--;

        // Loop-Effekt: Wenn wir vor dem ersten sind, springe zum letzten Monster der Liste
        if (_currentIndex < 0)
        {
            _currentIndex = GameplayManager.Instance.CaughtCreatures.Count - 1;
        }

        ShowCreature(_currentIndex);
    }
}

