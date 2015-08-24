using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public Button FavoriteOn;
    public Button FavoriteOff;

    public RawImage rawImage;
    public Text title;

    private bool isFavorite;

	void Start () {
        rawImage.texture = Data.Instance.lastArtTexture;
        title.text = Data.Instance.artData.selectedArtWork.title;
		print (Data.Instance.artData.selectedArtWork.gallery);
		if (Data.Instance.artData.selectedArtWork.gallery.Equals("My Artworks")) {
			FavoriteOn.gameObject.SetActive(false);
			FavoriteOff.gameObject.SetActive(false);
		} else {
			if (Data.Instance.artData.isFavorite (Data.Instance.artData.selectedArtWork.galleryId, Data.Instance.artData.selectedArtWork.artId))
				isFavorite = true;            
			SetFavorite ();
		}


    }
    public void Confirm()
    {
        if (Data.Instance.lastPhotoTexture == null)
            Data.Instance.LoadLevel("LoadRoom");
        else
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
    public void SwitchFavorites()
    {
        isFavorite = !isFavorite;
        if (isFavorite)
            Data.Instance.artData.AddToFavorites();
        else
            Data.Instance.artData.RemoveFromFavorites();
        SetFavorite();
    }
    private void SetFavorite()
    {
        if (isFavorite)
        {
            FavoriteOn.gameObject.SetActive(true);
            FavoriteOff.gameObject.SetActive(false);
        }
        else
        {
            FavoriteOn.gameObject.SetActive(false);
            FavoriteOff.gameObject.SetActive(true);
        }
    }
}
