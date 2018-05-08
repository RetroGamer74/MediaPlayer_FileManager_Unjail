using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class PS4Unjail : MonoBehaviour
{
	public Button unjailButton;

	[DllImport("libPS4Unjail")]
	private static extern int FreeUnjail();

	[DllImport("libPS4Unjail")]
	private static extern int GetPid();

	[DllImport("libPS4Unjail")]
	private static extern int GetUid();

	public void Unjail()
	{

		unjailButton.interactable = false;
		FreeUnjail ();
	}

}
