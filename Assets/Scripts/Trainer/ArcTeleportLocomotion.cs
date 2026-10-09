using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Locomotion;
using UnityEngine;

public class ArcTeleportLocomotion : MonoBehaviour
{
    enum State { Idle, Aiming }

    const int TeleportEventId = 1001;

    [SerializeField] float pressThreshold = 0.5f;
    [SerializeField] float releaseThreshold = 0.25f;
    [SerializeField] float arcSpeed = 7f;
    [SerializeField] float arcStep = 0.03f;
    [SerializeField] int arcSegments = 80;
    [SerializeField] float markerRadius = 0.18f;
    [SerializeField] Color validColor = new Color(0.1f, 0.75f, 1f);
    [SerializeField] Color invalidColor = new Color(0.9f, 0.25f, 0.25f);

    OVRCameraRig _rig;
    FirstPersonLocomotor _locomotor;
    TeleportInteractable[] _surfaces;
    LineRenderer _arc;
    LineRenderer _ring;
    readonly List<Vector3> _arcPoints = new List<Vector3>();

    State _state = State.Idle;
    Vector3? _destination;
    Vector3 _aimHeadStart;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        var rig = FindAnyObjectByType<OVRCameraRig>();
        if (rig == null)
            return;

        ConfigureInteractionRig();
        new GameObject(nameof(ArcTeleportLocomotion)).AddComponent<ArcTeleportLocomotion>()._rig = rig;
    }

    static void ConfigureInteractionRig()
    {
        Transform root = null;
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include))
        {
            if (t.name.Contains("OVRComprehensiveInteractionRig"))
            {
                root = t;
                break;
            }
        }
        if (root == null)
            return;

        foreach (var c in root.GetComponentsInChildren<SlideLocomotionBroadcaster>(true))
            c.gameObject.SetActive(false);
        foreach (var c in root.GetComponentsInChildren<StepLocomotionBroadcaster>(true))
            c.gameObject.SetActive(false);
        foreach (var c in root.GetComponentsInChildren<TunnelingEffect>(true))
            c.gameObject.SetActive(false);
        foreach (var c in root.GetComponentsInChildren<TeleportInteractor>(true))
        {
            if (c.name == "TeleportControllerInteractor")
                c.gameObject.SetActive(false);
        }
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == "ControllerTurnerInteractor")
                t.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        _locomotor = FindAnyObjectByType<FirstPersonLocomotor>();
        _surfaces = FindObjectsByType<TeleportInteractable>(FindObjectsInactive.Exclude);
        _arc = CreateLine("Arc", 0.015f, false);
        _ring = CreateLine("TargetRing", 0.02f, true);
        _ring.positionCount = 32;
        SetVisible(false);
    }

    LineRenderer CreateLine(string name, float width, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var line = go.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        line.widthMultiplier = width;
        line.loop = loop;
        line.useWorldSpace = true;
        return line;
    }

    void Update()
    {
        float stickY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;

        switch (_state)
        {
            case State.Idle:
                if (stickY > pressThreshold)
                {
                    _state = State.Aiming;
                    _aimHeadStart = _rig.centerEyeAnchor.position;
                }
                break;

            case State.Aiming:
                HoldPlayerInPlace();
                if (stickY < releaseThreshold)
                {
                    _state = State.Idle;
                    SetVisible(false);
                    if (_destination.HasValue)
                        MovePlayer(_destination.Value);
                    break;
                }
                Aim(_rig.rightControllerAnchor);
                break;
        }
    }

    void HoldPlayerInPlace()
    {
        if (_locomotor == null)
            return;

        Vector3 drift = _rig.centerEyeAnchor.position - _aimHeadStart;
        drift.y = 0f;
        if (drift.sqrMagnitude < 0.000001f)
            return;

        _locomotor.HandleLocomotionEvent(new LocomotionEvent(TeleportEventId, new Pose(-drift, Quaternion.identity),
            LocomotionEvent.TranslationType.Relative, LocomotionEvent.RotationType.None));
    }

    void Aim(Transform hand)
    {
        BuildArc(hand.position, hand.forward * arcSpeed, _arcPoints);
        _destination = null;

        for (int i = 1; i < _arcPoints.Count; i++)
        {
            Vector3? hit = FindTarget(_arcPoints[i - 1], _arcPoints[i]);
            if (hit.HasValue)
            {
                _destination = hit;
                _arcPoints.RemoveRange(i, _arcPoints.Count - i);
                _arcPoints.Add(hit.Value);
                break;
            }
        }

        Color color = _destination.HasValue ? validColor : invalidColor;
        _arc.material.SetColor("_BaseColor", color);
        _arc.positionCount = _arcPoints.Count;
        _arc.SetPositions(_arcPoints.ToArray());
        _arc.enabled = true;

        _ring.enabled = _destination.HasValue;
        if (_destination.HasValue)
            DrawRing(_destination.Value);
    }

    void BuildArc(Vector3 start, Vector3 velocity, List<Vector3> points)
    {
        points.Clear();
        points.Add(start);
        Vector3 p = start;
        for (int i = 0; i < arcSegments && p.y > -5f; i++)
        {
            p += velocity * arcStep;
            velocity += Physics.gravity * arcStep;
            points.Add(p);
        }
    }

    Vector3? FindTarget(Vector3 from, Vector3 to)
    {
        Vector3? result = null;
        float nearest = float.MaxValue;

        foreach (var surface in _surfaces)
        {
            if (surface == null || !surface.isActiveAndEnabled || !surface.AllowTeleport)
                continue;
            if (!surface.DetectHit(from, to, out TeleportHit hit))
                continue;

            float d = Vector3.Distance(from, hit.Point);
            if (d < nearest)
            {
                nearest = d;
                result = surface.TargetPose(new Pose(hit.Point, Quaternion.identity)).position;
            }
        }
        return result;
    }

    void DrawRing(Vector3 center)
    {
        _ring.material.SetColor("_BaseColor", validColor);
        for (int i = 0; i < _ring.positionCount; i++)
        {
            float a = i * Mathf.PI * 2f / _ring.positionCount;
            _ring.SetPosition(i, center + new Vector3(Mathf.Cos(a) * markerRadius, 0.01f, Mathf.Sin(a) * markerRadius));
        }
    }

    void MovePlayer(Vector3 feet)
    {
        if (_locomotor == null)
            return;

        var target = new Pose(feet, _rig.transform.rotation);
        _locomotor.HandleLocomotionEvent(new LocomotionEvent(TeleportEventId, target,
            LocomotionEvent.TranslationType.Absolute, LocomotionEvent.RotationType.None));
    }

    void SetVisible(bool visible)
    {
        _arc.enabled = visible;
        _ring.enabled = visible;
    }
}
