using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class FileBrowser_FolderManagement : MonoBehaviour
{
    #region Variables

    [Header("Deletion")]

    public GameObject _tDeleteConfirmScreen;
    public Text _tDeleteConfirmText;

    [Header("Creation")]

    public GameObject _tCreateConfirmScreen;
    public InputField _tCreateName;
    public string _sDefaultDirectoryName = "Untitled";

    [Header("Rename")]
    
    public GameObject _tRenameConfirmScreen;
    public InputField _tRenameName;

    [Header("Copy,Cut,Paste")]

    public Color _tCutColor;

    // Used for cupy cut, and paste operations

    private string _sCopyPath;
    private bool _bCut;

    #endregion

    #region Delete

    // If something is selected, show confirmation screen
    public void ConfirmDelete()
    {
        if (FileBrowser_UI.Instance._tLastSelected != null)
        {
            _tDeleteConfirmScreen.SetActive(true);

            _tDeleteConfirmText.text = "Do you really want to delete \"" + FileBrowser_UI.Instance._tLastSelected._sLabel.text + "\" ?";
        }
    }

    // Hide window
    public void CancelDelete()
    {
        _tDeleteConfirmScreen.SetActive(false);
    }

    // Delete file, hide window, and refresh display
    public void DeleteSelected()
    {
        if (File.Exists(FileBrowser_UI.Instance._tCurrentlySelected._sPath))
            File.Delete(FileBrowser_UI.Instance._tCurrentlySelected._sPath);
        else if (Directory.Exists(FileBrowser_UI.Instance._tCurrentlySelected._sPath))
            Directory.Delete(FileBrowser_UI.Instance._tCurrentlySelected._sPath, true);

        _tDeleteConfirmScreen.SetActive(false);

        FileBrowser_UI.Instance.Refresh();
    }

    #endregion

    #region Create

    // Show confirmation screen with default directory name
    public void ConfirmCreate()
    {
        _tCreateConfirmScreen.SetActive(true);
        _tCreateName.text = _sDefaultDirectoryName;
    }

    // Hide window
    public void CancelCreate()
    {
        _tCreateConfirmScreen.SetActive(false);
    }

    // Makes sure the name is unique, create the directory, and refresh the UI
    public void CreateDirectory()
    {
        string sPath = FileBrowser_UI.Instance._tCurrentFolder + "/" + _tCreateName.text;
        if (Directory.Exists(sPath))
        {
            for( int i = 0; ; i++ )
            {
                if(!Directory.Exists(sPath + " (" + i + ")"))
                {
                    sPath += " (" + i + ")";
                    break;
                }
            }
        }

        Directory.CreateDirectory(sPath);

        _tCreateConfirmScreen.SetActive(false);

        FileBrowser_UI.Instance.Refresh();
    }

    #endregion

    #region CopyCutPaste

    // Remember the path of the selected file (it will be lost in the navigation process if not stored)
    public void Copy()
    {
        _sCopyPath = FileBrowser_UI.Instance._tLastSelected._sPath;
        _bCut = false;
    }

    // Same as copy, but also hide the button in the UI
    public void Cut()
    {
        _sCopyPath = FileBrowser_UI.Instance._tCurrentlySelected._sPath;
        _bCut = true;

        FileBrowser_UI.Instance._tCurrentlySelected.GetComponent<Image>().color = _tCutColor;
    }

    // Copy the file or move it if cut was chosen, and refresh the UI
    public void Paste()
    {
        if ( File.Exists(_sCopyPath) )
        {
            string sPath = FileBrowser_UI.Instance._tCurrentFolder + "/" + new FileInfo(_sCopyPath).Name;

            if (File.Exists(sPath))
            {
                for (int i = 0; ; i++)
                {
                    if (!File.Exists(sPath + " (" + i + ")"))
                    {
                        sPath += " (" + i + ")";
                        break;
                    }
                }
            }

            if (_bCut)
                File.Move(_sCopyPath, sPath);
            else
                File.Copy(_sCopyPath, sPath);

            FileBrowser_UI.Instance.Refresh();
        }
        else if( Directory.Exists(_sCopyPath) )
        {
            string sPath = FileBrowser_UI.Instance._tCurrentFolder + "/" + new DirectoryInfo(_sCopyPath).Name;

            if (Directory.Exists(sPath))
            {
                for (int i = 0; ; i++)
                {
                    if (!Directory.Exists(sPath + " (" + i + ")"))
                    {
                        sPath += " (" + i + ")";
                        break;
                    }
                }
            }

            if (_bCut)
                Directory.Move(_sCopyPath, FileBrowser_UI.Instance._tCurrentFolder + "/" + new DirectoryInfo(_sCopyPath).Name);
            else
                DirectoryCopy(_sCopyPath, FileBrowser_UI.Instance._tCurrentFolder + "/" + new DirectoryInfo(_sCopyPath).Name);

            FileBrowser_UI.Instance.Refresh();
        }

        _sCopyPath = string.Empty;
    }

    private void DirectoryCopy(string sSourceDirName, string sDestDirName)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo tDirectory = new DirectoryInfo(sSourceDirName);

        DirectoryInfo[] tDirs = tDirectory.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(sDestDirName))
        {
            Directory.CreateDirectory(sDestDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] tFiles = tDirectory.GetFiles();
        foreach (FileInfo tFile in tFiles)
        {
            string sTempPath = Path.Combine(sDestDirName, tFile.Name);
            tFile.CopyTo(sTempPath, false);
        }

        // Copy subdirectories and their contents to new location.
        foreach (DirectoryInfo tSubdir in tDirs)
        {
            string sTempPath = Path.Combine(sDestDirName, tSubdir.Name);
            DirectoryCopy(tSubdir.FullName, sTempPath);
        }
    }

    #endregion

    #region Rename

    // Show confirmation screen with current file name
    public void ConfirmRename()
    {
        _tRenameConfirmScreen.SetActive(true);
        _tRenameName.text = FileBrowser_UI.Instance._tLastSelected._sLabel.text;
    }

    // Hide window
    public void CancelRename()
    {
        _tRenameConfirmScreen.SetActive(false);
    }

    // Rename folder and refresh UI
    public void RenameDirectory()
    {
        string sPath = FileBrowser_UI.Instance._tLastSelected._sPath;

        if (File.Exists(sPath))
            File.Move(sPath, FileBrowser_UI.Instance._tCurrentFolder + "/" + _tRenameName.text);
        else if (Directory.Exists(sPath))
            Directory.Move(sPath, FileBrowser_UI.Instance._tCurrentFolder + "/" + _tRenameName.text);

        FileBrowser_UI.Instance.Refresh();

        _tRenameConfirmScreen.SetActive(false);
    }

    #endregion
}
