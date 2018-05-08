using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FileBrowser_Color : MonoBehaviour
{
    #region StructsAndEnums

    [System.Serializable]
    public struct ImageColor
    {
        public string _sLabel;
        public Image[] _tImages;
    }

    [System.Serializable]
    public struct TextColor
    {
        public string _sLabel;
        public Text[] _tTexts;
    }

    [System.Serializable]
    public struct OutlineColor
    {
        public string _sLabel;
        public Outline[] _tOutlines;
    }

    #endregion

    #region Variables

    public ImageColor[] _tImagesColors;
    public TextColor[] _tTextsColors;
    public OutlineColor[] _tOutlinesColors;

    #endregion
}
