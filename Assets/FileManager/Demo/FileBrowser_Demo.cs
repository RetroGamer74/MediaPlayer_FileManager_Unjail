using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FileBrowser_Demo : MonoBehaviour
{
    public string _sStartPath;
    public FileBrowser_UI.FetchMode _eMode;
    public FileBrowser_UI.ToDisplay _eDisplay;
    public string _sDefaultExtension;
    public string _tResult;
	public Transform PanelFileBrowser;
	public Transform VideoPlayer;
	public PS4VideoPlaybackSample videoInfo;
	public SonyPS4CommonDialog SonyCMD;

    void Update ()
    {
        if (!FileBrowser_UI.Instance._bIsOpen && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(WaitForResult());
	}

    IEnumerator WaitForResult()
    {
        FileBrowser_UI.Instance.ShowWindow(_sStartPath, _eDisplay, _eMode, _sDefaultExtension);

        while (FileBrowser_UI.Instance._bIsOpen)
            yield return null;

        _tResult = FileBrowser_UI.Instance._sResult;
		videoInfo.moviePath = _tResult;
		VideoPlayer.SetAsLastSibling ();
    }

	public void OpenFileBrowser()
	{
		if (!FileBrowser_UI.Instance._bIsOpen) {
			StartCoroutine (WaitForResult ());
			PanelFileBrowser.SetAsLastSibling ();

		}
	}


}
