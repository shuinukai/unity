using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// キーボードプレハブのルートに付ける。ひらがな入力専用のアプリ内キーボード
public class KeyboardController : MonoBehaviour
{
    [Header("UI要素")]
    public Button scrimButton;
    public GameObject keyboardPanel;
    public TMP_Text displayText;
    public GridLayoutGroup keyGrid;
    public Button keyPrefab;

    [Header("操作ボタン")]
    public Button backspaceButton;
    public Button clearButton;
    public Button submitButton;

    // 決定ボタンが押されたときに入力内容を通知する（今は未使用）
    public event Action<string> Submitted;

    private const int KeyColumnCount = 12;
    private const string DakutenKey = "゛";
    private const string HandakutenKey = "゜";
    private const string SmallKey = "小";
    private const string LongVowelKey = "ー";

    // 右から「あかさたな…」と並ぶ五十音表（12列×5行）。空文字は穴埋め用
    private static readonly string[] hiraganaKeys =
    {
        "゛", "ん", "わ", "ら", "や", "ま", "は", "な", "た", "さ", "か", "あ",
        "゜", "", "", "り", "", "み", "ひ", "に", "ち", "し", "き", "い",
        "ー", "", "", "る", "ゆ", "む", "ふ", "ぬ", "つ", "す", "く", "う",
        "小", "", "", "れ", "", "め", "へ", "ね", "て", "せ", "け", "え",
        "", "", "を", "ろ", "よ", "も", "ほ", "の", "と", "そ", "こ", "お"
    };

    private static readonly Dictionary<char, char> dakutenMap = new Dictionary<char, char>
    {
        {'か','が'},{'き','ぎ'},{'く','ぐ'},{'け','げ'},{'こ','ご'},
        {'さ','ざ'},{'し','じ'},{'す','ず'},{'せ','ぜ'},{'そ','ぞ'},
        {'た','だ'},{'ち','ぢ'},{'つ','づ'},{'て','で'},{'と','ど'},
        {'は','ば'},{'ひ','び'},{'ふ','ぶ'},{'へ','べ'},{'ほ','ぼ'},{'う','ゔ'}
    };

    private static readonly Dictionary<char, char> handakutenMap = new Dictionary<char, char>
    {
        {'は','ぱ'},{'ひ','ぴ'},{'ふ','ぷ'},{'へ','ぺ'},{'ほ','ぽ'}
    };

    private static readonly Dictionary<char, char> smallKanaMap = new Dictionary<char, char>
    {
        {'あ','ぁ'},{'い','ぃ'},{'う','ぅ'},{'え','ぇ'},{'お','ぉ'},
        {'や','ゃ'},{'ゆ','ゅ'},{'よ','ょ'},{'つ','っ'},{'わ','ゎ'}
    };

    private KeyboardSettings settings;
    private TMP_InputField target;
    private Button proxyButton;

    // ---------- 初期化 ----------

    public void Setup(KeyboardSettings settings)
    {
        this.settings = settings;

        // Scrimは常に全画面を覆う
        RectTransform scrimRect = scrimButton.GetComponent<RectTransform>();
        scrimRect.anchorMin = Vector2.zero;
        scrimRect.anchorMax = Vector2.one;
        scrimRect.offsetMin = Vector2.zero;
        scrimRect.offsetMax = Vector2.zero;

        ApplyGridSettings();
        CreateKeys();

        backspaceButton.onClick.AddListener(OnBackspacePressed);
        clearButton.onClick.AddListener(OnClearPressed);
        submitButton.onClick.AddListener(OnSubmitPressed);
        scrimButton.onClick.AddListener(Hide);

        Hide();
    }

    // 入力先のフォームを設定する。別のフォームで呼び直すと入力先が切り替わる
    public void Bind(TMP_InputField inputField)
    {
        if (target != null)
            target.onValueChanged.RemoveListener(UpdateDisplayText);

        if (proxyButton != null)
            Destroy(proxyButton.gameObject);

        target = inputField;
        target.onValueChanged.AddListener(UpdateDisplayText);

        proxyButton = CreateProxyButton(target);
        proxyButton.onClick.AddListener(Show);

        UpdateDisplayText(target.text);
    }

    private void ApplyGridSettings()
    {
        keyGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        keyGrid.constraintCount = KeyColumnCount;
        keyGrid.cellSize = settings.keySize;
        keyGrid.spacing = settings.keySpacing;
    }

