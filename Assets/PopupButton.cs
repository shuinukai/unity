using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PopupButton : MonoBehaviour
{
    public TMP_Text label;

    private PopupController controller;
    private PopupData data;

    public void Setup(PopupController controller, PopupData data, ButtonLabelStyle labelStyle)
    {
        this.controller = controller;
        this.data = data;

        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (label != null)
        {
            label.text = data.title;
            ApplyLabelStyle(labelStyle);
        }

        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Open);
    }

    private void ApplyLabelStyle(ButtonLabelStyle style)
    {
        if (style == null)
            return;

        if (style.font != null)
            label.font = style.font;

        label.enableAutoSizing = style.autoSize;

        if (style.autoSize)
        {
            label.fontSizeMin = style.fontSizeMin;
            label.fontSizeMax = style.fontSizeMax;
        }
        else
        {
            label.fontSize = style.fontSize;
        }
    }

    public void Open()
    {
        controller.OpenPopup(data);
    }
}
