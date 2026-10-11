using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MixedRealityMode : MonoBehaviour
{
    public GameObject[] vrObjects;
    public GameObject[] mrObjects;
    public OVRPassthroughLayer passthrough;
    public EffectMesh[] roomMeshes;
    public AnchorPrefabSpawner tableKit;
    public Button toggleButton;
    public TMP_Text toggleLabel;

    public bool IsMixedReality { get; private set; }

    Transform _trackingSpace;
    Vector3 _trackingPosition;
    Quaternion _trackingRotation;
    bool[] _vrWasActive;

    void Start()
    {
        _trackingSpace = FindAnyObjectByType<OVRCameraRig>().trackingSpace;
        _trackingPosition = _trackingSpace.localPosition;
        _trackingRotation = _trackingSpace.localRotation;
        toggleButton.onClick.AddListener(Toggle);
        _vrWasActive = new bool[vrObjects.Length];
        MRUK.Instance.EnableWorldLock = false;
        passthrough.enabled = false;
        toggleLabel.text = "ЛР6";
    }

    public void Toggle()
    {
        Apply(!IsMixedReality);
    }

    void Apply(bool mixedReality)
    {
        IsMixedReality = mixedReality;
        MRUK.Instance.EnableWorldLock = mixedReality;
        if (!mixedReality)
            _trackingSpace.SetLocalPositionAndRotation(_trackingPosition, _trackingRotation);

        for (int i = 0; i < vrObjects.Length; i++)
        {
            if (mixedReality)
                _vrWasActive[i] = vrObjects[i].activeSelf;
            vrObjects[i].SetActive(!mixedReality && _vrWasActive[i]);
        }
        foreach (var go in mrObjects)
            go.SetActive(mixedReality);
        passthrough.enabled = mixedReality;
        foreach (var mesh in roomMeshes)
            mesh.HideMesh = !mixedReality;
        foreach (var kit in tableKit.AnchorPrefabSpawnerObjects.Values)
            kit.SetActive(mixedReality);

        toggleLabel.text = mixedReality ? "VR" : "ЛР6";
        Debug.Log(mixedReality ? "[MR] Режим змішаної реальності (ЛР6)" : "[MR] Режим VR (ЛР2–5)");
    }
}
