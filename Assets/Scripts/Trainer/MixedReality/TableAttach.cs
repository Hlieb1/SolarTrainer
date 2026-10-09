using System;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class TableAttach : MonoBehaviour
{
    public Transform target;

    public MRUKAnchor Table { get; private set; }
    public event Action Attached;

    void Start()
    {
        target.gameObject.SetActive(false);
        MRUK.Instance.RegisterSceneLoadedCallback(Attach);
    }

    void Attach()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
            return;

        foreach (var anchor in room.Anchors)
        {
            if (anchor.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
            {
                Table = anchor;
                break;
            }
        }

        if (Table == null)
        {
            Debug.Log("[MR] Стіл у кімнаті не знайдено");
            return;
        }

        Vector3 forward = Vector3.ProjectOnPlane(Table.transform.up, Vector3.up);
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;
        target.SetPositionAndRotation(Table.transform.position, Quaternion.LookRotation(forward, Vector3.up));
        target.gameObject.SetActive(true);
        Debug.Log($"[MR] Модель встановлено на стіл: {Table.transform.position}");
        Attached?.Invoke();
    }
}
