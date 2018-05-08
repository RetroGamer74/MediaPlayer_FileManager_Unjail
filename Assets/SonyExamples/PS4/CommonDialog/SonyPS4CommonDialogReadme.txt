
Unity SCE Common Dialog Example Project.

This example project demonstrates how to use the Unity SCE Common Dialog API for displaying and retrieving results from user, system, progress, error and text entry (IME) dialogs.

Project Folder Structure

	Plugins/PS4 - Contains the CommonDialog native plugin.
	SonyAssemblies - Contains the SonyPS4CommonDialog managed interface to the CommonDialog plugin.
	SonyExample/CommonDialog - Contains a Unity scene which runs the scripts.
	SonyExample/CommonDialog/Scripts - Contains the Sony NP example scripts.
	SonyExample/Utils - Contains various utility scripts for use by the example.

The SonyPS4CommonDialog managed assembly defines the following namespaces...

Sony.PS4.Dialog.Main		Contains methods for initialising and updating the plugin.
Sony.PS4.Dialog.Common		Contains methods for working with the SCE Common Dialog for user, system, progress, and error messages.
Sony.PS4.Dialog.Ime			Contains methods for working with the SCE IME Dialog for text entry.
Sony.PS4.Dialog.Messages	Contains methods for communicating messages from the common dialog system back to scripts.

Sony.PS4.Dialog.Main

	Methods.

		public static void Initialise()
		Initialises the plugin, call once.

		public static void Update()
		Updates the plugin, call once each frame.

Sony.PS4.Dialog.Common

	Enumerations.

		System message dialog types, these are a one-one match with the values defined by SceMsgDialogSystemMessageType.
		public enum SystemMessageType
		{
			TRC_EMPTY_STORE,
			TRC_PSN_CHAT_RESTRICTION,
			TRC_PSN_UGC_RESTRICTION,
			WARNING_SWITCH_TO_SIMULVIEW,
			CAMERA_NOT_CONNECTED,
			WARNING_PROFILE_PICTURE_AND_NAME_NOT_SHARED
		}

		User message dialog types, these are a one-one match with the values defined by SceMsgDialogButtonType.
		public enum UserMessageType
		{
			OK,
			YESNO,
			NONE,
			OK_CANCEL,
			CANCEL
		}

		Dialog result, the button or action that resulted in the dialog closing.
		public enum CommonDialogResult
		{
			RESULT_BUTTON_NOT_SET,
			RESULT_BUTTON_OK,
			RESULT_BUTTON_CANCEL,
			RESULT_BUTTON_YES,
			RESULT_BUTTON_NO,
			RESULT_BUTTON_1,
			RESULT_BUTTON_2,
			RESULT_BUTTON_3,
			RESULT_CANCELED,
			RESULT_ABORTED,
			RESULT_CLOSED,
		}


	Events.

		OnGotDialogResult		Triggered when a dialog has closed and the result is available.

	Properties.

		public static bool IsDialogOpen
		Is a dialog open?

	Methods.

		public static bool ShowErrorMessage(UInt32 errorCode)
		Display an error message.

		public static bool ShowSystemMessage(SystemMessageType type, bool infoBar, int value)
		Display a system message.

		public static bool ShowProgressBar(string message)
		Display a progress bar.

		public static bool SetProgressBarPercent(int percent)
		Set progress bar percentage (0-100).

		public static bool SetProgressBarMessage(string message)
		Set progress bar message string.

		public static bool ShowUserMessage(UserMessageType type, bool infoBar, string str)
		Show a user message.
				
		public static bool Close()
		Close the dialog.

		public static CommonDialogResult GetResult()
		Get the result from the dialog that's just closed.

Sony.PS4.Dialog.Ime

	Enumerations.

		ImeParam enterLabel
		public enum EnterLabel
		{
			DEFAULT,
			SEND,
			LABEL_SEARCH,
			LABEL_GO
		}
				
		ImeParam type
		public enum Type
		{
			DEFAULT,
			BASIC_LATIN,
			URL,
			MAIL,
			NUMBER
		}

		Dialog result.
		public enum EnumImeDialogResult
		{
			RESULT_OK,				User selected either close button or Enter button
			RESULT_USER_CANCELED,	User performed cancel operation.
			RESULT_ABORTED,			IME Dialog operation has been aborted.
		}
		
		Dialog result button.
		public enum EnumImeDialogResultButton
		{
			BUTTON_NONE,	IME Dialog operation has been aborted or canceled.
			BUTTON_CLOSE,	User selected close button
			BUTTON_ENTER,	User selected Enter button
		}





	Events.

		OnGotIMEDialogResult	Triggered when the dialog has closed and the result is ready.

	Properties.

		public static bool IsDialogOpen
		Is the IME dialog open?

	Methods.

		public static bool Open(ImeDialogParams info)
		Opens the IME dialog.
				
		public static ImeDialogResult GetResult()
		Gets the IME dialog result.

