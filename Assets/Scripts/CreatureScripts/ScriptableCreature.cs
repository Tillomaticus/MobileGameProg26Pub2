using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCreature", menuName = "Scriptable Objects/ScriptableCreature")]
public class ScriptableCreature : ScriptableObject
{
    public int ID;
    public string Name;
    public string Type;
    public GameObject Model;
    public Sprite Icon;

    public int BaseHp;
    public int BaseAttack;
}
