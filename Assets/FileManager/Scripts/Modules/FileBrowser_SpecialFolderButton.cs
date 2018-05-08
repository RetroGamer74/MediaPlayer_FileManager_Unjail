using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FileBrowser_SpecialFolderButton : MonoBehaviour
{
    /*
        Simply add this class to a button and select the enum value you want.
        The folder it would send you to is plateform specific.
        Not all the enum values are used, depending of the target plateform.
        Please refer to the target plateform documentation for more information on the values of the enum.
    */

    [Tooltip("If enable, has priority over other parameters.")]
    public bool _bToApplicationDataPath;
    [Tooltip("If enable, has priority over special folder enum.")]
    public bool _bToApplicationPersistentDataPath;
    [Tooltip("If no boolean is enabled above, go to selected folder.")]
    public Environment.SpecialFolder _eFolder;

    public void Open()
    {
        if (_bToApplicationDataPath)
            FileBrowser_Core.Instance.OpenFolder("/mnt/usb0");
        else if (_bToApplicationPersistentDataPath)
            FileBrowser_Core.Instance.OpenFolder(Application.persistentDataPath);
        else
            FileBrowser_Core.Instance.OpenFolder(Environment.GetFolderPath(_eFolder));
    }
}
