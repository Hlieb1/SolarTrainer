using Oculus.Interaction;
using UnityEngine;

public class AssemblySocket : MonoBehaviour
{
    public string partId;
    public Transform snapPoint;
    public Renderer indicator;
    public Color emptyColor = Color.gray;
    public Color lockedColor = Color.green;

    public AssemblyPart Occupant { get; private set; }

    void Start()
    {
        indicator.material.color = emptyColor;
    }

    public void Attach(AssemblyPart part)
    {
        Occupant = part;
        part.CurrentSocket = this;
        part.body.linearVelocity = Vector3.zero;
        part.body.angularVelocity = Vector3.zero;
        part.body.LockKinematic();
        part.body.position = snapPoint.position;
        part.body.rotation = snapPoint.rotation;
        part.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
        indicator.material.color = lockedColor;
    }

    public void Detach()
    {
        if (Occupant == null)
            return;
        Occupant.CurrentSocket = null;
        Occupant.body.UnlockKinematic();
        Occupant = null;
        indicator.material.color = emptyColor;
    }
}
