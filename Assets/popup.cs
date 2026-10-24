
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject background;
    public GameObject closeButton;
    private GameObject currentpopup;

    public void OpenPopup(GameObject popup)
    {
        currentpopup=popup;
        background.SetActive(true);
        closeButton.SetActive(true);
        currentpopup.SetActive(true);
    }

    public void ClosePopup()
    {
        if (currentpopup != null)
        {
            currentpopup.SetActive(false);
            currentpopup = null;
        }
        background.SetActive(false);
        closeButton.SetActive(false);
    }
}
