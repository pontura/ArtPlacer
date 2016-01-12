using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ImageVideoContactPicker;

public class Galleries : MonoBehaviour {

    public GameObject WallsButton;
    public Animation anim;
    public GameObject Add;
    public GameObject Add_On;

    public GameObject SubMenu;
  //  public GalleryButton favouritesButton;
    public GalleryButton galleryButton;
//	public GalleryButton myArtWorks;
    public GameObject buttonsContainer;
	public Animation tooltipAddArt;

	public GameObject waitSign;

    private Vector2 thumbSize = new Vector2(290, 87);

	bool jsonReady = false;

	void Start () {
        Data.Instance.SetBackActive(true);
        Data.Instance.lastArtTexture = null;
        Events.HelpHide();
        Close();
        Data.Instance.SetMainMenuActive(true);
        Data.Instance.SetTitle("Artworks by Galleries");
        Events.Back += Back;
		PickerEventListener.onImageLoad += OnImageLoad;

        Data.Instance.isPhoto4Room = true;

		if (Data.Instance.artData.galleries.Length > 0 && !jsonReady)
			InitThumbs ();
        
        //if (Data.Instance.artData.myArtWorks.artWorksData.Count == 0) {
        //    myArtWorks.gameObject.SetActive (false);
        //    //	tooltipAddArt.gameObject.SetActive(true);
        //    //	tooltipAddArt.Play("tooltipOn");
        //    //	Invoke("CloseAddToolTip",3);
        //}else
        //    myArtWorks.Init (this, -2, "my artworks (" + Data.Instance.artData.myArtWorks.artWorksData.Count + ")", "");

        if (Data.Instance.areaData.areas.Count == 0) WallsButton.SetActive(false);

        Add.SetActive(false);

    }

	void InitThumbs(){
		jsonReady = true;
		foreach (ArtData.GalleryData data in Data.Instance.artData.galleries)
		{
            string url = "";
            if (data.artWorksData != null && data.artWorksData.Count > 0 && data.artWorksData[0].url != "")
            {
                int randomId =Random.Range(0, data.artWorksData.Count);
                url = data.artWorksData[randomId].url;
            }
            AddThumb(data.title, url, data.id);
		}

        //if (Data.Instance.artData.favorites.Count == 0)
        //    favouritesButton.gameObject.SetActive (false);
        //else
        //    favouritesButton.Init (this, -1, "my favourites (" + Data.Instance.artData.favorites.Count + ")", "");


		waitSign.SetActive(false);

	}

	void Update(){
		if (Data.Instance.artData.galleries.Length > 0 && !jsonReady)
			InitThumbs ();
	}

    void OnDestroy()
    {
        Events.Back -= Back;
        PickerEventListener.onImageLoad -= OnImageLoad;
    }
    void Back()
    {
        if(Data.Instance.areaData.areas.Count==0)
            Data.Instance.LoadLevel("SelectArtworks");
        else
            Data.Instance.LoadLevel("ArtPlaced");
    }
    public void Open()
    {

        Add.SetActive(false);
        Add_On.SetActive(true);
        SubMenu.SetActive(true);
        anim.Play("subMenuOpen");
    }
    public void Close()
    {
        Add.SetActive(true);
        Add_On.SetActive(false);
        SubMenu.SetActive(false);
        //Data.Instance.LoadLevel("LoadRoom");
    }
    //private int id = 0;
    private void AddThumb(string _title, string url, int id)
    {
        GalleryButton newButton = Instantiate(galleryButton);
        newButton.transform.SetParent(buttonsContainer.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        newButton.Init(this, id, _title, url);

        print("____" + url + "      _title: " + _title);
        //id++;
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
		Data.Instance.artData.selectedArtWork = null;
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

    public void TakePhoto()
    {
        Data.Instance.isPhoto4Room = false;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
        Debug.Log("Aca");
#if UNITY_ANDROID
        AndroidPicker.BrowseImage();
#elif UNITY_IPHONE
		IOSPicker.BrowseImage();
#endif
    }

    public void OnImageLoad(string imgPath, Texture2D tex)
    {
        Data.Instance.isPhoto4Room = false;
        Data.Instance.lastArtTexture = tex;
        Data.Instance.LoadLevel("ConfirmPhoto");
    }


}
