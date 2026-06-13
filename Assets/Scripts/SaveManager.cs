using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 1. Der Wrapper, damit Unitys JsonUtility die Liste serialisieren kann
[System.Serializable]
public class SaveDataWrapper
{
    public List<CreatureData> CaughtCreatures = new List<CreatureData>();
}

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    // SPEICHERN
    public static void SaveCreatures(List<CreatureData> creaturesToSave)
    {
        try
        {
            // Daten in den Wrapper packen
            SaveDataWrapper wrapper = new SaveDataWrapper();
            wrapper.CaughtCreatures = creaturesToSave;

            // In JSON-String umwandeln (true für "prettyPrint", damit man es am PC lesen kann)
            string json = JsonUtility.ToJson(wrapper, true);

            // Datei auf Android schreiben
            File.WriteAllText(SavePath, json);
            
            Debug.Log($"Erfolgreich gespeichert unter: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Speichern: {e.Message}");
        }
    }

    // LADEN
    public static List<CreatureData> LoadCreatures()
    {
        // Wenn noch keine Datei existiert, gib eine leere Liste zurück
        if (!File.Exists(SavePath))
        {
            Debug.Log("Kein Savegame gefunden. Starte mit leerer Liste.");
            return new List<CreatureData>();
        }

        try
        {

            // JSON-Text aus der Datei lesen
            string json = File.ReadAllText(SavePath);

            // Aus dem JSON wieder das Wrapper-Objekt machen
            SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

            Debug.Log("Savegame erfolgreich geladen.");
            return wrapper.CaughtCreatures;
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Laden: {e.Message}");
            return new List<CreatureData>();
        }
    }

    public static void DeleteSaveGame()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Savegame-Datei wurde erfolgreich gelöscht!");
        }
        else
        {
            Debug.LogWarning("Löschen fehlgeschlagen: Keine Savegame-Datei vorhanden.");
        }
    }
}