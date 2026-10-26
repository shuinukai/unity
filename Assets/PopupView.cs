using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupView : MonoBehaviour
{
    public PopupData data;

    public RectTransform textContainer;
    public RectTransform imageContainer;

    public TMP_Text textPrefab;
    public Image imagePrefab;

    public void ApplyData()
    {
        if (data == null)
        {
            Debug.LogError("PopupDataが設定されていません。");
            return;
        }

        ClearChildren(textContainer);
        ClearChildren(imageContainer);

        foreach (TextItem item in data.texts)
        {
            TMP_Text newText = Instantiate(
                textPrefab,
                textContainer
            );

            newText.text = item.text;
            newText.fontSize = item.fontSize;

            RectTransform rect =
                newText.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = item.position;
            rect.sizeDelta = item.size;
        }

        foreach (ImageItem item in data.images)
        {
            if (item.image == null)
                continue;

            Image newImage = Instantiate(
                imagePrefab,
                imageContainer
            );

            newImage.sprite = item.image;

            RectTransform rect =
                newImage.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = item.position;
            rect.sizeDelta = item.size;

            newImage.preserveAspect = true;
        }
    }

    private void ClearChildren(RectTransform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}
