using UnityEngine;

public class StatusHud : MonoBehaviour
{
    public static void Show(string message)
    {
        Debug.Log($"[СЕС] {System.DateTime.Now:HH:mm:ss.ff} {message}");
    }
}
