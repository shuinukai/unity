using UnityEngine;
using UnityEngine.UI;

// UIRootプレハブのルート（Canvas）に付けて、中の部品の参照をまとめておく
public class PopupUIRoot : MonoBehaviour
{
    [Header("ボタン一覧")]
    public ScrollRect scrollRect;

    [Header("ポップアップ")]
    public GameObject background;
    public Button closeButton;
    public PopupView popupView;

    public RectTransform ButtonContainer => scrollRect.content;
}
