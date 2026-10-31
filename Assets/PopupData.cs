using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPopupData", menuName = "Popup/Popup Data")]
public class PopupData : ScriptableObject
{
    [Header("ボタン")]
    public int id;
    public string title;

    [Header("ポップアップの中身")]
    public List<TextItem> texts = new List<TextItem>();
    public List<ImageItem> images = new List<ImageItem>();
}

[Serializable]
public class TextItem
{
    [TextArea(3, 10)]
    public string text;

    public int fontSize = 18;

    public Vector2 position = Vector2.zero;
    public Vector2 size = new Vector2(300, 100);
}

[Serializable]
public class ImageItem
{
    public Sprite image;

    public Vector2 position = Vector2.zero;
    public Vector2 size = new Vector2(200, 200);
}
