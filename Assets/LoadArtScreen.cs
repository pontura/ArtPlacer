using UnityEngine;
using System.Collections;
using ImageVideoContactPicker;

public class LoadArtScreen : MonoBehaviour {

	void Start()
	{		
		PickerEventListener.onImageLoad += OnImageLoad;
	}

	public void TakePhoto()
    {
		Data.Instance.isPhoto4Room = false;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
		Debug.Log ("Aca");
		#if UNITY_ANDROID
		AndroidPicker.BrowseImage();
		#elif UNITY_IPHONE
		IOSPicker.BrowseImage();
		#endif  
    }    
    public void Back()
    {
        Data.Instance.Back();
    }

	public void OnImageLoad(string imgPath, Texture2D tex){
		Data.Instance.isPhoto4Room = false;
		Data.Instance.lastArtTexture = tex;		 
		Data.Instance.LoadLevel("ConfirmPhoto");
	}
	
	void OnDestroy(){
		PickerEventListener.onImageLoad -= OnImageLoad;
	}
}

