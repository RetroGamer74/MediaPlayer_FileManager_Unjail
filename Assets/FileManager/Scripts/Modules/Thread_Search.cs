using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Thread_Search : ThreadedJob
{
    #region Variables

    private string _sPath;
    private string _sSearchPattern;

    private object _tResultHandle = new object(); // Used to lock when necessary
    private List<string> _tResult = new List<string>();

    #endregion

    #region ThreadSafeAccessors

    private List<string> Result
    {
        get
        {
            List<string> tTmp;
            lock (_tResultHandle)
            {
                tTmp = _tResult;
            }
            return tTmp;
        }
        set
        {
            lock (_tResultHandle)
            {
                _tResult = value;
            }
        }
    }

    public List<string> GetLastResults()
    {
        List<string> tTmp;
        lock (_tResultHandle)
        {
            tTmp = new List<string>(_tResult);
            _tResult.Clear();
        }
        return tTmp;
    }

    #endregion

    #region Functions

    public Thread_Search(string sPath, string sSearchpattern)
    {
        _sPath = sPath;
        _sSearchPattern = "*" + sSearchpattern + "*";
    }

    protected override void ThreadFunction()
    {
        SearchIn(_sPath, _sSearchPattern);
    }

    protected override void OnFinished()
    {
    }

    private void SearchIn(string sPath, string sSearchpattern)
    {
        // Retrieve matching files and directories of the current folder

        string[] sDirectories = Directory.GetDirectories(sPath, sSearchpattern);
        string[] sFiles = Directory.GetFiles(sPath, sSearchpattern);

        // Add them to the list

        Result.AddRange(sDirectories);
        Result.AddRange(sFiles);

        // Search recursively in the subdirectories

        string[] sToSearch = Directory.GetDirectories(sPath);
        for (int i = 0; i < sToSearch.Length; i++)
            SearchIn(sToSearch[i], sSearchpattern);
    }

    #endregion
}
