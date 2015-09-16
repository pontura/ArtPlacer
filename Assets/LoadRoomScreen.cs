using UnityEngine;
using System.Collections;
using ImageVideoContactPicker;

public class LoadRoomScreen : MonoBehaviour {
	  

    void Start()
    {
		PickerEventListener.onImageLoad += OnImageLoad;
    }

    public void TakePhoto()
    {
		Data.Instance.isPhoto4Room = true;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
        Data.Instance.roomsData.type = RoomsData.types.ONLINE;
        Data.Instance.LoadLevel("Rooms");        
    }
    public void Open()
	{	
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
		Data.Instance.lastPhotoTexture = tex;		 
		Data.Instance.LoadLevel("ConfirmPhoto");
	}

	void OnDestroy(){
		PickerEventListener.onImageLoad -= OnImageLoad;
	}
}

