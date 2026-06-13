using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject catchCanvas;

    [SerializeField] private GameObject _aRBaitSpawnerGameObject;


    public static ARSceneManager Instance;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void StartEndRoutine()
    {
        StartCoroutine(TriggerCatchUIRoutine()); // UI-Verzögerung starten
    }

    // Die Coroutine wartet eine Sekunde und schaltet dann die UI ein
    IEnumerator TriggerCatchUIRoutine()
    {
        _aRBaitSpawnerGameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        if (catchCanvas != null)
        {
            catchCanvas.SetActive(true);
            Debug.Log("Fangen-UI wurde aktiviert!");
        }
    }


    public void LoadMainScene()
    {
        SceneManager.LoadScene("MapScene");
    }

}
