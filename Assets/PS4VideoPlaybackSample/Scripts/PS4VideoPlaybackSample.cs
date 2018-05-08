using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PS4VideoPlaybackSample : MonoBehaviour
{
    [Header("Movie (within Assets/StreamingAssets)")]
    [Tooltip("For use with the Video Playback functionality you must place the video inside your StreamingAssets path.")]
	public string moviePath = "Movies/demo01.mp4"; // See "%SCE_ORBIS_SAMPLE_DIR%\data\audio_video\video"

    [Space(10)]
    [Header("UI Elements")]
    public Image videoImage;
    public Slider timelineSlider;
    public Text timelineCurrentDisplay;
    public Text timelineTotalDisplay;
    public Sprite playIcon;
    public Sprite pauseIcon;
    public Sprite audioIcon;
    public Sprite muteIcon;
    public Image playPauseIcon;
    public Image audioToggleIcon;
    public Image loopingIcon;
    public Text outputText;

#if UNITY_PS4
    PS4VideoPlayer video;
    PS4ImageStream lumaTex;
    PS4ImageStream chromaTex;
    bool isMuted = false;
    PS4VideoPlayer.Looping isLooping = PS4VideoPlayer.Looping.None;
#endif

    void Start()
    {
#if UNITY_PS4
        // Please set a movie to play!
        if(string.IsNullOrEmpty(moviePath))
        {
            Debug.LogError("Movie Path is null or empty! Please set this from within the Editor!");
            enabled = false;
            return;
        }

        // You cannot use back-slashes on PS4, although they work on PC-Hosted they will fail on an installed package
        if (moviePath.Contains("\\"))
        {
            Debug.LogError("Movie Path uses back-slashes! Replacing with forward-slashes instead.");
            moviePath = moviePath.Replace("\\", "/");
        }

        moviePath = Path.Combine(Application.streamingAssetsPath, moviePath);

        if(!File.Exists(moviePath))
        {
            Debug.LogError("Movie could not be found! Unable to play.");
            enabled = false;
            return;
        }

        AddToOutputText(string.Format("Movie path for playback is: {0}", moviePath));

#if UNITY_5_4_OR_NEWER
        // In 5.3 this event is triggered automatically, as an optimization in 5.4 onwards you need to register the callback
        PS4VideoPlayer.OnMovieEvent += OnMovieEvent;
#endif

        video = new PS4VideoPlayer(); // This sets up a VideoDecoderType.DEFAULT system
        video.PerformanceLevel = PS4VideoPlayer.Performance.Optimal;
        video.demuxVideoBufferSize = 2 * 1024 * 1024; // Change the demux buffer from it's 1mb default
        video.numOutputVideoFrameBuffers = 2; // Increasing this can stop frame stuttering

        lumaTex = new PS4ImageStream();
        lumaTex.Create(1920, 1080, PS4ImageStream.Type.R8, 0);
        chromaTex = new PS4ImageStream();
        chromaTex.Create(1920 / 2, 1080 / 2, PS4ImageStream.Type.R8G8, 0);
        video.Init(lumaTex, chromaTex);

        // Apply video textures to the UI image
        videoImage.material.SetTexture("_MainTex", lumaTex.GetTexture());
        videoImage.material.SetTexture("_CromaTex", chromaTex.GetTexture());
#endif
    }

    void Update()
    {
#if UNITY_PS4
        // Required to keep the video processing
        video.Update();

        if(video.playerState > PS4VideoPlayer.VidState.READY)
        {
            // The video CurrentTime and Length will return 0 if the video player is not in a valid active state
            DateTime videoCurrentTime = new DateTime(video.CurrentTime * 10000);
            DateTime videoLength = new DateTime(video.Length * 10000);

            // Display the current time on a slider bar
            timelineSlider.value = videoCurrentTime.Ticks;
            timelineSlider.maxValue = videoLength.Ticks;

            // Display the current time and the total time in text form
            timelineCurrentDisplay.text = videoCurrentTime.ToString("HH:mm:ss:fff");
            timelineTotalDisplay.text = videoLength.ToString("HH:mm:ss:fff");
        }

        CropVideo();
#endif
    }

    void CropVideo()
    {
#if UNITY_PS4
        // The video player on the PS4 frequently generates video on larger textures than the video, and requires us to crop the video. This code calculates
        // the crop values and passes the data on to the TRANSFORM_TEX call in the shader, without it we get nasty black borders at the edge of video
        if (videoImage != null)
        {
            int cropleft, cropright, croptop, cropbottom, width, height;
            video.GetVideoCropValues(out cropleft, out cropright, out croptop, out cropbottom, out width, out height);
            float scalex = 1.0f;
            float scaley = 1.0f;
            float offx = 0.0f;
            float offy = 0.0f;

            if ((width > 0) && (height > 0))
            {
                int fullwidth = width + cropleft + cropright;
                scalex = (float)width / (float)fullwidth;
                offx = (float)cropleft / (float)fullwidth;
                int fullheight = height + croptop + cropbottom;
                scaley = (float)height / (float)fullheight;
                offy = (float)croptop / (float)fullheight;
            }

            // Typically we want to invert the Y on the video because thats how planes UV's are layed out
            videoImage.material.SetVector("_MainTex_ST", new Vector4(scalex, scaley * -1, offx, 1 - offy));
        }
#endif
    }

    public void VideoPlayPause()
    {
#if UNITY_PS4
        // Pause if playing, Resume if paused, Play if stopped
        if (video.playerState == PS4VideoPlayer.VidState.PLAY)
        {
            video.Pause();
            playPauseIcon.sprite = playIcon;

            AddToOutputText("Video was playing, now paused");
        }
        else if(video.playerState == PS4VideoPlayer.VidState.PAUSE)
        {
            video.Resume();
            playPauseIcon.sprite = pauseIcon;

            AddToOutputText("Video was paused, now resumed");
        }
        else
        {
            video.Play(moviePath, isLooping);
            playPauseIcon.sprite = pauseIcon;

            AddToOutputText("Video was stopped, now playing");
        }

        // Calling Play ignores the current mute settings. This reapplies them
        video.SetVolume(isMuted ? 0 : 100);
#endif
    }

    // Stop playback and reset the current time displays to zero
    public void VideoStop()
    {
#if UNITY_PS4
        video.Stop();
        playPauseIcon.sprite = playIcon;
        timelineCurrentDisplay.text = "00:00:00:000";
        timelineSlider.value = 0;

        AddToOutputText("Playback stopped");
#endif
    }

    // Jump forwards 1000ms
    public void VideoFastForward()
    {
#if UNITY_PS4
        video.JumpToTime(video.GetCurrentTime() + 1000);

        AddToOutputText("Fast-forwarded 1 second");
#endif
    }

    // Jump backwards 1000ms
    public void VideoRewind()
    {
#if UNITY_PS4
        video.JumpToTime(video.GetCurrentTime() - 1000);

        AddToOutputText("Rewinded 1 second");
#endif
    }

    // Change the volume to make the video muted
    public void VideoToggleMute()
    {
#if UNITY_PS4
        if (isMuted)
        {
            video.SetVolume(100);
            audioToggleIcon.sprite = audioIcon;
        }
        else
        {
            video.SetVolume(0);
            audioToggleIcon.sprite = muteIcon;
        }

        isMuted = !isMuted;

        AddToOutputText("Mute state is now set to: " + isMuted);
#endif
    }

    // Toggle looping. Note that this only takes effect after starting playback, and doesn't affect a currently playing video
    public void VideoToggleLooping()
    {
#if UNITY_PS4
        if(isLooping == PS4VideoPlayer.Looping.None)
        {
            isLooping = PS4VideoPlayer.Looping.Continuous;
            loopingIcon.color = Color.white;
        }
        else
        {
            isLooping = PS4VideoPlayer.Looping.None;
            loopingIcon.color = new Color(1, 1, 1, 0.25f);
        }

        AddToOutputText("Looping is now set to: " + isLooping);
#endif
    }

    void OnMovieEvent(int FMVevent)
    {
#if UNITY_PS4
        AddToOutputText("Script has received FMV event: " + (PS4VideoPlayer.Event)FMVevent);
#endif
    }

    void AddToOutputText(string message)
    {
        outputText.text = Time.frameCount + ": " + message + "\n" + outputText.text;
    }
}
