using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeToggleSwitch : MonoBehaviour
{
    public PokeInteractable interactable;
    public Transform lever;
    public bool isOn = true;
    public UnityEvent<bool> onValueChanged = new UnityEvent<bool>();

    void Start()
    {
        UpdateLever();
    }

    void OnEnable()
    {
        interactable.WhenStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        interactable.WhenStateChanged -= OnStateChanged;
    }

    void OnStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
            SetValue(!isOn);
    }

    public void SetValue(bool value)
    {
        isOn = value;
        UpdateLever();
        onValueChanged.Invoke(isOn);
    }

    void UpdateLever()
    {
        lever.localRotation = Quaternion.Euler(isOn ? -25f : 25f, 0f, 0f);
    }
}
