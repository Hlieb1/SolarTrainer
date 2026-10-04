using Oculus.Interaction;
using UnityEngine;

public class AssemblyPart : MonoBehaviour
{
    public string partId;
    public Grabbable grabbable;
    public Rigidbody body;
    public AssemblyStation station;

    public AssemblySocket CurrentSocket { get; set; }
    public bool IsHeld => grabbable.SelectingPointsCount > 0;

    Vector3 _homePosition;
    Quaternion _homeRotation;

    void Awake()
    {
        _homePosition = transform.position;
        _homeRotation = transform.rotation;
    }

    void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnPointerEvent;
    }

    void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && CurrentSocket != null)
            CurrentSocket.Detach();
        else if (evt.Type == PointerEventType.Unselect && !IsHeld)
            station.TryInstall(this);
    }

    public void ReturnHome()
    {
        if (CurrentSocket != null)
            CurrentSocket.Detach();
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.position = _homePosition;
        body.rotation = _homeRotation;
        transform.SetPositionAndRotation(_homePosition, _homeRotation);
    }
}
