using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 見た目・配置はHierarchy側で設定し、ここではボタンの生成とポップアップの開閉だけを行う
public class PopupController : MonoBehaviour
{
    [Header("ボタン一覧")]
    [Tooltip("ボタンを並べる親（Scroll View の Content）")]
    public RectTransform buttonContainer;
    public PopupButton buttonPrefab;

    [Header("データ（id順に並べ替えて表示）")]
    public List<PopupData> popupDataList = new List<PopupData>();

    [Header("ポップアップ")]
    public GameObject background;
    public Button closeButton;
    public PopupView popupView;
    public bool closeOnBackgroundClick = true;

    private PopupData currentData;

    private void Awake()
    {
        if (buttonContainer == null || buttonPrefab == null ||
            background == null || closeButton == null || popupView == null)
        {
            Debug.LogError("PopupControllerの参照が設定されていません。インスペクタを確認してください。");
            return;
        }

        closeButton.onClick.AddListener(ClosePopup);

        if (closeOnBackgroundClick)
            RegisterBackgroundClose();

        CreateButtons();

        ClosePopup();
    }

    // ---------- 生成 ----------

    private void CreateButtons()
    {
        // 同じidのデータはリストの上にあるものから順に並ぶ
        IEnumerable<PopupData> sortedData = popupDataList
            .Where(data => data != null)
            .OrderBy(data => data.id);

        foreach (PopupData data in sortedData)
        {
            PopupButton button = Instantiate(buttonPrefab, buttonContainer);
            button.name = $"PopupButton_{data.id}";
            button.Setup(this, data);
        }
    }

    private void RegisterBackgroundClose()
    {
        Image image = background.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning("backgroundにImageがないため、背景クリックで閉じる機能は無効です。");
            return;
        }

        image.raycastTarget = true;

        Button backgroundButton = background.GetComponent<Button>();
        if (backgroundButton == null)
            backgroundButton = background.AddComponent<Button>();

        backgroundButton.transition = Selectable.Transition.None;
        backgroundButton.onClick.AddListener(ClosePopup);
    }

    // ---------- ポップアップ ----------

    public void OpenPopup(PopupData data)
    {
        if (data == null)
        {
            Debug.LogError("PopupDataが設定されていません。");
            return;
        }

        currentData = data;

        popupView.data = currentData;
        popupView.ApplyData();

        SetPopupActive(true);
    }

    public void ClosePopup()
    {
        SetPopupActive(false);

        currentData = null;
    }

    private void SetPopupActive(bool active)
    {
        background.SetActive(active);
        closeButton.gameObject.SetActive(active);
        popupView.gameObject.SetActive(active);
    }
}
