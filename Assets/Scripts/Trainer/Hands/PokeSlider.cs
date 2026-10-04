using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeSlider : MonoBehaviour
{
    public PokeInteractable interactable;
    public Transform knob;
    public float length = 0.24f;
    [Range(0f, 1f)] public float value = 0.6f;
    public UnityEvent<float> onValueChanged = new UnityEvent<float>();

    bool _pressed;

    void Start()
    {
        UpdateKnob();
    }

    void OnEnable()
    {
        interactable.WhenPointerEventRaised += OnPointerEvent;
    }

    void OnDisable()
    {
        interactable.WhenPointerEventRaised -= OnPointerEvent;
    }

    void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
            _pressed = true;
        else if (evt.Type == PointerEventType.Unselect || evt.Type == PointerEventType.Cancel)
            _pressed = false;

        if (_pressed && (evt.Type == PointerEventType.Select || evt.Type == PointerEventType.Move))
        {
            float x = transform.InverseTransformPoint(evt.Pose.position).x;
            value = Mathf.Clamp01(x / length + 0.5f);
            UpdateKnob();
            onValueChanged.Invoke(value);
        }
    }

    void UpdateKnob()
    {
        var position = knob.localPosition;
        position.x = (value - 0.5f) * length;
        knob.localPosition = position;
    }
}
