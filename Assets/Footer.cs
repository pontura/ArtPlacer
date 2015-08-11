using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Footer : MonoBehaviour {

    public Text title;
    public GameObject FooterPanel;
    public GameObject closeFooter;
    public Animation anim;
    public GameObject container;
    public ThumbImage ThumbButton;
    public int separation = 85;

	void Start () {
        closeFooter.SetActive(false);
        if (Data.Instance.areaData.CountArtPlaced() == 0 && Data.Instance.lastArtTexture == null)
            FooterPanel.SetActive(false);
        else
            LoadThumbs();
	}
    void LoadThumbs()
    {
        ArtData.GalleryData galleryData = Data.Instance.artData.GetCurrentGallery();

        title.text = galleryData.title;

        int a = 0;
        foreach(ArtData.GalleryData.ArtData artData in galleryData.artWorksData)
        {
            ThumbImage thumbButton = Instantiate(ThumbButton);
            thumbButton.transform.SetParent(container.transform);
            thumbButton.transform.localScale = Vector3.one;

            thumbButton.Init(this, artData.url, a);

            Vector3 pos = Vector3.zero;
            pos.x = separation * a;
            thumbButton.transform.localPosition = pos;
            a++;
        }
    }
    public void OnSelect(int id)
    {
        print("Footer " + id);
    }
    public void Close()
    {
        anim.Play("FooterOff");
        closeFooter.SetActive(true);
    }
    public void Open()
    {
        closeFooter.SetActive(false);
        anim.Play("FooterOn");
    }
}
