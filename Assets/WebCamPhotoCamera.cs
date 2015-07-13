﻿using UnityEngine;
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
        print("WebCamPhotoCamera Start");
        webCamTexture = new WebCamTexture();
        if (webCamTexture.isPlaying)
        {
            print("ASSAD");
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

        Data.Instance.lastPhotoTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
        Data.Instance.lastPhotoTexture.SetPixels(webCamTexture.GetPixels());
        Data.Instance.lastPhotoTexture.Apply();

       
        //Encode to a PNG
        byte[] bytes = Data.Instance.lastPhotoTexture.EncodeToPNG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible

        string path = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        Data.Instance.imagePath = path;

        var filePath = Data.Instance.GetImagesPath(Data.Instance.imagePath);
        File.WriteAllBytes(filePath, bytes);
        Data.Instance.LoadLevel("Walls");
    }

}