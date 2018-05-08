using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FileBrowser_ColorManager : MonoBehaviour
{
    #region StructsAndEnums

    [System.Serializable]
    public struct LabelColor
    {
        public string _sLabel;
        public Color _tColor;
    }

    #endregion

    #region Variables

    public FileBrowser_Color[] _tColors;
    public LabelColor[] _tAssociations;
    public Color _tGlobalColor;
    public bool _bSet;
    public bool _bRevert;

    #endregion

    #region UnityCallbacks

    void Update()
    {
        if( _bSet )
        {
            Set();
            _bSet = false;
        }
        if( _bRevert )
        {
            Revert();
            _bRevert = false;
        }
    }

    #endregion

    #region Functions

    void Set()
    {
        // For each association, if the label match the one from a FileBrowser_Color item, set its color

        for (int i = 0; i < _tAssociations.Length; i++)
        {
            for (int j = 0; j < _tColors.Length; j++)
            {
                for (int k = 0; k < _tColors[j]._tImagesColors.Length; k++)
                {
                    for (int l = 0; l < _tColors[j]._tImagesColors[k]._tImages.Length; l++)
                    {
                        if (_tAssociations[i]._sLabel == _tColors[j]._tImagesColors[k]._sLabel)
                            _tColors[j]._tImagesColors[k]._tImages[l].color = _tAssociations[i]._tColor * _tGlobalColor;
                    }
                }
                for (int k = 0; k < _tColors[j]._tTextsColors.Length; k++)
                {
                    for (int l = 0; l < _tColors[j]._tTextsColors[k]._tTexts.Length; l++)
                    {
                        if (_tAssociations[i]._sLabel == _tColors[j]._tTextsColors[k]._sLabel)
                            _tColors[j]._tTextsColors[k]._tTexts[l].color = _tAssociations[i]._tColor * _tGlobalColor;
                    }
                }
                for (int k = 0; k < _tColors[j]._tOutlinesColors.Length; k++)
                {
                    for (int l = 0; l < _tColors[j]._tOutlinesColors[k]._tOutlines.Length; l++)
                    {
                        if (_tAssociations[i]._sLabel == _tColors[j]._tOutlinesColors[k]._sLabel)
                            _tColors[j]._tOutlinesColors[k]._tOutlines[l].effectColor = _tAssociations[i]._tColor * _tGlobalColor;
                    }
                }
            }
        }
    }

    void Revert()
    {
        for (int i = 0; i < _tAssociations.Length; i++)
            _tAssociations[i]._tColor = new Color(1 - _tAssociations[i]._tColor.r, 1 - _tAssociations[i]._tColor.g, 1 - _tAssociations[i]._tColor.b, _tAssociations[i]._tColor.a);
    }
    
    #endregion
}
