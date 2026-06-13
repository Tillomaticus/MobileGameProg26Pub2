using TMPro;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    float _timer = 0f;
    string _oldColorName = "white";

    Color[] _colors = { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow };
    string[] _colorNames = { "Red", "Green", "Blue", "Cyan", "Magenta", "Yellow" };


    //Advanced im Inspector
    [System.Serializable]
    public class NamedColor
    {
        public Color color;
        public string name;
    }
    [SerializeField]
    NamedColor[] _namedColors;


    [SerializeField]
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    TextMeshPro _textMeshPro;



    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime; //gibt die Zeit seit dem letzten Frame aus

        if (_timer >= 1f)
        {
            _timer -= 1f;
            TriggerColorSwitch();
            //TriggerNamedColorSwitch();
        }

    }



    void TriggerColorSwitch()
    {
        //Random Color generieren
        int randomColor = Random.Range(0, _colors.Length);

        //Farbe auf Sprite malen
        _spriteRenderer.color = _colors[randomColor];
        //neuen Farbnamen speichern
        string newColorName = _colorNames[randomColor];

        //Text im Sprite mit Namen der alten Farbe setzen
        _textMeshPro.text = _oldColorName;

        //Debug Log schreiben
        Debug.Log("Time: " + Time.realtimeSinceStartup + " old Color:  " + _oldColorName + " new Color: " + newColorName);

        //Name der neuen Farbe für nächstes mal speichern
        _oldColorName = newColorName;
    }


    void TriggerNamedColorSwitch()
    {
        //Random Color generieren
        int randomColor = Random.Range(0, _namedColors.Length);

        //Farbe auf Sprite malen
        _spriteRenderer.color = _namedColors[randomColor].color;
        //neuen Farbnamen speichern
        string newColorName = _namedColors[randomColor].name;

        //Text im Sprite mit Namen der alten Farbe setzen
        _textMeshPro.text = _oldColorName;

        //Debug Log schreiben im String Interpolation Format und in LogFormat, geben beide den identischen Log aus
        Debug.Log($"Time: {Time.realtimeSinceStartup} old Color: {_oldColorName} new Color: {newColorName}");
        //Debug.LogFormat("Time2: {0} old Color: {1} new Color: {2}", Time.realtimeSinceStartup,_oldColorName,newColorName);

        //Name der alten Farbe für nächstes mal speichern
        _oldColorName = newColorName;
    }
}
