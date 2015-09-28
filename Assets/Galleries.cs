using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Galleries : MonoBehaviour {

    public GalleryButton favouritesButton;
    public GalleryButton galleryButton;
	public GalleryButton myArtWorks;
    public GameObject buttonsContainer;
	public Animation tooltipAddArt;

    private Vector2 thumbSize = new Vector2(290, 87);

	void Start () {
        Data.Instance.isPhoto4Room = true;
        foreach (ArtData.GalleryData data in Data.Instance.artData.galleries)
        {
            AddThumb(data.title, "");
        }

		if (Data.Instance.artData.favorites.Count == 0)
			favouritesButton.gameObject.SetActive (false);
		else
			favouritesButton.Init (this, -1, "my favourites (" + Data.Instance.artData.favorites.Count + ")", "");
		if (Data.Instance.artData.myArtWorks.artWorksData.Count == 0) {
			myArtWorks.gameObject.SetActive (false);
			tooltipAddArt.gameObject.SetActive(true);
			tooltipAddArt.Play("tooltipOn");
			Invoke("CloseAddToolTip",3);
		}else
			myArtWorks.Init (this, -2, "my artworks (" + Data.Instance.artData.myArtWorks.artWorksData.Count + ")", "");
    }
    private int id = 0;
    private void AddThumb(string _title, string url)
    {
        GalleryButton newButton = Instantiate(galleryButton);
        newButton.transform.SetParent(buttonsContainer.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        newButton.Init(this, id, _title, url);
        id++;
    }
    public void OnSelect(int id)
    {
        Data.Instance.artData.selectedGallery = id;
        Data.Instance.LoadLevel("Artworks");
    }
    public void GotoRoom()
    {
        if (Data.Instance.lastPhotoTexture != null)
            Data.Instance.LoadLevel("ArtPlaced");
        else
            Data.Instance.LoadLevel("LoadRoom");
    }

	public void AddArtWorks(){
		Data.Instance.artData.selectedGallery = 0;
		Data.Instance.LoadLevel("LoadArtwork");
		tooltipAddArt.gameObject.SetActive(false);
	}
	void CloseAddToolTip(){
		tooltipAddArt.gameObject.SetActive (false);
	}

	public void GotoMyArtWorks(){
		OnSelect(-2);
	}
}
