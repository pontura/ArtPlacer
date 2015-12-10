using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class Footer : MonoBehaviour {

    public Text title;
    public GameObject FooterPanel;
    public GameObject closeFooter;
    public Animation anim;
    public GameObject container;
    public ThumbImage ThumbButton;
    public int separation = 85;
    public GameObject Nav_Left;
    public GameObject Nav_Right;

    float goto_X;
    public float scrollSpeed = 0.1f;

    int totalThumbsToShow = 7;
    public int actualThumbScroll;
    ArtData.GalleryData galleryData;

	void Start () {
        actualThumbScroll = 0;
        closeFooter.SetActive(false);
        if (Data.Instance.areaData.CountArtPlaced() == 0 && Data.Instance.lastArtTexture == null)
            FooterPanel.SetActive(false);
        else
            LoadThumbs();
	}
    void LoadThumbs()
    {
        galleryData = Data.Instance.artData.GetCurrentGallery();

        title.text = galleryData.title;

        int a = 0;
        foreach(ArtData.GalleryData.ArtData artData in galleryData.artWorksData)
        {
            ThumbImage thumbButton = Instantiate(ThumbButton);
            thumbButton.transform.SetParent(container.transform);
            thumbButton.transform.localScale = Vector3.one;

            string url = artData.GetUrl();            

			thumbButton.Init(this, url , artData.artId, artData.isLocal);

            Vector3 pos = Vector3.zero;
            pos.x = separation * a;
            thumbButton.transform.localPosition = pos;
            a++;
        }
         SetNavigation();

    }
    public void OnSelect(int id)
    {
        print("Footer " + id);
		Data.Instance.artData.SetSelectedArtworkByThumbID (id);
		Events.OnSelectFooterArtwork();
    }
    public void Close()
    {
        anim.Play("FooterOff");
        closeFooter.SetActive(true);
		Events.ArtworkPreview (false);
    }
    public void Open()
    {
        closeFooter.SetActive(false);
        anim.Play("FooterOn");
		Events.ArtworkPreview (true);
    }

    private void SetNavigation()
    {
        print ("SetNavigation: " + galleryData.artWorksData.Count + " ++++++ " +  totalThumbsToShow) ;

        MoveContainer(0);

        if (galleryData.artWorksData.Count < totalThumbsToShow)
        {
            Nav_Left.SetActive(false);
            Nav_Right.SetActive(false);
        }
        
    }
    public void NavLeftClicked()
    {
        MoveContainer(5);
    }
    public void NavRightClicked()
    {
        MoveContainer(-5);
    }
    void MoveContainer(int qty)
    {
        Nav_Left.SetActive(true);
        Nav_Right.SetActive(true);

        actualThumbScroll += qty;
        Vector3 pos = container.transform.localPosition;
        goto_X = pos.x + (separation * qty);

        if (Mathf.Abs(actualThumbScroll) < 1) Nav_Left.SetActive(false);
        else if (Mathf.Abs(actualThumbScroll) > galleryData.artWorksData.Count - totalThumbsToShow) Nav_Right.SetActive(false);
    }
    
    void Update()
    {
        Vector3 pos = container.transform.localPosition;
        if (pos.x > goto_X)
            pos.x -= Mathf.Abs(pos.x - goto_X) * scrollSpeed;
        else if (pos.x < goto_X)
            pos.x += Mathf.Abs(pos.x - goto_X) * scrollSpeed;
        container.transform.localPosition = pos;
    }
}
