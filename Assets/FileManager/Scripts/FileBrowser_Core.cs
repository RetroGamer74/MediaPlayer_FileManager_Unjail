using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

public class BoolEvent : UnityEvent<bool> { }

public class FileBrowser_Core : MonoBehaviour
{
    #region Variables

    // Singleton reference
    static public FileBrowser_Core Instance { get; private set; }

    public UnityEvent _tOnFolderStartOpen;
    public UnityEvent _tOnFolderOpened;
    public BoolEvent _tOnDirectoryDataRetrieve = new BoolEvent(); // Boolean will be true when directories will be treated, and false when files will be treated

    [HideInInspector]
    public string[] _tCurrentData;

    #endregion

    #region UnityCallbacks

    // Simply set the singleton reference
    void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Navigation

    public void OpenParent()
    {
        // If at root, can't go to any parent folder
        if (FileBrowser_UI.Instance._tCurrentFolder == "Root")
            return;

        // Get info on current folder to retrieve parent folder, and call the necessary function
        DirectoryInfo tInfo = new DirectoryInfo(FileBrowser_UI.Instance._tCurrentFolder);

        if (tInfo.Parent != null)
            OpenFolder(tInfo.Parent.FullName);
        else
            DisplayDrives();
    }

    public void OpenFolder(string sPath)
    {
        sPath = sPath.Trim(); // Just to be sure

        // Go to root when necessary
        if( sPath == "Root" || string.IsNullOrEmpty(sPath) )
        {
            DisplayDrives();
            return;
        }
        // Make sure the path is correct, and regenerate the UI
        else if(Directory.Exists(sPath))
        {
            StopAllCoroutines();

            FileBrowser_UI.Instance.ClearView();

            FileBrowser_UI.Instance.SetFolder(sPath);

            CreateView(sPath);
        }
    }

    public void DisplayDrives()
    {
        _tOnFolderStartOpen.Invoke();

        FileBrowser_UI.Instance.SetFolder("Root");

        string[] tDrives = Directory.GetLogicalDrives();
        FileBrowser_UI.ButtonData[] tData = new FileBrowser_UI.ButtonData[tDrives.Length];

        for (int i = 0; i < tDrives.Length; i++)
            tData[i] = new FileBrowser_UI.ButtonData(FileBrowser_UI.ButtonType.Drive, tDrives[i], tDrives[i]);

        FileBrowser_UI.Instance.SetData(tData);

        _tOnFolderOpened.Invoke();
    }

    #endregion

    #region ViewportCreation

    void CreateView(string sPath)
    {
        _tOnFolderStartOpen.Invoke();

        List<FileBrowser_UI.ButtonData> tData = new List<FileBrowser_UI.ButtonData>();

        if ( FileBrowser_UI.Instance._eToDisplay != FileBrowser_UI.ToDisplay.File ) // If only file, don't fetch the folders
        {
            // Retrieve all the directory in the current folder, extract there info, and create a button for each of them
            _tCurrentData = Directory.GetDirectories(sPath);
            _tOnDirectoryDataRetrieve.Invoke(true);

            for (int i = 0; i < _tCurrentData.Length; i++)
            {
                DirectoryInfo tInfo = new DirectoryInfo(_tCurrentData[i]);
                tData.Add(new FileBrowser_UI.ButtonData(FileBrowser_UI.ButtonType.Folder, tInfo.FullName, tInfo.Name));
            }
        }

        if( FileBrowser_UI.Instance._eToDisplay != FileBrowser_UI.ToDisplay.Folder ) // If only folders, don't fetch the files
        {
            // Retrieve all the files in the current folder, extract there info, and create a button for each of them
            _tCurrentData = Directory.GetFiles(sPath);
            _tOnDirectoryDataRetrieve.Invoke(false);

            string[] sFilters = FileBrowser_UI.Instance._tFilters[FileBrowser_UI.Instance._tFiltersList.value]._sExtensions;

            for (int i = 0; i < _tCurrentData.Length; i++)
            {
                FileInfo tInfo = new FileInfo(_tCurrentData[i]);

                bool bSkip = sFilters.Length != 0;

                for( int j = 0; j < sFilters.Length; j++ )
                {
                    if (tInfo.Extension.ToLower() == sFilters[j].ToLower())
                        bSkip = false;
                }

                if (bSkip)
                    continue;
                
                tData.Add(new FileBrowser_UI.ButtonData(FileBrowser_UI.ButtonType.File, tInfo.FullName, tInfo.Name));
            }
        }
        
        FileBrowser_UI.Instance.SetData(tData);

        _tOnFolderOpened.Invoke();
    }

    #endregion
}
