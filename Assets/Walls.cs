using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Walls : MonoBehaviour {

    public Text field;
    public RawImage rawImage;
    public RawImage rawImage2;

	void Start () {
        rawImage.texture = Data.Instance.lastPhotoTexture;
	}
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void Refresh()
    {
        var filePath = Data.Instance.imagePath;
        if (System.IO.File.Exists(filePath))
        {
            field.text += " EXISTS!!!";
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            rawImage2.material.mainTexture = tex;
        }
    }
}
