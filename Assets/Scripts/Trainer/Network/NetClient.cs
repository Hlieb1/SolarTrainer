using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public static class NetClient
{
    const int TimeoutSeconds = 10;

    public static IEnumerator Get(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (var request = UnityWebRequest.Get(url))
        {
            request.timeout = TimeoutSeconds;
            yield return request.SendWebRequest();
            Complete(request, onSuccess, onError);
        }
    }

    public static IEnumerator PostJson(string url, string json, Action<string> onSuccess, Action<string> onError)
    {
        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = TimeoutSeconds;
            yield return request.SendWebRequest();
            Complete(request, onSuccess, onError);
        }
    }

    static void Complete(UnityWebRequest request, Action<string> onSuccess, Action<string> onError)
    {
        if (request.result == UnityWebRequest.Result.Success)
            onSuccess(request.downloadHandler.text);
        else
            onError(request.error);
    }
}
