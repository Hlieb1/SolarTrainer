using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class InfoCardTrigger : MonoBehaviour
{
    public MonoBehaviour interactable;
    public GameObject card;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public float height = 0.35f;

    IInteractableView _view;
    Transform _head;
    bool _hovered;
    bool _pinned;

    void Awake()
    {
        _view = interactable as IInteractableView;
        card.SetActive(false);
    }

    void OnEnable()
    {
        _view.WhenStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        _view.WhenStateChanged -= OnStateChanged;
    }

    void Start()
    {
        var rig = FindAnyObjectByType<OVRCameraRig>();
        _head = rig != null ? rig.centerEyeAnchor : Camera.main.transform;
        Refresh();
    }

    void OnStateChanged(InteractableStateChangeArgs args)
    {
        if (card == null)
            return;
        if (args.NewState == InteractableState.Select)
            _pinned = !_pinned;
        _hovered = args.NewState == InteractableState.Hover || args.NewState == InteractableState.Select;
        Refresh();
        card.SetActive(_hovered || _pinned);
    }

    void Refresh()
    {
        var panel = GetComponent<SolarPanel>();
        if (panel == null)
            return;
        titleText.text = panel.isBroken ? "Панель: пошкоджена" : "Панель: справна";
        bodyText.text = $"{panel.model}\nПотужність: {panel.powerWatts} Вт\nСтан: {panel.Status}\n" +
                        (panel.isBroken ? "Замініть модуль, монтаж заборонено." : "Можна встановлювати на стенд.");
    }

    void LateUpdate()
    {
        if (card == null || !card.activeSelf || _head == null)
            return;
        card.transform.position = transform.position + Vector3.up * height;
        card.transform.rotation = Quaternion.LookRotation(card.transform.position - _head.position);
    }
}
