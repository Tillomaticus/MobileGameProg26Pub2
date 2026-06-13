using System;
using UnityEngine;

public class CreatureLogic : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float stopDistance = 0.1f; // Wie nah muss sie ran? (10cm)

    private Transform targetBait;
    private bool isMoving = false;

    [SerializeField] Transform _visualRoot;
    ScriptableCreature _scriptableCreature;

    void OnDisable()
    {
        // Unbedingt immer desubscriben, um Memory Leaks zu verhindern!
        ARBaitSpawner.OnBaitPlaced -= HandleNewBait;
    }

    void OnEnable()
    {
        // Auf das statische Event vom BaitSpawner subscriben
        ARBaitSpawner.OnBaitPlaced += HandleNewBait;
    }

    public void SetCreatureType(ScriptableCreature scriptableCreature)
    {
        _scriptableCreature = scriptableCreature;
        GameObject creatureVisuals = Instantiate(_scriptableCreature.Model, _visualRoot);
    }

    // Wird aufgerufen, sobald das Event im Spawner feuert
    private void HandleNewBait(Transform baitTransform)
    {
        targetBait = baitTransform;
        isMoving = true;
    }

    void Update()
    {
        // Wenn wir kein Ziel haben oder nicht laufen sollen, tun wir nichts
        if (!isMoving || targetBait == null) return;

        // 1. Blickrichtung zum Köder drehen (sieht natürlicher aus)
        Vector3 targetPosition = new Vector3(targetBait.position.x,
        transform.position.y,
        targetBait.position.z);

        transform.LookAt(targetPosition);

        // 2. Auf den Köder zubewegen
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 3. Distanz prüfen: Sind wir nah genug dran?
        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            CatchLogic();
        }
    }

    void CatchLogic()
    {
        isMoving = false; // Bewegung stoppen
        ARSceneManager.Instance.StartEndRoutine();

        CreatureData caughtCreature = new CreatureData();
        // ID zuweisen
        caughtCreature.ScriptableID = _scriptableCreature.ID;
        // Fangzeitpunkt speichern
        caughtCreature.CaughtDateTime = DateTime.Now;

        // Creature Data in Inventar übertragen
        GameplayManager.Instance.AddCaughtCreature(caughtCreature);

        Invoke("DisableSelf", 0.9f);
    }

    void DisableSelf()
    {
        this.gameObject.SetActive(false);
    }



}
