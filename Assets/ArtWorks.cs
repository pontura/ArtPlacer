using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using ImageVideoContactPicker;

public class ArtWorks : MonoBehaviour
{
    public ContactGallery contactGallery;
    public GameObject addFiltersButton;

    public GameObject Add;
    public GameObject Add_On;
    public GameObject SubMenu;
    public Animation anim;

    public Text helpText;
    public GameObject WallsButton;
    public Text title;
    public ThumbImage button;
    public GameObject container;
	//public GameObject addButton;

    public Vector2 thumbSize = new Vector2(180, 180);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private int selectionId;
    private bool isOn;
    private int separationY = 0;
    private int separationx = 0;

    public ArtData.GalleryData gallery;
    //private int id;

    void Start()
    {
        //Data.Instance.artData.selectedArtWork.url = "";
        gallery = Data.Instance.artData.GetCurrentGallery();
        Data.Instance.SetBackActive(true);
        string _title = "";
        if (Data.Instance.artData.selectedGallery != -3)
        {
            _title = "Gallery: ";
            addFiltersButton.SetActive(false);
        }
        else
        {
            addFiltersButton.SetActive(true);
        }

        Data.Instance.SetTitle(_title + Data.Instance.artData.GetCurrentGallery().title);

        EventsAnalytics.EnterGallery(Data.Instance.artData.GetCurrentGallery().title);

        Events.Back += Back;
        PickerEventListener.onImageLoad += OnImageLoad;
        thumbSize += separation;
        ArtData.GalleryData currentGallery = Data.Instance.artData.GetCurrentGallery();

        if (Data.Instance.artData.selectedGallery <0)
            contactGallery.gameObject.SetActive(false);
        else
            contactGallery.gameObject.SetActive(true);

		//if(currentGallery.id!=-2)addButton.gameObject.SetActive(false);

        if (currentGallery.artWorksData.Count == 0)
        {
            if (Data.Instance.artData.selectedGallery == -1)
            {
                helpText.text = "There are no artworks marked as favorites.";
            }
            else if (Data.Instance.artData.selectedGallery == -2)
            {
                helpText.text = "You didn´t create any artworks yet.";
            }
            else if (Data.Instance.artData.selectedGallery == -3)
            {
                helpText.text = "No artworks with this filter.";
            }
            Events.HelpChangeState(true);
        }

       foreach (ArtData.GalleryData.ArtData data in currentGallery.artWorksData)
       {
           string path;
           try
           {
               path = data.GetUrl(true);               
               AddThumb(path, data.artId, data.isLocal);
           }
           catch
           {
               Debug.Log("LA obra ya no existe ");
           }			
       }

       float totalThumbs = currentGallery.artWorksData.Count;
       float totalRows = totalThumbs / cols;
       int maxScroll = (int)((totalRows - 1) * (thumbSize.y +separationY));

    //   print("cols: " + cols + " totalThumbs " + totalThumbs + " totalRows " + totalRows + " maxScroll " + maxScroll);

       if (Data.Instance.areaData.areas.Count == 0) WallsButton.SetActive(false);

       if (Data.Instance.artData.selectedGallery == -2)
       {
           Add.SetActive(true);
           WallsButton.SetActive(false);
       }
       else
           Add.SetActive(false);

    }
    public void AddFilter()
    {
        Data.Instance.LoadLevel("Filters");
    }
    public void RemoveFilters()
    {
        Data.Instance.filtersManager.Clear();
        Data.Instance.LoadLevel("Filters");
    }
    void OnDestroy()
    {
        Events.Back -= Back;
        PickerEventListener.onImageLoad -= OnImageLoad;
    }
    private float _y;
    private void AddThumb(string url, int artId, bool local)
    {
        ThumbImage newButton = Instantiate(button) as ThumbImage;        
        newButton.transform.SetParent(container.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.transform.localPosition = Vector3.zero;
		newButton.Init(this, url, artId, local);
    }
    void SetOff()
    {
        separationY = 0;
        separationx = 0;
        foreach (ThumbImage child in container.GetComponentsInChildren<ThumbImage>())
            Destroy(child.gameObject);
    }
    public void GotoRoom()
    {
        if(Data.Instance.lastPhotoTexture == null)
            Data.Instance.LoadLevel("LoadRoom");
        else
            Data.Instance.LoadLevel("ArtPlaced");
    }
	public void AddArtwork()
	{
		Data.Instance.artData.selectedGallery = 0;
		
		Data.Instance.LoadLevel("LoadArtwork");
	}

    public void Back()
    {

        if (Data.Instance.lastScene == "SelectArtworks")
            Data.Instance.LoadLevel("SelectArtworks");
        else if (Data.Instance.lastScene == "Galleries")
            Data.Instance.LoadLevel("Galleries");
        else if (Data.Instance.artData.selectedGallery == -3)
             Data.Instance.LoadLevel("Filter");
          else if (Data.Instance.artData.selectedGallery == -1 || Data.Instance.artData.selectedGallery == -2)
                Data.Instance.LoadLevel("SelectArtworks");
          else
              Data.Instance.LoadLevel("Galleries");
    }
   

    public void Open()
    {
        if (
           !StoreData.Instance.fullVersion &&
           StoreData.Instance.GetComponent<StoreSettings>().loaded &&
           Data.Instance.artData.myArtWorks.artWorksData.Count >= StoreData.Instance.GetComponent<StoreSettings>().max_artworks
           )
        {
            Events.OnGetFullVersion(StoreData.Instance.GetComponent<StoreSettings>().msg_artworks);
            return;
        }
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
    public void TakePhoto()
    {
        EventsAnalytics.SendScreen("NEW_ARTWORK_PHOTO");
        Data.Instance.artData.selectedArtWork.url = "";
        Data.Instance.isPhoto4Room = false;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
        EventsAnalytics.SendScreen("NEW_ARTWORK_BROWSE");
        Data.Instance.artData.selectedArtWork.url = "";
        Events.OnPicker(true);
        Invoke("Delay", 0.1f);        
    }
    void Delay()
    {
        Debug.Log("Browse");
#if UNITY_ANDROID
        AndroidPicker.BrowseImage();
#elif UNITY_IPHONE
		IOSPicker.BrowseImage();
#endif
    }
    public void OnImageLoad(string imgPath, Texture2D tex)
    {
        Events.OnPicker(false);
        Data.Instance.isPhoto4Room = false;
        Data.Instance.lastArtTexture = tex;
        Data.Instance.LoadLevel("ConfirmPhoto");
    }





    public void OnSelect(int id)
    {
        //Data.Instance.artData.SetSelectedArtworkByThumbID(id);
        //Data.Instance.isArtworkInfo2Place = true;  
        //Data.Instance.LoadLevel("ConfirmArtWork");
        Events.OnLoading(true);
        if (Data.Instance.artData.selectedGallery == -3 || Data.Instance.artData.selectedGallery == -1)
        {
            print(id);
            int gallery_id = 0;
            foreach (ArtData.GalleryData.ArtData artData in gallery.artWorksData)
            {
                if (artData != null && artData.artId != null && artData.artId == id)
                    gallery_id = artData.galleryId;
            }
            print(id + " gallery_id: " + gallery_id);
            StartCoroutine(LoadArtWork(Data.Instance.artData.GetArtData(gallery_id, id)));
        }
        else
        {	
            StartCoroutine(LoadArtWork(Data.Instance.artData.GetArtData(gallery.id, id)));
        }

    }
    int GetCurentDataIdInArray()
    {
        int id = 0;
        foreach (ArtData.GalleryData.ArtData artData in gallery.artWorksData)
        {
            if (artData.url == Data.Instance.artData.selectedArtWork.url)
                return id;

            id++;
        }
        return id;
    }
    private Texture2D texture2d;
    public IEnumerator LoadArtWork(ArtData.GalleryData.ArtData artData)
    {		
        Debug.Log("______" + artData.url + " gallery.id " + Data.Instance.artData.selectedGallery);

        if (Data.Instance.artData.selectedGallery == -2)
            texture2d = TextureUtils.LoadLocal(artData.GetUrl(false));
        else
            yield return StartCoroutine(TextureUtils.LoadRemote(artData.url, value => texture2d = value));

        Events.OnLoading(false);

        Data.Instance.SetLastArtTexture(texture2d);


        Data.Instance.artData.SetSelectedArtworkByArtID(artData.artId);
        Data.Instance.artData.SetSelectedArtworkByThumbID(artData.artId);
        Data.Instance.isArtworkInfo2Place = true;
        Data.Instance.LoadLevel("ConfirmArtWork");

        //Data.Instance.artData.SetSelectedArtworkByThumbID(id);
        //Data.Instance.isArtworkInfo2Place = true;
        //Data.Instance.LoadLevel("ConfirmArtWork");


        Debug.Log("newArtID: " + artData.artId);
       
        yield return null;
    }
}
