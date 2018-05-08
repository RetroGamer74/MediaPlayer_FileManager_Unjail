using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;

public class FileBrowser_Appearance : MonoBehaviour
{
    #region EnumsAndStructs

    public enum SortMode
    {
        Name,
        Date,
        Type,
    }

    #endregion

    #region Variables

    private SortMode _eSortMode;

    #endregion

    #region UnityCallbacks

    void Start()
    {
        // Simply register sorting function to the core manager
        FileBrowser_Core.Instance._tOnDirectoryDataRetrieve.AddListener(Sort);
    }

    #endregion

    #region UICallbacks

    public void SetHeight(float fValue)
    {
        FileBrowser_UI.Instance._tVirtualView._fHeight = fValue;
        FileBrowser_UI.Instance._tVirtualView.UpdateLook();

        Transform tView = FileBrowser_UI.Instance._tVirtualView._tScrollRect.content.transform;

        for( int i = 0; i < tView.childCount; i++ )
        {
            FileBrowser_Button tButton = tView.GetChild(i).GetComponent<FileBrowser_Button>();

            if( tButton != null )
            {
                Vector2 tDelta = tButton._tSubImage.rectTransform.sizeDelta;
                tDelta.x = fValue - 10;
                tButton._tSubImage.rectTransform.sizeDelta = tDelta;

                Vector2 tPosition = tButton._sLabel.rectTransform.offsetMin;
                tPosition.x = fValue + 5;
                tButton._sLabel.rectTransform.offsetMin = tPosition;
            }
        }
    }

    public void SetSortMode(int iValue)
    {
        _eSortMode = (SortMode)iValue;
        FileBrowser_UI.Instance.Refresh();
    }

    #endregion

    #region Functions

    // Simply dispatch the sorting function based on if it's for the files or the directories
    void Sort(bool bIsDirectory)
    {

        if ( bIsDirectory )
        {
            switch (_eSortMode)
            {
                case SortMode.Date:
                {
                    Array.Sort<string>(FileBrowser_Core.Instance._tCurrentData, CompareDirectoryByDate);
                    break;
                }
                default:
                {
                    Array.Sort<string>(FileBrowser_Core.Instance._tCurrentData, CompareDirectoryByName);
                    break;
                }
            }
        }
        else
        {
            switch (_eSortMode)
            {
                case SortMode.Date:
                    {
                        Array.Sort<string>(FileBrowser_Core.Instance._tCurrentData, CompareFileByDate);
                        break;
                    }
                case SortMode.Type:
                    {
                        Array.Sort<string>(FileBrowser_Core.Instance._tCurrentData, CompareFileByType);
                        break;
                    }
                default:
                    {
                        Array.Sort<string>(FileBrowser_Core.Instance._tCurrentData, CompareFileByName);
                        break;
                    }
            }
        }
    }

    #endregion

    #region SortingFunctions

    private int CompareDirectoryByName(string s1, string s2)
    {
        return s1.CompareTo(s2);
    }

    private int CompareDirectoryByDate(string s1, string s2)
    {
        DirectoryInfo t1 = new DirectoryInfo(s1);
        DirectoryInfo t2 = new DirectoryInfo(s2);

        return t1.CreationTime.CompareTo(t2.CreationTime);
    }

    private int CompareFileByName(string s1, string s2)
    {
        return s1.CompareTo(s2);
    }

    private int CompareFileByType(string s1, string s2)
    {
        FileInfo t1 = new FileInfo(s1);
        FileInfo t2 = new FileInfo(s2);

        return t1.Extension.CompareTo(t2.Extension);
    }

    private int CompareFileByDate(string s1, string s2)
    {
        FileInfo t1 = new FileInfo(s1);
        FileInfo t2 = new FileInfo(s2);

        return t1.CreationTime.CompareTo(t2.CreationTime);
    }

    #endregion
}
