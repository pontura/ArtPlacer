using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

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
           data.gallery = Data.Instance.artData.galleries[selectionId].title;
           AddThumb(data.url);
       }
       int totalThumbs = currentGallery.artWorksData.Count;
       int totalRows = totalThumbs / cols;
       int maxScroll = (int)((totalRows - 1) * (thumbSize.y +separationY));
       if (totalRows > 3) scrollLimit.SetMaxScroll(maxScroll);
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
        print(selectionId + "    " + id);

        Data.Instance.artData.selectedArtWork = Data.Instance.artData.galleries[selectionId].artWorksData[id];

        if (Data.Instance.artData.selectedGallery == -1)
        {
            Data.Instance.artData.selectedArtWork.gallery = Data.Instance.artData.galleries[Data.Instance.artData.favorites[id].galleryId].title;
            Data.Instance.artData.selectedArtWork.galleryId = Data.Instance.artData.favorites[id].galleryId;
            Data.Instance.artData.selectedArtWork.artId = Data.Instance.artData.favorites[id].artId;
        }
        else
        {
            Data.Instance.artData.selectedArtWork.gallery = Data.Instance.artData.galleries[Data.Instance.artData.selectedGallery].title;
            Data.Instance.artData.selectedArtWork.galleryId = Data.Instance.artData.selectedGallery;
            Data.Instance.artData.selectedArtWork.artId = id;
        }
               
        Data.Instance.LoadLevel("ConfirmArtWork");
    }
}
