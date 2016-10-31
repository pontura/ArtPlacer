using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmArtWork : MonoBehaviour {

    public GameObject WallsButton;
    public Button FavoriteOn;
    public Button FavoriteOff;
	public Button PlaceItButton;
	public Button editButton;
	public GameObject addButton;

    public RawImage rawImage;
    public GameObject paginator;
   // public Text title;

    private bool isFavorite;
    public ArtData.GalleryData gallery;

	void Start () {
        gallery = Data.Instance.artData.GetCurrentGallery();
        if (gallery.artWorksData.Count < 2)
        {
            paginator.SetActive(false);
        }
        Data.Instance.isPhoto4Room = true;

        Events.Back += Back;
        Data.Instance.SetTitle(Data.Instance.artData.selectedArtWork.title);

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
      //  title.text = ;

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
        {
            PlaceItButton.GetComponentInChildren<Text>().text = "< Gallery";
            Events.HelpChangeStep(1);
        }
        else
        {
            Events.HelpHide();
        }

		Events.ConvertUnits -= ConvertUnits;

        if (Data.Instance.areaData.areas.Count == 0) WallsButton.SetActive(false);


    }
    public void Confirm()
    {
        Events.OnLoading(true);
        Invoke("Delay", 0.1f);
        
    }
    public void Delay()
    {
        if (Data.Instance.areaData.areas.Count == 0)
            Data.Instance.LoadLevel("LoadRoom");
        else
        {
            Data.Instance.roomsData.ChangesMade(true);
            if (Data.Instance.isArtworkInfo2Place == false)
                Data.Instance.LoadLevel("Artworks");
            else
                Data.Instance.LoadLevel("ArtPlaced");
        }
    }
	public void Back2Walls()
	{
        Data.Instance.lastArtTexture = null;
        Data.Instance.LoadLevel("ArtPlaced");
	}
    public void Back()
    {
		if (Data.Instance.lastScene == "ConfirmArtworkSize") {
			Data.Instance.LoadLevel ("Artworks");
		} else if (Data.Instance.lastScene == "ConfirmArtWork") {
			Debug.Log ("Aca");
			Data.Instance.LoadLevel ("Artworks");
		} else {
			Data.Instance.lastArtTexture = null;
			Data.Instance.Back ();
		}
		Debug.Log (Data.Instance.lastScene);
    }
    public void Galleries()
    {
		Data.Instance.lastArtTexture = null;
		Data.Instance.LoadLevel("Galleries");
    }
    public void SwitchFavorites()
    {
        isFavorite = !isFavorite;
        if (isFavorite)
        {
            if (
                !StoreData.Instance.fullVersion && 
                StoreData.Instance.GetComponent<StoreSettings>().loaded &&
                Data.Instance.artData.favorites.Count >= StoreData.Instance.GetComponent<StoreSettings>().max_favourites
           )
            {
                Events.OnGetFullVersion(StoreData.Instance.GetComponent<StoreSettings>().msg_favourites);
                return;
            }
            Data.Instance.artData.AddToFavorites();
        }
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
            EventsAnalytics.NewFavourite(Data.Instance.artData.selectedArtWork.galleryId.ToString(), Data.Instance.artData.selectedArtWork.artId.ToString());
        }
        else
        {
            FavoriteOn.gameObject.SetActive(false);
            FavoriteOff.gameObject.SetActive(true);
        }
		addButton.SetActive (isFavorite);
    }

	public void Add(){

		if (
			!StoreData.Instance.fullVersion &&
			StoreData.Instance.GetComponent<StoreSettings>().loaded &&
			Data.Instance.artData.myArtWorks.artWorksData.Count >= StoreData.Instance.GetComponent<StoreSettings>().max_artworks
		)
		{
			Events.OnGetFullVersion(StoreData.Instance.GetComponent<StoreSettings>().msg_artworks);
			return;
		}

		SwitchFavorites ();
		Data.Instance.SavePhotoArt(Data.Instance.artData.selectedArtWork.title,Data.Instance.artData.selectedArtWork.autor+", "+Data.Instance.artData.selectedArtWork.gallery,Data.Instance.artData.selectedArtWork.size.x,Data.Instance.artData.selectedArtWork.size.y);
		Data.Instance.artData.selectedGallery = -2;
		Data.Instance.LoadLevel("Artworks");
	}

	private void ConvertUnits(){

	}

	void OnDestroy()
	{
		Events.ConvertUnits -= ConvertUnits;
        Events.Back -= Back;
	}
    public void Next()
    {
        MoveTo(true);
    }
    public void Prev()
    {
        MoveTo(false);
    }
    private bool next;
    void MoveTo(bool next)
    {
        this.next = next;
        Events.OnLoading(true);
        int artDataArrayID = GetCurentDataIdInArray();
        if (artDataArrayID == -1)
        {
            Debug.Log("ERROR");
            Events.OnLoading(false);
            return;
        }
        if (next)
        {
            if (artDataArrayID + 1 == gallery.artWorksData.Count) artDataArrayID = 0;
            else artDataArrayID++;
        }
        else
        {
            if (artDataArrayID == 0) artDataArrayID = gallery.artWorksData.Count - 1;
            else artDataArrayID--;
        }
        //int newArtID = gallery.artWorksData[artDataArrayID].artId;
        StartCoroutine(LoadArtWork(gallery.artWorksData[artDataArrayID]));

    }
    int GetCurentDataIdInArray()
    {
        int id = 0;
        foreach (ArtData.GalleryData.ArtData artData in gallery.artWorksData)
        {
            if (artData != null && artData.url == Data.Instance.artData.selectedArtWork.url)
                return id;

            id++;
        }
        return -1;
    }
    private Texture2D texture2d;
    public IEnumerator LoadArtWork(ArtData.GalleryData.ArtData artData)
    {
        if (artData == null)
        {
            Debug.Log("No existe el proximo artwork!");
            Events.OnLoading(false);
            MoveTo(next);
        }
        else
        {
            if (gallery.id == -2)
                texture2d = TextureUtils.LoadLocal(artData.GetUrl(false));
            else
                yield return StartCoroutine(TextureUtils.LoadRemote(artData.url, value => texture2d = value));


            Data.Instance.SetLastArtTexture(texture2d);

            Data.Instance.artData.SetSelectedArtworkByArtID(artData.artId);
            Data.Instance.artData.SetSelectedArtworkByThumbID(artData.artId);
            Data.Instance.isArtworkInfo2Place = true;
            Data.Instance.LoadLevel("ConfirmArtWork");

            Events.OnLoading(false);
        }
        yield return null;
    }




    //public void OnSelected(Footer footer, int id)
    //{
    //    if (sprite)
    //    {
    //        Data.Instance.SetLastArtTexture(texture2d);
    //        //Data.Instance.lastArtTexture = sprite.texture;
    //    }
    //    footer.OnSelect(id);
    //}

    //public void OnSelectedLocal(ArtWorks artWorks, int id)
    //{
    //    if (sprite)
    //    {
    //        Data.Instance.SetLastArtTexture(texture2d);
    //        //Data.Instance.lastArtTexture = sprite.texture;
    //    }
    //    artWorks.OnSelect(id);
    //}

   
}
