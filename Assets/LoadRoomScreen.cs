using UnityEngine;
using System.Collections;
using ImageVideoContactPicker;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadRoomScreen : MonoBehaviour {

    public GameObject openButton;

    void Start()
    {
       // Data.Instance.SetMainMenuActive(false);
        if (Data.Instance.roomsData.rooms.Count == 0)
            openButton.SetActive(false);

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
		Data.Instance.lastPhotoTexture = tex;		 
		Data.Instance.LoadLevel("ConfirmPhoto");
	}

	void OnDestroy(){
		PickerEventListener.onImageLoad -= OnImageLoad;
	}
}

