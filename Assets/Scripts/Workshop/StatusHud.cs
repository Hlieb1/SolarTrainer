using UnityEngine;

public class StatusHud : MonoBehaviour
{
    static StatusHud _instance;

    [TextArea] public string controls = "W/S - рух візка, A/D - поворот, ПКМ + миша - огляд, колесо - наближення";

    string _message = "Тренажер монтажника СЕС: під'їдьте до зон цеху";
    float _messageTime;
    GUIStyle _style;

    void Awake()
    {
        _instance = this;
    }

    public static void Show(string message)
    {
        Debug.Log("[СЕС] " + message);
        if (_instance == null)
            return;
        _instance._message = message;
        _instance._messageTime = Time.time;
    }

    void OnGUI()
    {
        if (_style == null)
            _style = new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.UpperLeft, wordWrap = true };

        GUI.Box(new Rect(10, 10, 640, 34), controls, _style);
        Color previous = GUI.color;
        GUI.color = Time.time - _messageTime < 3f ? Color.yellow : Color.white;
        GUI.Box(new Rect(10, 50, 640, 60), _message, _style);
        GUI.color = previous;
    }
}
