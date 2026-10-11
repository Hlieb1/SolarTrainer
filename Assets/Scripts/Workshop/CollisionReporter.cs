using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    const float MinImpactSpeed = 0.2f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < MinImpactSpeed || collision.GetContact(0).normal.y > 0.7f)
            return;

        var panel = collision.gameObject.GetComponentInParent<SolarPanel>();
        if (panel != null)
            StatusHud.Show($"Зіткнення з панеллю {panel.model}. Стан: {panel.Status}");
        else
            StatusHud.Show($"Зіткнення з перешкодою: {collision.gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        var zone = other.GetComponent<WorkZone>();
        if (zone == null)
            return;

        zone.Enter();
        StatusHud.Show($"Увійшли в зону «{zone.title}». {zone.hint}");
    }

    void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<WorkZone>();
        if (zone == null)
            return;

        zone.Exit();
        StatusHud.Show($"Вийшли із зони «{zone.title}»");
    }
}
