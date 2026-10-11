using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeButton3D : MonoBehaviour
{
    public PokeInteractable interactable;
    public UnityEvent onPressed = new UnityEvent();

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
            onPressed.Invoke();
    }
}
