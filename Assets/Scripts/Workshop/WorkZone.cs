using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorkZone : MonoBehaviour
{
    public string title = "Робоча зона";
    [TextArea] public string hint = "";
    public Renderer marker;
    public Color idleColor = new Color(1f, 0.8f, 0.2f, 0.25f);
    public Color activeColor = new Color(0.2f, 1f, 0.4f, 0.4f);

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        SetColor(idleColor);
    }

    public void Enter()
    {
        SetColor(activeColor);
    }

    public void Exit()
    {
        SetColor(idleColor);
    }

    void SetColor(Color color)
    {
        if (marker != null)
            marker.material.color = color;
    }
}
