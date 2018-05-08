using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FileBrowser_History : MonoBehaviour
{
    #region Variables

    private List<string> _tHistory = new List<string>(); // List of all the folder opened
    private int _iCurrentPointer = 0; // Current position in the history list
    private bool _bIgnoreHistory = false; // Used to avoid writing to history when actually going through it

    #endregion

    #region UnityCallbacks

    void Start()
    {
        // When a folder is opened, add it to the history
        FileBrowser_Core.Instance._tOnFolderStartOpen.AddListener(SetNextEntry);
    }

    #endregion

    #region UICallbacks

    public void GoToNext()
    {
        GoTo(_iCurrentPointer + 1);
    }

    public void GoToPrevious()
    {
        GoTo(_iCurrentPointer - 1);
    }

    #endregion

    #region Functions

    public void SetNextEntry()
    {
        // If new folder was open from here, cancel
        if(_bIgnoreHistory)
        {
            _bIgnoreHistory = false;
            return;
        }

        // If cursor is not at last entry, delete excess history
        if (_iCurrentPointer < (_tHistory.Count - 1))
            _tHistory.RemoveRange(_iCurrentPointer + 1, _tHistory.Count - (_iCurrentPointer + 1));
        
        // Add entry and set cursor
        _tHistory.Add(FileBrowser_UI.Instance._tCurrentFolder);
        _iCurrentPointer = _tHistory.Count - 1;
    }

    public void GoTo(int iPointer)
    {
        iPointer = Mathf.Clamp(iPointer, 0, _tHistory.Count-1);

        // If no change, or history is too short
        if (iPointer == _iCurrentPointer || _tHistory.Count == 1)
            return;

        // Set cursor, make sur we won't add this entry to history, and open folder
        _iCurrentPointer = iPointer;
        _bIgnoreHistory = true;
        FileBrowser_Core.Instance.OpenFolder(_tHistory[_iCurrentPointer]);
    }

    #endregion
}
