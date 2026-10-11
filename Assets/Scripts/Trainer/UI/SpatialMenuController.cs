using UnityEngine;
using UnityEngine.UI;

public class SpatialMenuController : MonoBehaviour
{
    public GameObject panel;
    public Button closeButton;
    public float distance = 0.55f;
    public float heightOffset = -0.12f;
    public bool openOnStart;

    const float ToggleCooldown = 0.3f;

    Transform _head;
    float _lastToggle = -1f;

    void Start()
    {
        var rig = FindAnyObjectByType<OVRCameraRig>();
        _head = rig != null ? rig.centerEyeAnchor : Camera.main.transform;

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        panel.SetActive(false);
        if (openOnStart)
            Open();
    }

    void Update()
    {
        if (Time.unscaledTime - _lastToggle < ToggleCooldown)
            return;

        bool controllerMenu = OVRInput.GetDown(OVRInput.Button.Start) || OVRInput.GetDown(OVRInput.RawButton.Start);
        bool keyboard = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.M);
        if (controllerMenu || keyboard)
        {
            _lastToggle = Time.unscaledTime;
            Toggle();
            Debug.Log($"[SpatialMenu] {(panel.activeSelf ? "Opened" : "Closed")} by {(controllerMenu ? "controller menu button" : "keyboard")}");
        }
    }

    public void Toggle()
    {
        if (panel.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        Vector3 forward = Vector3.ProjectOnPlane(_head.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        forward.Normalize();

        Vector3 position = _head.position + forward * distance + Vector3.up * heightOffset;
        panel.transform.SetPositionAndRotation(position, Quaternion.LookRotation(position - _head.position));
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
