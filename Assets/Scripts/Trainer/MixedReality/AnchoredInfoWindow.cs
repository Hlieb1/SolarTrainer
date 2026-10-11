using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class AnchoredInfoWindow : MonoBehaviour
{
    const string UuidKey = "SolarTrainer.InfoWindowAnchor2";
    const float WorldLockTimeout = 3f;

    public AnchorPrefabSpawner tableKit;
    public Transform window;
    public TMP_Text statusText;
    public Vector3 offsetFromModel = new Vector3(0f, 0.5f, 0f);

    public bool Restored { get; private set; }
    public bool Saved { get; private set; }

    async void Start()
    {
        window.gameObject.SetActive(false);
        if (Guid.TryParse(PlayerPrefs.GetString(UuidKey, ""), out var uuid) && await Restore(uuid))
            return;

        while (this != null && tableKit.AnchorPrefabSpawnerObjects.Count == 0)
            await Task.Yield();

        float deadline = Time.time + WorldLockTimeout;
        while (this != null && !MRUK.Instance.IsWorldLockActive && Time.time < deadline)
            await Task.Yield();
        await Task.Delay(500);

        if (this != null)
            CreateAnchor();
    }

    async Task<bool> Restore(Guid uuid)
    {
        var unbound = new List<OVRSpatialAnchor.UnboundAnchor>();
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(new[] { uuid }, unbound);
        if (!result.Success || unbound.Count == 0)
        {
            Debug.Log("[MR] Збережений якір не знайдено, буде створено новий");
            return false;
        }

        var saved = unbound[0];
        if (!saved.Localized && !await saved.LocalizeAsync())
            return false;

        var anchor = new GameObject("InfoWindowAnchor").AddComponent<OVRSpatialAnchor>();
        saved.BindTo(anchor);
        Show(anchor.transform, $"Якір відновлено після перезапуску\n{uuid}");
        Restored = true;
        return true;
    }

    async void CreateAnchor()
    {
        Transform kit = tableKit.AnchorPrefabSpawnerObjects.Values.First().transform;
        Vector3 position = kit.TransformPoint(offsetFromModel);
        Vector3 head = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
        Vector3 facing = Vector3.ProjectOnPlane(position - head, Vector3.up);
        if (facing.sqrMagnitude < 0.001f)
            facing = Vector3.forward;

        var go = new GameObject("InfoWindowAnchor");
        go.transform.SetPositionAndRotation(position, Quaternion.LookRotation(facing));
        var anchor = go.AddComponent<OVRSpatialAnchor>();
        Show(go.transform, "Створення просторового якоря…");

        if (!await anchor.WhenCreatedAsync())
        {
            statusText.text = "Не вдалося створити якір";
            return;
        }

        var result = await anchor.SaveAnchorAsync();
        if (!result.Success)
        {
            statusText.text = "Не вдалося зберегти якір";
            return;
        }

        PlayerPrefs.SetString(UuidKey, anchor.Uuid.ToString());
        PlayerPrefs.Save();
        Saved = true;
        statusText.text = $"Якір створено і збережено\n{anchor.Uuid}";
        Debug.Log("[MR] Якір збережено: " + anchor.Uuid);
    }

    void Show(Transform anchor, string status)
    {
        window.SetParent(anchor, false);
        window.localPosition = Vector3.zero;
        window.localRotation = Quaternion.identity;
        window.gameObject.SetActive(true);
        statusText.text = status;
        Debug.Log("[MR] " + status.Replace('\n', ' '));
    }
}
