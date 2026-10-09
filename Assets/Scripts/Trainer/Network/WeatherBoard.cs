using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherBoard : MonoBehaviour
{
    [Serializable]
    class Current
    {
        public string time;
        public float temperature_2m;
        public float cloud_cover;
        public float shortwave_radiation;
        public float wind_speed_10m;
    }

    [Serializable]
    class Response
    {
        public Current current;
    }

    public string url = "https://api.open-meteo.com/v1/forecast?latitude=50.45&longitude=30.52&current=temperature_2m,cloud_cover,shortwave_radiation,wind_speed_10m";
    public float plantPowerKw = 5f;
    public TMP_Text dataText;
    public TMP_Text statusText;
    public Image statusLamp;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        SetStatus("Завантаження даних…", Color.yellow);
        StartCoroutine(NetClient.Get(url, OnData, OnError));
    }

    void OnData(string json)
    {
        var data = JsonUtility.FromJson<Response>(json);
        if (data == null || data.current == null)
        {
            OnError("невірний формат відповіді");
            return;
        }

        var c = data.current;
        float expectedKw = c.shortwave_radiation / 1000f * plantPowerKw * 0.8f;
        dataText.text = $"Київ, {c.time}\n" +
                        $"Температура: {c.temperature_2m:0.0} °C\n" +
                        $"Хмарність: {c.cloud_cover:0} %\n" +
                        $"Сонячна радіація: {c.shortwave_radiation:0} Вт/м²\n" +
                        $"Вітер: {c.wind_speed_10m:0.0} км/год\n" +
                        $"Очікувана генерація СЕС: {expectedKw:0.00} кВт";
        SetStatus($"Успішно оновлено о {DateTime.Now:HH:mm:ss}", Color.green);
    }

    void OnError(string error)
    {
        SetStatus("Помилка з'єднання: " + error, Color.red);
    }

    void SetStatus(string text, Color color)
    {
        statusText.text = text;
        statusText.color = color;
        statusLamp.color = color;
        Debug.Log("[Мережа] " + text);
    }
}
