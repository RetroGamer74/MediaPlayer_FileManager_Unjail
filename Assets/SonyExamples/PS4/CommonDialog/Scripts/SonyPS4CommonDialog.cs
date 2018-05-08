using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SonyPS4CommonDialog : MonoBehaviour
{
    MenuStack menuStack;
	float waitTime = 0;
	float progressDelay = 0;
	float progressTime = 0;
	string imeText = "";

	public InputField RenameFileName;
	public bool openKeyboardOnScreen = false;

	MenuLayout menuMain;
	MenuLayout menuUserMessage;
	MenuLayout menuSystemMessage1;
	MenuLayout menuErrorMessage;
	MenuLayout menuProgress;

	void Start()
	{

		Sony.PS4.Dialog.Main.OnLog += OnLog;
		Sony.PS4.Dialog.Main.OnLogWarning += OnLogWarning;
		Sony.PS4.Dialog.Main.OnLogError += OnLogError;

		Sony.PS4.Dialog.Common.OnGotDialogResult += OnGotDialogResult;
		Sony.PS4.Dialog.Ime.OnGotIMEDialogResult += OnGotIMEDialogResult;

		Sony.PS4.Dialog.Main.Initialise();
	}

	public void OnEnter() {}
	public void OnExit() {}


	public void ShowKeyboardOnScreen()
	{
		Sony.PS4.Dialog.Ime.SceImeDialogParam ImeParam = new Sony.PS4.Dialog.Ime.SceImeDialogParam();
		Sony.PS4.Dialog.Ime.SceImeParamExtended ImeExtendedParam = new Sony.PS4.Dialog.Ime.SceImeParamExtended();

		// Set supported languages, 'or' flags together or set to 0 to support all languages.
		ImeParam.supportedLanguages = Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_JAPANESE |
			Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_ENGLISH_GB |
			Sony.PS4.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_DANISH;

		ImeParam.option = Sony.PS4.Dialog.Ime.Option.DEFAULT;
		ImeParam.title = "Keyboard";
		ImeParam.maxTextLength = 255;
		ImeParam.inputTextBuffer = RenameFileName.text;

		Sony.PS4.Dialog.Ime.Open(ImeParam, ImeExtendedParam);

	}

	public void KeyboardOnScreen()
	{
		openKeyboardOnScreen = true;
	}

    void OnGotIMEDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
    {
		Sony.PS4.Dialog.Ime.ImeDialogResult result = Sony.PS4.Dialog.Ime.GetResult();
		RenameFileName.text = result.text;
		/*TO DO . HERE WE HAVE TO SELECT WHAT IS THE DESTINATION TEXT UI  */
		/* FOR THIS SAMPLE I WILL SET TO THE RENAME FUNCTION */

        /*OnScreenLog.Add("IME result: " + result.result);
        OnScreenLog.Add("IME button: " + result.button);
        OnScreenLog.Add("IME text: " + result.text);
        */
		if (result.result == Sony.PS4.Dialog.Ime.EnumImeDialogResult.RESULT_OK)
		{
			imeText = result.text;
			openKeyboardOnScreen = false;
		}

		SetRenameFileNameTextWithIMEKeyboardData ();
    }

	public void SetRenameFileNameTextWithIMEKeyboardData()
	{
		RenameFileName.text = imeText;

	}

	void OnLog(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{

	}

	void OnLogWarning(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{

	}

	void OnLogError(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{

	}

	void OnGotDialogResult(Sony.PS4.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS4.Dialog.Common.CommonDialogResult result = Sony.PS4.Dialog.Common.GetResult ();
	}

	void Update()
	{
		Sony.PS4.Dialog.Main.Update ();
	}

	void OnGUI()
	{
		if (openKeyboardOnScreen)
			ShowKeyboardOnScreen ();
			
	}

}
