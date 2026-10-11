using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabletKeyboard : MonoBehaviour
{
    static readonly string[] Rows =
    {
        "йцукенгшщзхї",
        "фівапролджє",
        "ячсмитьбю'"
    };

    const float KeyWidth = 58f;
    const float KeyHeight = 50f;
    const float Gap = 6f;
    const float PanelHeight = 270f;

    static readonly Color PanelColor = new Color(0.08f, 0.09f, 0.12f, 0.98f);
    static readonly Color KeyColor = new Color(0.25f, 0.28f, 0.34f);
    static readonly Color ActionColor = new Color(0.25f, 0.6f, 1f);

    TMP_InputField _target;

    public static TabletKeyboard Attach(TMP_InputField target, RectTransform page)
    {
        var go = new GameObject("TabletKeyboard", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(page, false);
        var rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(0f, PanelHeight);
        rect.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = PanelColor;

        var keyboard = go.AddComponent<TabletKeyboard>();
        keyboard._target = target;
        keyboard.Build(page.rect.width);
        target.onSelect.AddListener(_ => keyboard.gameObject.SetActive(true));
        go.SetActive(false);
        return keyboard;
    }

    void Build(float width)
    {
        float y = 12f;
        for (int r = 0; r < Rows.Length; r++)
        {
            string row = Rows[r];
            float extra = r == Rows.Length - 1 ? 130f + Gap : 0f;
            float x = (width - (row.Length * (KeyWidth + Gap) - Gap + extra)) / 2f;
            foreach (char c in row)
            {
                char letter = c;
                AddKey(letter.ToString(), x, y, KeyWidth, KeyColor, () => Type(letter));
                x += KeyWidth + Gap;
            }
            if (r == Rows.Length - 1)
                AddKey("Стерти", x, y, 130f, KeyColor, Erase);
            y += KeyHeight + Gap;
        }

        float bottomWidth = 420f + Gap + 170f;
        float bx = (width - bottomWidth) / 2f;
        AddKey("Пробіл", bx, y, 420f, KeyColor, () => Append(" "));
        AddKey("Готово", bx + 420f + Gap, y, 170f, ActionColor, Hide);
    }

    void AddKey(string label, float x, float y, float w, Color color, UnityAction action)
    {
        var key = new GameObject("Key_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        key.transform.SetParent(transform, false);
        var rect = (RectTransform)key.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, KeyHeight);
        var image = key.GetComponent<Image>();
        image.color = color;
        var button = key.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        var text = new GameObject("Label", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(key.transform, false);
        var textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        text.font = TMP_Settings.defaultFontAsset;
        text.text = label.Length == 1 ? label.ToUpper() : label;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    void Type(char letter)
    {
        string text = _target.text;
        bool capital = text.Length == 0 || text.EndsWith(" ") || text.EndsWith(".");
        Append((capital ? char.ToUpper(letter) : letter).ToString());
    }

    void Append(string value)
    {
        if (_target.characterLimit > 0 && _target.text.Length + value.Length > _target.characterLimit)
            return;
        _target.text += value;
    }

    void Erase()
    {
        if (_target.text.Length > 0)
            _target.text = _target.text.Substring(0, _target.text.Length - 1);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
