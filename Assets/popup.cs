using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject background;
    public GameObject closeButton;
    public GameObject popupPanel;

    private PopupData currentData;

    public void OpenPopup(PopupData data)
    {
        if (data == null)
        {
            Debug.LogError("PopupDataが設定されていません。");
            return;
        }

        currentData = data;

        PopupView view = popupPanel.GetComponent<PopupView>();

        if (view != null)
        {
            view.data = currentData;
            view.ApplyData();
        }

        background.SetActive(true);
        closeButton.SetActive(true);
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        background.SetActive(false);
        closeButton.SetActive(false);

        currentData = null;
    }
}
