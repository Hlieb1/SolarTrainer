using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public string model = "SP-60 (навчальний модуль)";
    public int powerWatts = 60;
    public bool isBroken;

    public string Status => isBroken ? "Пошкоджена: тріщини на склі" : "Справна";
}
