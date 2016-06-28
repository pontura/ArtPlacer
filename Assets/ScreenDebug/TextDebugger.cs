using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextDebugger : MonoBehaviour {

	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		ScreenDebugger.Log += Log;
	}

	void OnDestroy()
	{
		ScreenDebugger.Log -= Log;
	}

	public void Log(string log){		
		text.text = "Log: " + log;
	}
}
