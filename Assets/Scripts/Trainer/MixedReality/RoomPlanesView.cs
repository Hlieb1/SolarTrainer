using Meta.XR.MRUtilityKit;
using UnityEngine;

public class RoomPlanesView : MonoBehaviour
{
    public Material planeMaterial;

    public int PlaneCount { get; private set; }

    void Start()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(ShowPlanes);
    }

    void ShowPlanes()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
            return;

        foreach (var anchor in room.Anchors)
        {
            if (anchor.VolumeBounds.HasValue && anchor.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
                ShowVolume(anchor);

            if (!anchor.PlaneRect.HasValue)
                continue;

            Color color = ColorFor(anchor);
            if (color.a == 0f)
                continue;

            var rect = anchor.PlaneRect.Value;
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "Plane_" + anchor.Label;
            Destroy(quad.GetComponent<Collider>());
            quad.transform.SetParent(anchor.transform, false);
            quad.transform.localPosition = new Vector3(rect.center.x, rect.center.y, 0.005f);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            quad.transform.localScale = new Vector3(rect.width, rect.height, 1f);
            var renderer = quad.GetComponent<Renderer>();
            renderer.sharedMaterial = planeMaterial;
            renderer.material.color = color;
            PlaneCount++;
            Debug.Log($"[MR] Площина {anchor.Label}: {rect.width:0.00} x {rect.height:0.00} м");
        }
    }

    void ShowVolume(MRUKAnchor anchor)
    {
        var bounds = anchor.VolumeBounds.Value;
        var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = "Volume_" + anchor.Label;
        Destroy(box.GetComponent<Collider>());
        box.transform.SetParent(anchor.transform, false);
        box.transform.localPosition = bounds.center;
        box.transform.localScale = bounds.size;
        var renderer = box.GetComponent<Renderer>();
        renderer.sharedMaterial = planeMaterial;
        renderer.material.color = ColorFor(anchor);
        PlaneCount++;
        Debug.Log($"[MR] Об'єм {anchor.Label}: {bounds.size.x:0.00} x {bounds.size.y:0.00} x {bounds.size.z:0.00} м");
    }

    static Color ColorFor(MRUKAnchor anchor)
    {
        if (anchor.HasAnyLabel(MRUKAnchor.SceneLabels.FLOOR))
            return new Color(0.2f, 0.9f, 0.3f, 0.25f);
        if (anchor.HasAnyLabel(MRUKAnchor.SceneLabels.WALL_FACE))
            return new Color(0.2f, 0.5f, 1f, 0.2f);
        if (anchor.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE))
            return new Color(1f, 0.6f, 0.1f, 0.4f);
        return new Color(0f, 0f, 0f, 0f);
    }
}
