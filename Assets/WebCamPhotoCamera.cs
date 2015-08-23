using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WebCamPhotoCamera : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage rawImage;
    public Button takePhotoButton;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        } else
        webCamTexture.Play();
    }
    void Update()
    {
        rawImage.texture = webCamTexture;
    }
    void OnDestroy()
    {
        webCamTexture.Stop();
    }
    public void TakePhoto()
    {        
        
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

        Data.Instance.LoadLevel("ConfirmPhoto");
        
    }

}