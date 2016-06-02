using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using NatCamU;

public class WebCamPhotoCamera : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage rawImage;
    public Button takePhotoButton;
    private bool photoTaken;
    private bool ready;

    void Start()
    {
        if (Data.Instance.isPhoto4Room) 
            Data.Instance.lastPhotoTexture = null;
        else 
            Data.Instance.lastArtTexture = null;

        //webCamTexture = new WebCamTexture();
        //webCamTexture.requestedHeight = 1280;
        //webCamTexture.requestedWidth = 720;
		/*webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, (int)Data.Instance.defaultCamSize.x, (int)Data.Instance.defaultCamSize.y, 30);
		if (webCamTexture.isPlaying)
		{
			webCamTexture.Stop();
		} else
			webCamTexture.Play();
		*/

		NatCam.Initialize ();
		NatCam.PhotoSaveMode = PhotoSaveMode.SaveToAppAlbum;

		NatCam.Play (DeviceCamera.RearCamera);
		NatCam.ExecuteOnPreviewStart (() => rawImage.texture = NatCam.Preview);

        Vector3 scale = rawImage.transform.localScale;
        
#if UNITY_IOS
        scale.x *= -1;
       rawImage.transform.localEulerAngles = new Vector3(0, 0, 180);
#endif
        rawImage.transform.localScale = scale;

		Events.OnLoading (false);
    }
    void Update()
    {
        if (ready) return;
        if (photoTaken)
        {
            if (Data.Instance.isPhoto4Room && Data.Instance.lastPhotoTexture != null)
                Ready();
            else if (!Data.Instance.isPhoto4Room && Data.Instance.lastArtTexture != null)
                Ready();
        }
        /*else
        {
            rawImage.texture = webCamTexture;
        }*/
    }
    void Ready()
    {
        ready = true;
        Data.Instance.LoadLevel("ConfirmPhoto");
    }
    void OnDestroy()
    {
        //webCamTexture.Stop();
		NatCam.Release();
    }
    public void TakePhoto()
    {
		Events.OnLoading (true);
        photoTaken = true;
        takePhotoButton.gameObject.SetActive(false);

        if (Data.Instance.isPhoto4Room)
        {
			/*if (Input.deviceOrientation == DeviceOrientation.Portrait){
				Texture2D temp = new Texture2D(webCamTexture.width, webCamTexture.height);
				temp.SetPixels(webCamTexture.GetPixels());
				temp.Apply();				
				Texture2D rotated = TextureUtils.Rotate90CW(temp);
				Data.Instance.lastPhotoTexture = new Texture2D(rotated.width, rotated.height);
				Data.Instance.lastPhotoTexture.SetPixels(rotated.GetPixels());
			}else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown){
				Texture2D temp = new Texture2D(webCamTexture.width, webCamTexture.height);
				temp.SetPixels(webCamTexture.GetPixels());
				temp.Apply();				
				Texture2D rotated = TextureUtils.Rotate90CCW(temp);
				Data.Instance.lastPhotoTexture = new Texture2D(rotated.width, rotated.height);
				Data.Instance.lastPhotoTexture.SetPixels(rotated.GetPixels());
			}else{
            	Data.Instance.lastPhotoTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
            	Data.Instance.lastPhotoTexture.SetPixels(webCamTexture.GetPixels());
			}
            Data.Instance.lastPhotoTexture.Apply();*/

			NatCam.CapturePhoto(
				(Texture2D photo) => {
					//Set a material's main texture to be the captured photo
					Data.Instance.lastPhotoTexture = photo;
					//We don't need to manually unregister this delegate
				});
        }
        else
        {
			/*if (Input.deviceOrientation == DeviceOrientation.Portrait){
				Texture2D temp = new Texture2D(webCamTexture.width, webCamTexture.height);
				temp.SetPixels(webCamTexture.GetPixels());
				temp.Apply();				
				Texture2D rotated = TextureUtils.Rotate90CW(temp);
				Data.Instance.lastArtTexture = new Texture2D(rotated.width, rotated.height);
				Data.Instance.lastArtTexture.SetPixels(rotated.GetPixels());
			}else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown){
				Texture2D temp = new Texture2D(webCamTexture.width, webCamTexture.height);
				temp.SetPixels(webCamTexture.GetPixels());
				temp.Apply();				
				Texture2D rotated = TextureUtils.Rotate90CCW(temp);
				Data.Instance.lastArtTexture = new Texture2D(rotated.width, rotated.height);
				Data.Instance.lastArtTexture.SetPixels(rotated.GetPixels());
			}else{
            	Data.Instance.lastArtTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
            	Data.Instance.lastArtTexture.SetPixels(webCamTexture.GetPixels());
			}
            Data.Instance.lastArtTexture.Apply();*/

			NatCam.CapturePhoto(
				(Texture2D photo) => {
					//Set a material's main texture to be the captured photo
					Data.Instance.lastArtTexture = photo;
					//We don't need to manually unregister this delegate
			});
        }

      //  Data.Instance.LoadLevel("ConfirmPhoto");
        
    }

}