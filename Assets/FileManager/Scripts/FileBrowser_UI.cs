using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FileBrowser_UI : MonoBehaviour
{
    #region EnumsAndStructs

    // Used to branch action when necessary, like open folders and return files
    public enum ButtonType
    {
        Drive,
        Folder,
        File,
    }

    // The items to display.
    // For example, if you only need to fetch a folder, getting files might be unecessary.
    public enum ToDisplay
    {
        Both,
        Folder,
        File,
    }

    // How should the UI react to you actions ?
    public enum FetchMode
    {
        SelectFile, // Open folders, return files
        SelectFolder, // Ignore files, open folders with double click, open button return selected folder
        SaveFile, // Open folders, fouble click on files set file name, open file return folder path and file name
    }

    [System.Serializable]
    public struct Filter
    {
        public string _sDisplay;
        public string[] _sExtensions;
    }

    [System.Serializable]
    public struct ButtonData
    {
        public ButtonType _eType;
        public string _sPath;
        public string _sLabel;

        public ButtonData(ButtonType eType, string sPath, string sLabel)
        {
            _eType = eType;
            _sPath = sPath;
            _sLabel = sLabel;
        }
    }

    #endregion

    #region Variables

    // Singleton reference
    static public FileBrowser_UI Instance { get; private set; }

    [Header("UI")]
    public GameObject _tRoot; // The root of all the UI
    public InputField _tAdressBar; // Input fieldto display current folder path
    public VirtualView _tVirtualView; // The virtual view where elements are displayed
    public Dropdown _tFiltersList; // The filter list to only display the files you want
    public InputField _tFileName; // The name of the file when selecting a file to save
    public GameObject _tConfirmFileOverride; // The root of the confirmation panel to override existing file when saving.
    public Text _tOpenButtonLabel; // The Text object displaying the "Open" or "Save" text.

    [Header("Navigation mode")]
    public ToDisplay _eToDisplay;
    public FetchMode _eFetchMode;

    [Header("Other")]
    public bool _bOpenWithOneClick;
    public float _fDoubleClickDelay;
    public Filter[] _tFilters; // one filter can contains several extensions
    
    public FileBrowser_Button _tCurrentlySelected { get; private set; } // The selected button, but set to null if anything else is selected in the UI
    public FileBrowser_Button _tLastSelected { get; private set; } // The selected button, but the value is kept until another FileBrowser_Button is sellected
    public string _tCurrentFolder { get; private set; } // The current folder. Only using the adress bar text might be an issue when it's modified
    public string _sResult { get; private set; } // The path returned when closing the file browser
    public bool _bIsOpen { get; private set; } // Use it if you want to wait in your script for the player to select the file or folder he wants
    private float _fDoubleClickTime; // Used to cancel double click when delay is passed
    private List<ButtonData> _tContent; // Hold the list of items to display
    private string _sDefaultExtension; // The extension applied to the file when in save mode

    #endregion

    #region UnityCallbacks

    // Simply set singleton reference
    void Awake()
    {
        Instance = this;

        _tContent = new List<ButtonData>();

        _tFiltersList.ClearOptions();
        for( int i = 0; i < _tFilters.Length; i++ )
        {
            _tFiltersList.options.Add(new Dropdown.OptionData(_tFilters[i]._sDisplay));
        }

        _tFiltersList.RefreshShownValue();
    }

    void Update()
    {
        // If unselected element, reset its value
        if( _tCurrentlySelected != null && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
        {
            _tCurrentlySelected = null;
        }
    }

    #endregion

    #region Window

    // Call this function to use the file browser
    public void ShowWindow(string sStartPath, ToDisplay eToDisplay, FetchMode eFetchMode, string sDefaultExtension = "")
    {
        _eToDisplay = eToDisplay;
        _eFetchMode = eFetchMode;
        _sDefaultExtension = sDefaultExtension;

        switch (_eFetchMode)
        {
            case FetchMode.SelectFile:
                _tFiltersList.gameObject.SetActive(true);
                _tFileName.gameObject.SetActive(false);
                _tOpenButtonLabel.text = "Open";
                break;
            case FetchMode.SelectFolder:
                _tFiltersList.gameObject.SetActive(false);
                _tFileName.gameObject.SetActive(false);
                _tOpenButtonLabel.text = "Select";
                break;
            case FetchMode.SaveFile:
                _tFiltersList.gameObject.SetActive(false);
                _tFileName.gameObject.SetActive(true);
                _tOpenButtonLabel.text = "Save";
                break;
        }

        FileBrowser_Core.Instance.OpenFolder(sStartPath);
        _tRoot.SetActive(true);
        _bIsOpen = true;
        _sResult = string.Empty;
    }

    // Call this function to use the file browser
    public void ShowWindow(string sStartPath, ToDisplay eToDisplay, string sDefaultExtension = "")
    {
        ShowWindow(sStartPath, eToDisplay, _eFetchMode, sDefaultExtension);
    }

    // Call this function to use the file browser
    public void ShowWindow(string sStartPath, FetchMode eFetchMode, string sDefaultExtension = "")
    {
        ShowWindow(sStartPath, _eToDisplay, eFetchMode, sDefaultExtension);
    }

    // Call this function to use the file browser
    public void ShowWindow(string sStartPath, string sDefaultExtension = "")
    {
        ShowWindow(sStartPath, _eToDisplay, _eFetchMode, sDefaultExtension);
    }

    // Call this one to close the file browser
    public void HideWindow(string sReturnValue)
    {
        _sResult = sReturnValue;
        _bIsOpen = false;
        _tRoot.SetActive(false);
        ClearView();

        Debug.Log(sReturnValue);
    }

    #endregion

    #region Navigation

    // Called when you click on a button and it's already selected
    public void Open_DoubleClick(ButtonType eType, string sPath)
    {
        // Cancel if delay of double click is passed
        if (Time.realtimeSinceStartup - _fDoubleClickTime > _fDoubleClickDelay && !_bOpenWithOneClick)
        {
            _fDoubleClickTime = Time.realtimeSinceStartup;
            return;
        }

        switch(eType)
        {
            // Always enter drive and folders when double click
            case ButtonType.Drive:
                FileBrowser_Core.Instance.OpenFolder(sPath);
                break;
            case ButtonType.Folder:
                FileBrowser_Core.Instance.OpenFolder(sPath);
                break;
            // When just selecting file, return current file, but when wanting to save, set the name of the file to save and return full path
            case ButtonType.File:
                if (_eFetchMode == FetchMode.SelectFile)
                    HideWindow(sPath);
                else if( _eFetchMode == FetchMode.SaveFile)
                    SaveFile(_tCurrentlySelected._sLabel.text);
                break;
        }
    }

    // The Open function called from the button in the UI
    public void Open_UI()
    {
        // If nothing is selected when in save file mode, save file with current set names
        if (_tCurrentlySelected == null)
        {
            if(_eFetchMode == FetchMode.SaveFile)
            {
                SaveFile();
            }
            return;
        }

        switch (_eFetchMode)
        {
            // If wanting to select a file, will open any folder, but return the file path
            case FetchMode.SelectFile:
                if (_tCurrentlySelected._eType == ButtonType.File)
                    HideWindow(_tCurrentlySelected._sPath);
                else
                    FileBrowser_Core.Instance.OpenFolder(_tCurrentlySelected._sPath);
                break;
            // If wanting to select a folder, will ignore any file, but return the folder path
            case FetchMode.SelectFolder:
                if (_tCurrentlySelected._eType == ButtonType.File)
                    return;
                else
                    HideWindow(_tCurrentlySelected._sPath);
                break;
            // If wanting to save a file, will open any folder, but will set the name of the current file and save otherwise
            case FetchMode.SaveFile:
                if (_tCurrentlySelected._eType == ButtonType.File)
                    SaveFile(_tCurrentlySelected._sLabel.text);
                else
                    FileBrowser_Core.Instance.OpenFolder(_tCurrentlySelected._sPath);
                break;
        }
    }

    void SaveFile(string sName = "")
    {
        // Check if provided string is empty. If not, set current file name to save.
        sName = sName.Trim();

        if (!string.IsNullOrEmpty(sName))
            _tFileName.text = sName;

        string sPath = Path.Combine(_tCurrentFolder, _tFileName.text);

        // If an extension was provided, check if the one the user provided is the same. If not, replace it.
        if (!string.IsNullOrEmpty( _sDefaultExtension ))
        {
            FileInfo tInfo = new FileInfo(sPath);
            if (tInfo.Extension.ToLower() != _sDefaultExtension.ToLower())
            {
                sPath = Path.ChangeExtension(sPath, _sDefaultExtension);
            }
        }

        // If file already exists, ask user to confirm.
        if (File.Exists(sPath))
            _tConfirmFileOverride.SetActive(true);
        else
            HideWindow(sPath);
    }

    public void ConfirmSave(bool bConfirm)
    {
        _tConfirmFileOverride.SetActive(false);

        if (bConfirm)
            HideWindow(System.IO.Path.Combine(_tCurrentFolder, _tFileName.text));
    }

    public void Refresh()
    {
        FileBrowser_Core.Instance.OpenFolder(_tCurrentFolder);
    }

    #endregion

    #region ViewportCreation

    public void SetData(IEnumerable<ButtonData> tData)
    {
        _tContent.Clear();

        _tContent.AddRange(tData);

        _tVirtualView.CreateView(_tContent.Count, AddCallback);
    }

    public void AddData(ButtonData tData)
    {
        _tContent.Add(tData);

        _tVirtualView.CreateView(_tContent.Count, AddCallback);
    }

    public void AddData(IEnumerable<ButtonData> tData)
    {
        _tContent.AddRange(tData);

        _tVirtualView.CreateView(_tContent.Count, AddCallback);
    }

    public void AddCallback(GameObject tObject, int iID)
    {
        tObject.GetComponent<FileBrowser_Button>().Init(_tContent[iID]._eType, _tContent[iID]._sPath, _tContent[iID]._sLabel);
    }

    public void ClearView()
    {
        _tVirtualView.ClearView();
        _tContent.Clear();

        _tCurrentlySelected = null;

        _fDoubleClickTime = 0;
    }

    // Set current folder path
    public void SetFolder(string sPath)
    {
        _tCurrentFolder = sPath;

        _tAdressBar.text = sPath;
    }

    // Set currently selected button
    public void SetSelected(FileBrowser_Button tSelected)
    {
        _tLastSelected = tSelected;
        _tCurrentlySelected = tSelected;
        _fDoubleClickTime = Time.realtimeSinceStartup;
    }

    #endregion
}
