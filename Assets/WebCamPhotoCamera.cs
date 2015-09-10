using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

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
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, 1280, 720, 30);

        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        } else
        webCamTexture.Play();

        Vector3 scale = rawImage.transform.localScale;
        
#if UNITY_IOS
        scale.x *= -1;
       rawImage.transform.localEulerAngles = new Vector3(0, 0, 180);
#endif
        rawImage.transform.localScale = scale;
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
        else
        {
            rawImage.texture = webCamTexture;
        }
    }
    void Ready()
    {
        ready = true;
        Data.Instance.LoadLevel("ConfirmPhoto");
    }
    void OnDestroy()
    {
        webCamTexture.Stop();
    }
    public void TakePhoto()
    {
        photoTaken = true;
        takePhotoButton.gameObject.SetActive(false);

        if (Data.Instance.isPhoto4Room)
        {
            Data.Instance.lastPhotoTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
            Data.Instance.lastPhotoTexture.SetPixels(webCamTexture.GetPixels());
            Data.Instance.lastPhotoTexture.Apply();
        }
        else
        {
            Data.Instance.lastArtTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
            Data.Instance.lastArtTexture.SetPixels(webCamTexture.GetPixels());
            Data.Instance.lastArtTexture.Apply();
        }

      //  Data.Instance.LoadLevel("ConfirmPhoto");
        
    }

}