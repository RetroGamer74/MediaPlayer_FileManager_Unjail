using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FileBrowser_Search : MonoBehaviour
{
    #region Variables

    public Image _tSearching;
    public Color _tBlinkingColor = new Color(1, 1, 1, 0);
    public bool _bDisableLooseFocusTrigger = true;

    private Color _tStartColor;
    private Thread_Search _tSearchThread;

    #endregion

    #region UICallbacks

    public void Search(string sPattern)
    {
        if (!Input.GetButtonDown("Submit") && _bDisableLooseFocusTrigger)
            return;

        // Clear all items from the current view
        FileBrowser_UI.Instance.ClearView();

        // Avoid having several search running at the same time
        StopAllCoroutines();

        if (_tSearchThread != null)
            _tSearchThread.Abort();

        // Start new search
        _tSearchThread = new Thread_Search(FileBrowser_UI.Instance._tCurrentFolder, sPattern);
        _tSearchThread.Start();

        StartCoroutine(Coroutine_Search());
    }

    public void Cancel()
    {
        StopAllCoroutines();

        if (_tSearchThread != null)
            _tSearchThread.Abort();

        FileBrowser_UI.Instance.Refresh();

        _tSearching.color = _tStartColor;
    }

    #endregion

    #region Functions

    IEnumerator Coroutine_Search()
    {
        _tStartColor = _tSearching.color;

        // Retrieve latest found files and display them, until search is complete

        while (!_tSearchThread.Update())
        {
            _tSearching.color = Color.Lerp(_tStartColor, _tBlinkingColor, Mathf.Sin(Time.realtimeSinceStartup) * 0.5f + 0.5f);

            List<string> tNew = _tSearchThread.GetLastResults();
            CreateView(tNew.ToArray());
            yield return null;
        }

        List<string> tLast = _tSearchThread.GetLastResults();
        CreateView(tLast.ToArray());

        _tSearching.color = _tStartColor;
    }

    void CreateView(string[] sPaths)
    {
        string[] sFilters = FileBrowser_UI.Instance._tFilters[FileBrowser_UI.Instance._tFiltersList.value]._sExtensions;
        List<FileBrowser_UI.ButtonData> tData = new List<FileBrowser_UI.ButtonData>();

        for (int i = 0; i < sPaths.Length; i++)
        {
            if (FileBrowser_UI.Instance._eToDisplay != FileBrowser_UI.ToDisplay.File)
            {
                // Retrieve directory data

                DirectoryInfo tInfo = new DirectoryInfo(sPaths[i]);

                // Add entry

                tData.Add(new FileBrowser_UI.ButtonData(FileBrowser_UI.ButtonType.Folder, tInfo.FullName, tInfo.Name));
            }
            else if (FileBrowser_UI.Instance._eToDisplay != FileBrowser_UI.ToDisplay.Folder)
            {
                // Retrieve file data

                FileInfo tInfo = new FileInfo(sPaths[i]);

                // Skip entry if not among selected filters

                bool bSkip = sFilters.Length != 0;

                for (int j = 0; j < sFilters.Length; j++)
                {
                    if (tInfo.Extension.ToLower() == sFilters[j].ToLower())
                        bSkip = false;
                }

                if (bSkip)
                    continue;

                // Add entry

                tData.Add(new FileBrowser_UI.ButtonData(FileBrowser_UI.ButtonType.File, tInfo.FullName, tInfo.Name));
            }
        }

        FileBrowser_UI.Instance.AddData(tData);
    }

    #endregion
}