using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InverterCheckForm : MonoBehaviour
{
    [Serializable]
    class FormData
    {
        public bool dcIsolatorOpen;
        public bool groundingChecked;
        public float dcVoltage = 380f;
        public float temperature = 35f;
        public string inspector = "";
        public string savedAt = "";
    }

    const string SaveKey = "SolarTrainer.InverterCheck";
    const float MaxDcVoltage = 600f;
    const float MaxTemperature = 60f;

    public Toggle dcIsolatorToggle;
    public Toggle groundingToggle;
    public Slider voltageSlider;
    public Slider temperatureSlider;
    public TMP_Text voltageValue;
    public TMP_Text temperatureValue;
    public TMP_InputField inspectorInput;
    public Button saveButton;
    public Button resetButton;
    public TMP_Text statusText;

    void Awake()
    {
        voltageSlider.onValueChanged.AddListener(v => voltageValue.text = $"{v:0} В");
        temperatureSlider.onValueChanged.AddListener(v => temperatureValue.text = $"{v:0} °C");
        saveButton.onClick.AddListener(Save);
        resetButton.onClick.AddListener(ResetValues);
    }

    void Start()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json))
        {
            Apply(new FormData());
            statusText.text = "Заповніть протокол перевірки інвертора та натисніть «Зберегти».";
            return;
        }

        var data = JsonUtility.FromJson<FormData>(json);
        Apply(data);
        statusText.text = $"Завантажено протокол від {data.savedAt} ({data.inspector}).";
    }

    void Apply(FormData data)
    {
        dcIsolatorToggle.isOn = data.dcIsolatorOpen;
        groundingToggle.isOn = data.groundingChecked;
        voltageSlider.value = data.dcVoltage;
        temperatureSlider.value = data.temperature;
        inspectorInput.text = data.inspector;
        voltageValue.text = $"{data.dcVoltage:0} В";
        temperatureValue.text = $"{data.temperature:0} °C";
    }

    public void Save()
    {
        string inspector = inspectorInput.text.Trim();
        if (inspector.Length == 0)
        {
            statusText.text = "<color=#FF6B6B>Вкажіть ПІБ монтажника.</color>";
            return;
        }
        if (!dcIsolatorToggle.isOn || !groundingToggle.isOn)
        {
            statusText.text = "<color=#FF6B6B>Спочатку розімкніть DC-роз'єднувач і перевірте заземлення.</color>";
            return;
        }

        var data = new FormData
        {
            dcIsolatorOpen = true,
            groundingChecked = true,
            dcVoltage = voltageSlider.value,
            temperature = temperatureSlider.value,
            inspector = inspector,
            savedAt = DateTime.Now.ToString("HH:mm:ss")
        };
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();

        bool ok = data.dcVoltage <= MaxDcVoltage && data.temperature <= MaxTemperature;
        statusText.text = $"Збережено о {data.savedAt}: {inspector}, DC {data.dcVoltage:0} В, {data.temperature:0} °C. " +
                          (ok ? "<color=#7CFC9A>Інвертор у нормі.</color>" : "<color=#FFB060>Параметри поза нормою: потрібна діагностика.</color>");
    }

    public void ResetValues()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Apply(new FormData());
        statusText.text = "Протокол скинуто до початкових значень.";
    }
}
