using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Galleries : MonoBehaviour {

    public GalleryButton favouritesButton;
    public GalleryButton galleryButton;
	public GalleryButton myArtWorks;
    public ScrollLimit scrollLimit;
    public GameObject buttonsContainer;

    private Vector2 thumbSize = new Vector2(290, 87);
    private int separationY = 0;
    private int separationx = 0;

    private int cols = 2;

	void Start () {
        Data.Instance.isPhoto4Room = true;
        foreach (ArtData.GalleryData data in Data.Instance.artData.galleries)
        {
            AddThumb(data.title, "");
        }
        if (separationY > 3) scrollLimit.SetMaxScroll(100);
		favouritesButton.Init (this, -1, "MY FAVOURITES (" + Data.Instance.artData.favorites.Count + ")", "");
		if (Data.Instance.artData.favorites.Count == 0)
			favouritesButton.gameObject.SetActive(false);

    }
    private int id = 0;
    private void AddThumb(string _title, string url)
    {
        GalleryButton newButton = Instantiate(galleryButton);
        newButton.transform.SetParent(buttonsContainer.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        newButton.Init(this, id, _title, url);
        float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
        float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));
        newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 0);
        if (separationx == cols - 1) { separationY++; separationx = 0; } else separationx++;
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
		if (Data.Instance.artData.myArtWorks.artWorksData.Count > 0) {
			OnSelect(-2);
		} else {
			Data.Instance.isPhoto4Room = false;
			Data.Instance.LoadLevel("TakePhoto");
		}

	}
}
