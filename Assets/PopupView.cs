
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupView : MonoBehaviour
{
    public PopupData data;
    public TMP_Text[] textTargets;
    public Image[] imageTargets;

    public void ApplyData()
    {
        for (int i = 0; i < textTargets.Length; i++)
        {
            if (textTargets[i] != null && i < data.texts.Count)
            {
                textTargets[i].text = data.texts[i].text;
                textTargets[i].fontSize = data.texts[i].fontSize;
            }
        }

        for (int i = 0; i < imageTargets.Length; i++)
        {
            if (imageTargets[i] != null && i < data.images.Count)
            {
                imageTargets[i].sprite = data.images[i].image;
                imageTargets[i].gameObject.SetActive(
                    data.images[i].image != null
                );
            }
        }
    }
}
