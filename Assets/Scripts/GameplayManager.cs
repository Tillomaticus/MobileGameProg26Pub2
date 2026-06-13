using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public static GameplayManager Instance;

    // ReadOnly, schreiben nur über SetSelectedCreature
    public ScriptableCreature SelectedCreature { get; private set; }


    public List<CreatureData> CaughtCreatures = new List<CreatureData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        CaughtCreatures = SaveManager.LoadCreatures();
    }

    public void SetSelectedCreature(ScriptableCreature scriptableCreature)
    {
        SelectedCreature = scriptableCreature;
    }
    public void UnSelectCreature()
    {
        SelectedCreature = null;
    }


    public void AddCaughtCreature(CreatureData caughtCreature)
    {
        CaughtCreatures.Add(caughtCreature);
        SaveManager.SaveCreatures(CaughtCreatures);
    }





}
