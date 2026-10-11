using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public static class DesktopMode
{
    const string DesktopScene = "Lab1_Workshop";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void DisableXrInDesktopScene()
    {
        if (SceneManager.GetActiveScene().name != DesktopScene)
            return;

        var settings = XRGeneralSettings.Instance;
        if (settings == null || settings.Manager == null || settings.Manager.activeLoader == null)
            return;

        settings.Manager.StopSubsystems();
        settings.Manager.DeinitializeLoader();
    }
}
