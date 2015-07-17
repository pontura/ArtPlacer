using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public RawImage rawImage;

	void Start () {
        rawImage.texture = Data.Instance.lastArtTexture;
	}
    public void Confirm()
    {
        //Data.Instance.SavePhotoTaken();
        Data.Instance.LoadLevel("ArtPlaced");
    }
    public void Back()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
