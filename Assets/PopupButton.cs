using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public PopupController controller;
    public PopupData data;

    public void Open()
    {
        controller.OpenPopup(data);
    }
}