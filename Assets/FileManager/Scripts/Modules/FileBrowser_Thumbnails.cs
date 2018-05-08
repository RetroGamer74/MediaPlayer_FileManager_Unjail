using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowser_Thumbnails : MonoBehaviour
{
    #region Variables

    public int _tThumbnailsSize = 128;
    private string[] _tThumbnailsFilter = { "png", "jpg", "jpeg" };
    private Thread_Thumbnails _tThread;
    private Texture2D _tTextureHigh;
    private Texture2D _tTextureLow;

    #endregion

    #region Functions

    public void GenerateThumbnails(FileBrowser_Button tButton)
    {
        CancelGeneration();
        StartCoroutine(Coroutine_GenerateThumbnails(tButton));
    }

    void CancelGeneration()
    {
        if (_tThread != null)
        {
            _tThread.Abort();
            _tThread = null;
            _tTextureHigh = null;
            _tTextureLow = null;
        }

        if (_tTextureHigh != null)
        {
            DestroyImmediate(_tTextureHigh);
            _tTextureHigh = null;
        }

        if (_tTextureLow != null)
        {
            DestroyImmediate(_tTextureLow);
            _tTextureLow = null;
        }

        StopAllCoroutines();
    }

    IEnumerator Coroutine_GenerateThumbnails(FileBrowser_Button tButton)
    {
        FileInfo tInfo = new FileInfo(tButton._sPath);

        if( tInfo.Exists )
        {
            // Check if file has the correct extension

            bool bOK = false;

            for( int j = 0; j < _tThumbnailsFilter.Length; j++ )
            {
                if (tInfo.Extension.ToLower().Contains(_tThumbnailsFilter[j].ToLower()))
                    bOK = true;
            }

            if(bOK)
            {
                // Retrieve file asyncronously

                WWW tWWW = new WWW("file://" + tButton._sPath);

                while (!tWWW.isDone)
                    yield return null;

                if (string.IsNullOrEmpty(tWWW.error))
                {
                    _tTextureHigh = tWWW.texture;

                    if (_tTextureHigh != null)
                    {
                        // Adapat thumbnail size to texture ratio

                        int iW = _tThumbnailsSize;
                        int iH = _tThumbnailsSize;

                        if (_tTextureHigh.width < _tTextureHigh.height)
                            iW = (int)((float)_tTextureHigh.width / _tTextureHigh.height * _tThumbnailsSize);
                        else if (_tTextureHigh.height < _tTextureHigh.width)
                            iH = (int)((float)_tTextureHigh.height / _tTextureHigh.width * _tThumbnailsSize);

                        // Start thread to resize image to thumbnail size

                        _tThread = new Thread_Thumbnails(_tTextureHigh, iW, iH);
                        _tThread.Start();

                        yield return StartCoroutine(_tThread.WaitFor());

                        // Set image as sprite for the UI

                        _tTextureLow = _tThread._tResult;

                        tButton._tSubImage.sprite = Sprite.Create(_tTextureLow, new Rect(0, 0, iW, iH), Vector2.one * 0.5f);
                        tButton._tSubImage.color = Color.white;

                        _tThread.Abort();
                        _tThread = null;
                        _tTextureHigh = null;
                        _tTextureLow = null;
                    }
                }
            }

            yield return null;
        }
    }

    #endregion
}
