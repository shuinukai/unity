using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPopupData", menuName = "Popup/Popup Data")]
public class PopupData : ScriptableObject
{
    public List<TextItem> texts = new List<TextItem>();
    public List<ImageItem> images = new List<ImageItem>();
}

[Serializable]
public class TextItem
{
    [TextArea(3, 10)]
    public string text;

    public int fontSize = 18;
}

[Serializable]
public class ImageItem
{
    public Sprite image;
}
