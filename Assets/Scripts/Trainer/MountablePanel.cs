using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(SolarPanel))]
public class MountablePanel : MonoBehaviour
{
    public Grabbable grabbable;
    public Rigidbody body;

    public SolarPanel Info { get; private set; }
    public PanelMountSlot Slot { get; set; }
    public Rigidbody Body => body;

    void Awake()
    {
        Info = GetComponent<SolarPanel>();
    }

    void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnPointer;
    }

    void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnPointer;
    }

    void OnPointer(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && Slot != null)
            Slot.Release();

        if (evt.Type != PointerEventType.Unselect || grabbable.SelectingPointsCount > 0)
            return;

        foreach (var slot in FindObjectsByType<PanelMountSlot>(FindObjectsInactive.Exclude))
        {
            if (slot.Contains(transform.position))
            {
                slot.TryMount(this);
                return;
            }
        }
    }
}
