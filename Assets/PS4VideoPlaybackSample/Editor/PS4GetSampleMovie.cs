using UnityEngine;
using UnityEditor;
using System.IO;

public class PS4GetSampleMovie : MonoBehaviour
{
    [MenuItem("PlayStation 4/Video Sample/Get Sample Movie")]
    public static void GetSampleMovie()
    {
        ReplaceFile("InputManagerDefault");
    }

	static void ReplaceFile(string fileName)
	{
		string sourceFile = Path.Combine(System.Environment.GetEnvironmentVariable("SCE_ORBIS_SAMPLE_DIR"), "data/audio_video/video/demo01.mp4");
        if (!File.Exists (sourceFile))
        {
			Debug.LogErrorFormat ("Source file does not exist! Tried to find: {0}", sourceFile);
			return;
		}
		
		string targetFile = Path.Combine(Application.streamingAssetsPath, "Movies/demo01.mp4");
		if (File.Exists (targetFile))
        {
			Debug.LogErrorFormat("Target file '{0}' already exists! Manually delete this first.", targetFile);
            return;
		}
		
		File.Copy (sourceFile, targetFile);
		AssetDatabase.Refresh();
		
		Debug.Log("Sample movie 'demo01.mp4' successfully copied!");
	}
}
