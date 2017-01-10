using UnityEngine;
using System.Collections;

public class PicoLanguage : MonoBehaviour {

	public static PicoLanguage Instance { get; private set; }
	
	public Font MicrosoftYH;
	public Font Arial;

	void Awake ()
	{
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}
		Instance = this;
	}

	// Use this for initialization
	void Start (){
		GetSystemLanguage ();
	}
	
	
	void GetSystemLanguage (){
		string systemLanguage = "zh";
		//PicoUnityActivity.CallObjectMethod<string> (ref systemLanguage, "getLanguage");
		SetLanguage (systemLanguage);
	}
	
	public void Android_SendLanguageToUnity (string language){
		Debug.Log ("current android system language is " + language);
		SetLanguage (language);
	}
	
	void SetLanguage (string systemLanguage){
		string language = "English";
		switch (systemLanguage) {
		case "zh":
			language = "Chinese";
			break;
		case "en":
			language = "English";
			break;
		case "ja":
			language = "Japanese";
			break;
		default:
			language = "Enlish";
			break;
		}
		Localization.language = language;
	}


}
