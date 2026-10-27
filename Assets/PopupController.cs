using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [Header("プレハブ")]
    public PopupUIRoot uiRootPrefab;
    public PopupButton buttonPrefab;

    [Header("データ（id順に並べ替えて表示）")]
    public List<PopupData> popupDataList = new List<PopupData>();

    [Header("ボタン一覧の設定")]
    public ButtonListSettings buttonListSettings = new ButtonListSettings();

    [Header("ポップアップの設定")]
    public PopupSettings popupSettings = new PopupSettings();

    private PopupUIRoot uiRoot;
    private PopupData currentData;

    private void Awake()
    {
        if (uiRootPrefab == null || buttonPrefab == null)
        {
            Debug.LogError("PopupControllerにプレハブが設定されていません。");
            return;
        }

        EnsureEventSystem();

        uiRoot = Instantiate(uiRootPrefab);
        uiRoot.closeButton.onClick.AddListener(ClosePopup);

        if (popupSettings.closeOnBackgroundClick)
            RegisterBackgroundClose();

        ApplyButtonListSettings();
        CreateButtons();

        ClosePopup();
    }

    // ---------- 生成 ----------

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(InputSystemUIInputModule)
        );
        eventSystem.transform.SetParent(transform);
    }

    private void ApplyButtonListSettings()
    {
        RectTransform container = uiRoot.ButtonContainer;

        VerticalLayoutGroup layout = GetOrAddComponent<VerticalLayoutGroup>(container.gameObject);
        layout.spacing = buttonListSettings.spacing;
        layout.padding = buttonListSettings.padding;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = buttonListSettings.stretchWidth;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = GetOrAddComponent<ContentSizeFitter>(container.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateButtons()
    {
        RectTransform container = uiRoot.ButtonContainer;

        // 同じidのデータはリストの上にあるものから順に並ぶ
        IEnumerable<PopupData> sortedData = popupDataList
            .Where(data => data != null)
            .OrderBy(data => data.id);

        foreach (PopupData data in sortedData)
        {
            PopupButton button = Instantiate(buttonPrefab, container);
            button.name = $"PopupButton_{data.id}";
            button.Setup(this, data, buttonListSettings.labelStyle);

            LayoutElement element = GetOrAddComponent<LayoutElement>(button.gameObject);
            element.preferredWidth = buttonListSettings.buttonSize.x;
            element.preferredHeight = buttonListSettings.buttonSize.y;
        }
    }

    private void RegisterBackgroundClose()
    {
        Image image = uiRoot.background.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning("backgroundにImageがないため、背景クリックで閉じる機能は無効です。");
            return;
        }

        image.raycastTarget = true;

        Button backgroundButton = GetOrAddComponent<Button>(uiRoot.background);
        backgroundButton.transition = Selectable.Transition.None;
        backgroundButton.onClick.AddListener(ClosePopup);
    }

    private static T GetOrAddComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
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

        uiRoot.popupView.data = currentData;
        uiRoot.popupView.ApplyData();

        SetPopupActive(true);
    }

    public void ClosePopup()
    {
        SetPopupActive(false);

        currentData = null;
    }

    private void SetPopupActive(bool active)
    {
        uiRoot.background.SetActive(active);
        uiRoot.closeButton.gameObject.SetActive(active);
        uiRoot.popupView.gameObject.SetActive(active);
    }
}

[Serializable]
public class ButtonListSettings
{
    public float spacing = 10f;
    public Vector2 buttonSize = new Vector2(200f, 50f);
    [Tooltip("ONにするとボタンの横幅をスクロール幅いっぱいに広げる（buttonSize.xは無視）")]
    public bool stretchWidth = false;
    public RectOffset padding = new RectOffset(10, 10, 10, 10);
    public ButtonLabelStyle labelStyle = new ButtonLabelStyle();
}

[Serializable]
public class ButtonLabelStyle
{
    [Tooltip("未設定ならプレハブのフォントのまま")]
    public TMP_FontAsset font;

    [Tooltip("ONにするとボタンの大きさに合わせて文字サイズを自動調整する")]
    public bool autoSize = false;

    [Tooltip("autoSizeがOFFのときの文字サイズ")]
    public float fontSize = 24f;

    [Tooltip("autoSizeがONのときの最小・最大サイズ")]
    public float fontSizeMin = 10f;
    public float fontSizeMax = 36f;
}

[Serializable]
public class PopupSettings
{
    public bool closeOnBackgroundClick = true;
}
