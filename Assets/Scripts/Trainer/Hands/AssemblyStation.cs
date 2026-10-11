using System.Linq;
using UnityEngine;

public class AssemblyStation : MonoBehaviour
{
    public AssemblySocket[] sockets;
    public AssemblyPart[] parts;
    public Renderer housing;
    public float snapDistance = 0.08f;
    public Color housingColor = Color.gray;
    public Color completeColor = Color.green;

    public bool IsComplete => sockets.All(s => s.Occupant != null);

    void Update()
    {
        housing.material.color = IsComplete ? completeColor : housingColor;
    }

    public void TryInstall(AssemblyPart part)
    {
        foreach (var socket in sockets)
        {
            bool near = Vector3.Distance(part.transform.position, socket.snapPoint.position) < snapDistance;
            if (near && socket.partId == part.partId && socket.Occupant == null)
            {
                socket.Attach(part);
                return;
            }
        }
    }

    public void ResetAll()
    {
        foreach (var part in parts)
            part.ReturnHome();
    }
}
