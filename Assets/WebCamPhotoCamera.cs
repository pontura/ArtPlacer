using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WebCamPhotoCamera : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage rawImage;
    public Button takePhotoButton;

    public Quaternion baseRotation;

    void Start()
    {
        webCamTexture = new WebCamTexture();

        baseRotation = transform.rotation;

        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        } else
        webCamTexture.Play();
    }
    void Update()
    {
        transform.rotation = baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.up);
        rawImage.texture = webCamTexture;
    }
    void OnDestroy()
    {
        webCamTexture.Stop();
    }
    public void TakePhoto()
    {        
        takePhotoButton.gameObject.SetActive(false);

        Data.Instance.lastPhotoTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
        Data.Instance.lastPhotoTexture.SetPixels(webCamTexture.GetPixels());
        Data.Instance.lastPhotoTexture.Apply();


        Data.Instance.LoadLevel("ConfirmPhoto");
        
    }

}