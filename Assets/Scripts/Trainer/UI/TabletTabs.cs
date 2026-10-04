using UnityEngine;
using UnityEngine.UI;

public class TabletTabs : MonoBehaviour
{
    public Button[] tabs;
    public GameObject[] pages;
    public Color activeColor = new Color(0.25f, 0.6f, 1f);
    public Color inactiveColor = new Color(0.85f, 0.87f, 0.9f);

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            tabs[i].onClick.AddListener(() => Show(index));
        }
        Show(0);
    }

    public void Show(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
            tabs[i].image.color = i == index ? activeColor : inactiveColor;
        }
    }
}
