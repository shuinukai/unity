using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PopupButton : MonoBehaviour
{
    public TMP_Text label;

    private PopupController controller;
    private PopupData data;

    // フォントや文字サイズはプレハブ側で設定する。ここではタイトルとクリック処理だけを設定する
    public void Setup(PopupController controller, PopupData data)
    {
        this.controller = controller;
        this.data = data;

        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (label != null)
            label.text = data.title;

        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Open);
    }

    public void Open()
    {
        controller.OpenPopup(data);
    }
}
