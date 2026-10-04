using UnityEngine;

public class InverterConsole : MonoBehaviour
{
    public PokeButton3D startButton;
    public PokeToggleSwitch dcSwitch;
    public PokeSlider powerLimit;
    public AssemblyStation junctionBox;
    public Renderer ledPower;
    public Renderer ledGrid;
    public Renderer ledFault;
    public Transform powerBar;
    public float barWidth = 0.22f;
    public Color onColor = new Color(0.2f, 1f, 0.3f);
    public Color offColor = new Color(0.08f, 0.15f, 0.08f);
    public Color faultColor = new Color(1f, 0.15f, 0.1f);
    public Color faultOffColor = new Color(0.3f, 0.05f, 0.05f);

    bool _running;
    bool _fault;

    void Start()
    {
        startButton.onPressed.AddListener(OnStartPressed);
        dcSwitch.onValueChanged.AddListener(OnDcSwitched);
        powerLimit.onValueChanged.AddListener(_ => Refresh());
        Refresh();
    }

    public bool IsRunning => _running;
    public bool HasFault => _fault;

    void OnStartPressed()
    {
        if (_running)
        {
            _running = false;
            StatusHud.Show("Інвертор зупинено");
        }
        else if (!dcSwitch.isOn)
        {
            _fault = true;
            StatusHud.Show("Пуск неможливий: DC-роз'єднувач розімкнено");
        }
        else if (!junctionBox.IsComplete)
        {
            _fault = true;
            StatusHud.Show("Пуск неможливий: клемну коробку не зібрано");
        }
        else
        {
            _running = true;
            _fault = false;
            StatusHud.Show("Інвертор запущено, генерація в мережу");
        }
        Refresh();
    }

    void OnDcSwitched(bool closed)
    {
        if (!closed && _running)
        {
            _running = false;
            StatusHud.Show("DC-роз'єднувач розімкнено: інвертор зупинено");
        }
        Refresh();
    }

    void Refresh()
    {
        ledPower.material.color = dcSwitch.isOn ? onColor : offColor;
        ledGrid.material.color = _running ? onColor : offColor;
        ledFault.material.color = _fault ? faultColor : faultOffColor;

        var scale = powerBar.localScale;
        scale.x = _running ? Mathf.Max(0.002f, barWidth * powerLimit.value) : 0.002f;
        powerBar.localScale = scale;
    }
}
