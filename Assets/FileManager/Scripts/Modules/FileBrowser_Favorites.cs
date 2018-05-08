using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class FileBrowser_Favorites : MonoBehaviour
{
    #region StructsAndEnums

    public enum PathMode
    {
        Absolute,
        FromDataPath,
        FromPersistentDataPath,
    }

    #endregion

    #region Variables
    [Header("Color change")]

    public Image _tFavoriteImage; // Will change color when in favorite folder
    private Color _tDefaultColor;
    public Color _tFavoriteColor = Color.yellow;

    [Header("Template")]

    public FileBrowser_Button _tButtonTemplate; // Button to instentiate for each favorite folder

    [Header("Path")]

    public string _sPathToFavoritesFile; // Path to the favorites file
    public PathMode _ePathMode = PathMode.FromPersistentDataPath; // From where does the path start ?

    private List<string> _sFavorites = new List<string>(); // Current list of favorites folders
    private List<FileBrowser_Button> _tCurrentView = new List<FileBrowser_Button>(); // Instantiated buttons of the list of favorites folders

    #endregion

    #region UnityCallback

    void Start()
    {
        _tDefaultColor = _tFavoriteImage.color;

        // Get path to file

        string sPath = _sPathToFavoritesFile;

        switch(_ePathMode)
        {
            case PathMode.FromDataPath:
                sPath = Path.Combine(Application.dataPath, sPath);
                break;
            case PathMode.FromPersistentDataPath:
                sPath = Path.Combine(Application.persistentDataPath, sPath);
                break;
        }

        // If file exists, parse it

        if ( File.Exists(sPath) )
        {
            StreamReader tSR = new StreamReader(sPath);

            while (tSR.Peek() >= 0)
            {
                string sFav = tSR.ReadLine().Trim();

                if (!string.IsNullOrEmpty(sFav))
                    _sFavorites.Add(sFav);
            }

            tSR.Dispose();
        }

        // Update view to create a button for each folder

        UpdateView();

        // When a folder is opened, check if it's a favorite

        FileBrowser_Core.Instance._tOnFolderOpened.AddListener(CheckCurrentFolder);
    }

    #endregion

    #region UICallbacks

    public void AddOrRemove()
    {
        string sCurrent = FileBrowser_UI.Instance._tCurrentFolder;

        if(_sFavorites.Contains(sCurrent))
            _sFavorites.Remove(sCurrent);
        else
            _sFavorites.Add(sCurrent);

        SaveFavorites();
        UpdateView();
        CheckCurrentFolder();
    }

    #endregion

    #region Functions

    void SaveFavorites()
    {
        string sPath = _sPathToFavoritesFile;

        switch (_ePathMode)
        {
            case PathMode.FromDataPath:
                sPath = Path.Combine(Application.dataPath, sPath);
                break;
            case PathMode.FromPersistentDataPath:
                sPath = Path.Combine(Application.persistentDataPath, sPath);
                break;
        }

        StreamWriter tSW = new StreamWriter(sPath);

        for (int i = 0; i < _sFavorites.Count; i++)
            tSW.WriteLine(_sFavorites[i]);

        tSW.Close();
    }

    void UpdateView()
    {
        // Clear all current buttons

        for (int i = 0; i < _tCurrentView.Count; i++)
            Destroy(_tCurrentView[i].gameObject);

        _tCurrentView.Clear();

        // For each faborite, create a button with the right label and the right path to to folder to open

        for (int i = 0; i < _sFavorites.Count; i++)
        {
            GameObject tNewButtonGO = Instantiate(_tButtonTemplate.gameObject);
            tNewButtonGO.SetActive(true);
            tNewButtonGO.transform.SetParent(_tButtonTemplate.transform.parent);

            FileBrowser_Button tNewButton = tNewButtonGO.GetComponent<FileBrowser_Button>();
            tNewButton.Init(FileBrowser_UI.ButtonType.Folder, _sFavorites[i], new DirectoryInfo(_sFavorites[i]).Name);

            _tCurrentView.Add(tNewButton);
        }
    }

    // Change the color of the favorite icon when in a favorite folder
    public void CheckCurrentFolder()
    {
        if (_sFavorites.Contains(FileBrowser_UI.Instance._tCurrentFolder))
            _tFavoriteImage.color = _tFavoriteColor;
        else
            _tFavoriteImage.color = _tDefaultColor;
    }

    #endregion
}
