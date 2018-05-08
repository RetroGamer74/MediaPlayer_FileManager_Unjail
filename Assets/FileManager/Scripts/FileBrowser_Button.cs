using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ButtonUnityEvent : UnityEvent<FileBrowser_Button>
{}

public class FileBrowser_Button : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite _tDrive;
    public Sprite _tFolder;
    public Sprite _tFile;

    [Header("UI")]
    public Button _tButton;
    public Image _tSubImage;
    public Text _sLabel;

    [Header("Callback")]
    public ButtonUnityEvent _tOnInit;

    // Data
    public FileBrowser_UI.ButtonType _eType { get; private set; }
    public string _sPath { get; private set; }

    public void Init(FileBrowser_UI.ButtonType eType, string sPath, string sLabel)
    {
        // Set data
        _eType = eType;
        _sPath = sPath;
        _sLabel.text = sLabel;

        if( _tSubImage.sprite != _tDrive
            && _tSubImage.sprite != _tFolder
            && _tSubImage.sprite != _tFile)
        {
            DestroyImmediate(_tSubImage.sprite);
        }

        // Set callback
        _tButton.onClick.RemoveAllListeners();
        _tButton.onClick.AddListener(OnClick);

        // Set necessary picture
        switch (_eType)
        {
            case FileBrowser_UI.ButtonType.Drive:
                _tSubImage.sprite = _tDrive;
                break;
            case FileBrowser_UI.ButtonType.Folder:
                _tSubImage.sprite = _tFolder;
                break;
            case FileBrowser_UI.ButtonType.File:
                _tSubImage.sprite = _tFile;
                break;
        }

        _tOnInit.Invoke(this);
    }

    public void OnClick()
    {
        if( FileBrowser_UI.Instance._bOpenWithOneClick )
        {
            FileBrowser_UI.Instance.SetSelected(this);
            FileBrowser_UI.Instance.Open_DoubleClick(_eType, _sPath);
        }

        if( FileBrowser_UI.Instance._tCurrentlySelected == this )
        {
            FileBrowser_UI.Instance.Open_DoubleClick(_eType, _sPath);
        }
        else
        {
            FileBrowser_UI.Instance.SetSelected(this);
        }
    }
}
