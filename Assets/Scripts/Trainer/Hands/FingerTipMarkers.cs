using System.Linq;
using Oculus.Interaction.Input;
using UnityEngine;

public class FingerTipMarkers : MonoBehaviour
{
    public Transform leftMarker;
    public Transform rightMarker;

    SyntheticHand[] _hands;

    void Update()
    {
        if (_hands == null || _hands.Length == 0)
            _hands = FindObjectsByType<SyntheticHand>(FindObjectsInactive.Exclude);

        UpdateMarker(leftMarker, Handedness.Left);
        UpdateMarker(rightMarker, Handedness.Right);
    }

    void UpdateMarker(Transform marker, Handedness handedness)
    {
        var hand = _hands.FirstOrDefault(h => h.Handedness == handedness && h.IsTrackedDataValid);
        Pose tip = default;
        bool tracked = hand != null && hand.GetJointPose(HandJointId.HandIndexTip, out tip);
        marker.gameObject.SetActive(tracked);
        if (!tracked)
            return;

        marker.position = tip.position;
        marker.GetComponent<Renderer>().material.color = hand.GetIndexFingerIsPinching() ? Color.yellow : Color.cyan;
    }
}