    private void CreateKeys()
    {
        foreach (string kana in hiraganaKeys)
        {
            Button key = Instantiate(keyPrefab, keyGrid.transform);
            TMP_Text keyText = key.GetComponentInChildren<TMP_Text>();
            Image keyImage = key.GetComponent<Image>();

            if (string.IsNullOrEmpty(kana))
            {
                key.name = "Placeholder";
                key.interactable = false;

                if (keyImage != null)
                {
                    keyImage.enabled = false;
                    keyImage.raycastTarget = false;
                }

                if (keyText != null)
                    keyText.enabled = false;

                continue;
            }

            key.name = "Key_" + kana;

            if (keyText != null)
            {
                keyText.text = kana;
                keyText.fontSize = settings.keyFontSize;
            }

            if (keyImage != null && IsFunctionKey(kana))
                keyImage.color = settings.functionKeyColor;

            string capturedKana = kana;
            key.onClick.AddListener(() => OnKanaKeyPressed(capturedKana));
        }
    }

    // フォームの上に透明なボタンを重ねて、フォームにフォーカスが行かない（OSのキーボードが出ない）ようにする
    private static Button CreateProxyButton(TMP_InputField inputField)
    {
        GameObject proxy = new GameObject(
            "KeyboardProxy",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        RectTransform rect = proxy.GetComponent<RectTransform>();
        rect.SetParent(inputField.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.SetAsLastSibling();

        proxy.GetComponent<Image>().color = Color.clear;

        Button button = proxy.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        return button;
    }

    private static bool IsFunctionKey(string kana)
    {
        return kana == DakutenKey || kana == HandakutenKey || kana == SmallKey || kana == LongVowelKey;
    }

    // ---------- 表示 ----------

    public void Show()
    {
        SetVisible(true);

        if (target != null)
            UpdateDisplayText(target.text);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        scrimButton.gameObject.SetActive(visible);
        keyboardPanel.SetActive(visible);
    }

    private void UpdateDisplayText(string text)
    {
        if (displayText != null)
            displayText.text = text;
    }

    // ---------- 入力 ----------

    private void OnKanaKeyPressed(string kana)
    {
        if (target == null)
            return;

        switch (kana)
        {
            case DakutenKey:
                ReplaceLastChar(c => ToggleMark(dakutenMap, c));
                break;
            case HandakutenKey:
                ReplaceLastChar(c => ToggleMark(handakutenMap, c));
                break;
            case SmallKey:
                ReplaceLastChar(c => ToggleMark(smallKanaMap, c));
                break;
            default:
                if (target.characterLimit > 0 && target.text.Length >= target.characterLimit)
                    return;

                target.text += kana;
                break;
        }
    }

    private void OnBackspacePressed()
    {
        if (target == null || target.text.Length == 0)
            return;

        target.text = target.text.Substring(0, target.text.Length - 1);
    }

    private void OnClearPressed()
    {
        if (target == null)
            return;

        target.text = "";
    }

    private void OnSubmitPressed()
    {
        Hide();

        if (target != null)
            Submitted?.Invoke(target.text);
    }

    private void ReplaceLastChar(Func<char, char> convert)
    {
        string text = target.text;
        if (text.Length == 0)
            return;

        char last = text[text.Length - 1];
        char converted = convert(last);

        if (converted != last)
            target.text = text.Substring(0, text.Length - 1) + converted;
    }

    // 押すたびに「清音 ⇔ 濁音」のように切り替える。ば→ぱ のように別の記号が付いた文字にも対応
    private static char ToggleMark(Dictionary<char, char> map, char c)
    {
        char baseChar = ToBaseChar(c);

        if (!map.TryGetValue(baseChar, out char marked))
            return c;

        return c == marked ? baseChar : marked;
    }

    private static char ToBaseChar(char c)
    {
        foreach (Dictionary<char, char> map in new[] { dakutenMap, handakutenMap, smallKanaMap })
        {
            foreach (KeyValuePair<char, char> pair in map)
            {
                if (pair.Value == c)
                    return pair.Key;
            }
        }

        return c;
    }
}

[Serializable]
public class KeyboardSettings
{
    public Vector2 keySize = new Vector2(90f, 90f);
    public Vector2 keySpacing = new Vector2(6f, 6f);
    public float keyFontSize = 40f;
    public Color functionKeyColor = new Color(0.85f, 0.85f, 0.85f);
}
