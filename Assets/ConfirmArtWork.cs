using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public RawImage rawImage;
    public Text title;

	void Start () {
        rawImage.texture = Data.Instance.lastArtTexture;
        title.text = Data.Instance.artData.selectedArtWork.title;
    }
    public void Confirm()
    {
        //Data.Instance.SavePhotoTaken();
        Data.Instance.LoadLevel("ArtPlaced");
    }
    public void Back()
    {
        Data.Instance.LoadLevel("Artworks");
    }
    public void Galleries()
    {
        Data.Instance.LoadLevel("Galleries");
    }
}
