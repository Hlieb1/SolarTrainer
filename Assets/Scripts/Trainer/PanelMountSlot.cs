using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PanelMountSlot : MonoBehaviour
{
    public Renderer marker;
    public Color freeColor = new Color(1f, 0.8f, 0.2f);
    public Color mountedColor = new Color(0.2f, 1f, 0.35f);
    public Color rejectColor = new Color(1f, 0.2f, 0.2f);

    BoxCollider _zone;
    float _rejectUntil;

    public MountablePanel Panel { get; private set; }

    void Awake()
    {
        _zone = GetComponent<BoxCollider>();
        _zone.isTrigger = true;
    }

    void Update()
    {
        if (Panel != null)
            marker.material.color = mountedColor;
        else
            marker.material.color = Time.time < _rejectUntil ? rejectColor : freeColor;
    }

    public bool Contains(Vector3 point)
    {
        Vector3 local = transform.InverseTransformPoint(point) - _zone.center;
        Vector3 half = _zone.size * 0.5f;
        return Mathf.Abs(local.x) <= half.x && Mathf.Abs(local.y) <= half.y && Mathf.Abs(local.z) <= half.z;
    }

    public bool TryMount(MountablePanel panel)
    {
        if (Panel != null)
            return false;

        if (panel.Info.isBroken)
        {
            _rejectUntil = Time.time + 1.5f;
            StatusHud.Show($"Панель {panel.Info.model} пошкоджена: монтаж заборонено");
            return false;
        }

        Panel = panel;
        panel.Slot = this;
        panel.Body.linearVelocity = Vector3.zero;
        panel.Body.angularVelocity = Vector3.zero;
        panel.Body.LockKinematic();
        panel.Body.position = transform.position;
        panel.Body.rotation = transform.rotation;
        panel.transform.SetPositionAndRotation(transform.position, transform.rotation);
        StatusHud.Show($"Панель {panel.Info.model} закріплено на стенді");
        return true;
    }

    public void Release()
    {
        if (Panel == null)
            return;
        Panel.Slot = null;
        Panel.Body.UnlockKinematic();
        Panel = null;
    }
}
