using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public Button FavoriteOn;
    public Button FavoriteOff;
	public Button PlaceItButton;
	public Button editButton;

    public RawImage rawImage;
    public Text title;

    private bool isFavorite;

	void Start () {
		float maxWidth = rawImage.rectTransform.sizeDelta.x;
		float maxHeight = rawImage.rectTransform.sizeDelta.y;
		float aspect = maxWidth / maxHeight;
		float textAspect = 1f*Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
		if (aspect > textAspect) {
			rawImage.rectTransform.sizeDelta = new Vector2(maxHeight*textAspect,maxHeight);
		}else if(aspect < textAspect) {
			rawImage.rectTransform.sizeDelta = new Vector2(maxWidth,maxWidth/textAspect);
		}

        rawImage.texture = Data.Instance.lastArtTexture;
        title.text = Data.Instance.artData.selectedArtWork.title;

		if (Data.Instance.artData.selectedArtWork.galleryId == -2 ) {
			FavoriteOn.gameObject.SetActive(false);
			FavoriteOff.gameObject.SetActive(false);
			editButton.gameObject.SetActive(true);
			//FavoriteOn.GetComponentInChildren<Text> ().text = "DELETE";
		} else {
			if (Data.Instance.artData.isFavorite (Data.Instance.artData.selectedArtWork.galleryId, Data.Instance.artData.selectedArtWork.artId))
				isFavorite = true;            
			SetFavorite ();
		}

		if (Data.Instance.isArtworkInfo2Place == false)
			PlaceItButton.GetComponentInChildren<Text> ().text = "BACK to ROOM";

    }
    public void Confirm()
    {
		if(Data.Instance.isArtworkInfo2Place == false)Data.Instance.lastArtTexture = null;
        if (Data.Instance.lastPhotoTexture == null)
			Data.Instance.LoadLevel ("LoadRoom");
		else {
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
		if(Data.Instance.isArtworkInfo2Place == false)Data.Instance.lastArtTexture = null;
        Data.Instance.LoadLevel("Artworks");
    }
    public void Galleries()
    {
		if(Data.Instance.isArtworkInfo2Place == false)Data.Instance.lastArtTexture = null;
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

	public void RemoveArtwork(){
		print ("ArtID: " + Data.Instance.artData.selectedArtWork.artId);
		Data.Instance.artData.DeleteArtworkData (Data.Instance.artData.selectedArtWork.artId);
		Back ();
	}

	public void editArtwork(){
		Data.Instance.isPhoto4Room = false;
		Data.Instance.LoadLevel ("ConfirmArtworkSize");
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
