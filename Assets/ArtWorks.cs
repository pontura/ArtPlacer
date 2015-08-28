using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ArtWorks : MonoBehaviour
{
    public Text title;
    public ThumbImage button;
    public GameObject container;
    public ScrollLimit scrollLimit;

    public Vector2 thumbSize = new Vector2(180, 180);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private int selectionId;
    private bool isOn;
    private int separationY = 0;
    private int separationx = 0;
    private int id;

    void Start()
    {
        title.text = Data.Instance.artData.GetCurrentGallery().title;

        thumbSize += separation;
        ArtData.GalleryData currentGallery = Data.Instance.artData.GetCurrentGallery();

       foreach (ArtData.GalleryData.ArtData data in currentGallery.artWorksData)
       {           	
			string path = data.url;
			if(title.text.Equals("My Artworks")){
				string folder = Data.Instance.GetArtPath();		
				path = Path.Combine(folder, data.url + ".png");
			}
			AddThumb(path);
       }
       float totalThumbs = currentGallery.artWorksData.Count;
       float totalRows = totalThumbs / cols;
       int maxScroll = (int)((totalRows - 1) * (thumbSize.y +separationY));
       if (totalRows > 2) scrollLimit.SetMaxScroll(maxScroll);

       print("cols: " + cols + " totalThumbs " + totalThumbs + " totalRows " + totalRows + " maxScroll " + maxScroll);
    }

    private void AddThumb(string url)
    {
        ThumbImage newButton = Instantiate(button);        
        newButton.transform.SetParent(container.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.transform.localPosition = Vector3.zero;
        newButton.Init(this, url, id);
        float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
        float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));
        newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 0);
        if (separationx == cols - 1)  {  separationY++;   separationx = 0; }  else separationx++;
        id++;
    }
    void SetOff()
    {
        scrollLimit.ResetScroll();
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
    public void Back()
    {
        Data.Instance.LoadLevel("Galleries");
    }
    public void OnSelect(int id)
    {   
		Data.Instance.artData.SetSelectedArtwork (id);
		Data.Instance.isArtworkInfo2Place = true;  
        Data.Instance.LoadLevel("ConfirmArtWork");
    }
}
