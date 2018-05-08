using UnityEngine;
using System.Collections;
using System.IO;

public class Thread_Thumbnails : ThreadedJob
{
    #region Variables

    private Texture2D _tTextureHigh;
    private int _iWidth;
    private int _iHeight;

    private int _iBaseWidth;
    private int _iBaseHeight;
    private Color[] _tTextureBase;
    private Color[] _tTextureResized;

    public Texture2D _tResult;

    #endregion

    #region Functions

    public Thread_Thumbnails(Texture2D tTextureHigh, int iWidth, int iHeight)
    {
        _tTextureHigh = tTextureHigh;
        _iWidth = iWidth;
        _iHeight = iHeight;

        _iBaseWidth = _tTextureHigh.width;
        _iBaseHeight = _tTextureHigh.height;
        _tTextureBase = tTextureHigh.GetPixels();
        _tTextureResized = new Color[iWidth * iHeight];
    }

    protected override void ThreadFunction()
    {
        float fRatioX = 1.0f / ((float)_iWidth / (_iBaseWidth - 1));
        float fRatioY = 1.0f / ((float)_iHeight / (_iBaseHeight - 1));

        for (int y = 0; y < _iHeight; y++)
        {
            int iYFloor = (int)Mathf.Floor(y * fRatioY);
            float fYLerp = y * fRatioY - iYFloor;

            int iY1 = iYFloor * _iBaseWidth;
            int iY2 = (iYFloor + 1) * _iBaseWidth;
            int iYw = y * _iWidth;

            for (var x = 0; x < _iWidth; x++)
            {
                int iXFloor = (int)Mathf.Floor(x * fRatioX);
                float fXLerp = x * fRatioX - iXFloor;

                // Bilinear filtering
                _tTextureResized[iYw + x] = Color.Lerp(Color.Lerp(_tTextureBase[iY1 + iXFloor], _tTextureBase[iY1 + iXFloor + 1], fXLerp),
                                                       Color.Lerp(_tTextureBase[iY2 + iXFloor], _tTextureBase[iY2 + iXFloor + 1], fXLerp),
                                                       fYLerp);
            }
        }
    }

    protected override void OnFinished()
    {
        // Create result texture

        _tResult = new Texture2D(_iWidth, _iHeight);
        _tResult.SetPixels(_tTextureResized);
        _tResult.Apply();
    }

    public override void Abort()
    {
        base.Abort();

        Texture2D.DestroyImmediate(_tTextureHigh);
        _tResult = null;
        _tTextureBase = null;
        _tTextureResized = null;
    }

    private Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                         c1.g + (c2.g - c1.g) * value,
                         c1.b + (c2.b - c1.b) * value,
                         c1.a + (c2.a - c1.a) * value);
    }

    #endregion
}
