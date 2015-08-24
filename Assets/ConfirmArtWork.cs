using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public Button FavoriteOn;
    public Button FavoriteOff;
	public Button PlaceItButton;

    public RawImage rawImage;
    public Text title;

    private bool isFavorite;

	void Start () {
        rawImage.texture = Data.Instance.lastArtTexture;
        title.text = Data.Instance.artData.selectedArtWork.title;
		if (Data.Instance.artData.selectedArtWork.galleryId == -2 ) {
			FavoriteOn.gameObject.SetActive(false);
			FavoriteOff.gameObject.SetActive(false);
		} else {
			if (Data.Instance.artData.isFavorite (Data.Instance.artData.selectedArtWork.galleryId, Data.Instance.artData.selectedArtWork.artId))
				isFavorite = true;            
			SetFavorite ();
		}

		if (Data.Instance.isArtworkInfo2Place == false)
			PlaceItButton.GetComponentInChildren<Text> ().text = "BACK TO WALLS";
    }
    public void Confirm()
    {
        if (Data.Instance.lastPhotoTexture == null)
			Data.Instance.LoadLevel ("LoadRoom");
		else {
			if(Data.Instance.isArtworkInfo2Place == false)Data.Instance.lastArtTexture = null;
			Data.Instance.LoadLevel ("ArtPlaced");
		}
    }
	public void Back2Walls()
	{
		if(Data.Instance.isArtworkInfo2Place == false)Data.Instance.lastArtTexture = null;
		Data.Instance.LoadLevel ("ArtPlaced");
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
