using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ArtWorks : MonoBehaviour
{
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
    //private int id;

    void Start()
    {
        Data.Instance.SetTitle("GALLERY: " + Data.Instance.artData.GetCurrentGallery().title);
        Events.Back += Back;
        thumbSize += separation;
        ArtData.GalleryData currentGallery = Data.Instance.artData.GetCurrentGallery();

		//if(currentGallery.id!=-2)addButton.gameObject.SetActive(false);

       foreach (ArtData.GalleryData.ArtData data in currentGallery.artWorksData)
       {           	
			string path = data.GetUrl();

			AddThumb(path, data.artId, data.isLocal);
       }

      // Events.OnScrollSizeRefresh(new Vector2(611, _y));

       float totalThumbs = currentGallery.artWorksData.Count;
       float totalRows = totalThumbs / cols;
       int maxScroll = (int)((totalRows - 1) * (thumbSize.y +separationY));

       print("cols: " + cols + " totalThumbs " + totalThumbs + " totalRows " + totalRows + " maxScroll " + maxScroll);

       if (Data.Instance.areaData.areas.Count == 0) WallsButton.SetActive(false);
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    private float _y;
    private void AddThumb(string url, int artId, bool local)
    {
        ThumbImage newButton = Instantiate(button) as ThumbImage;        
        newButton.transform.SetParent(container.transform);
        newButton.transform.localScale = Vector3.one;
        newButton.transform.localPosition = Vector3.zero;
		newButton.Init(this, url, artId, local);
        //float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
        //_y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));
        //newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 0);
        //if (separationx == cols - 1)  {  separationY++;   separationx = 0; }  else separationx++;
        //id++;
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
		Data.Instance.artData.selectedArtWork = null;
		Data.Instance.LoadLevel("LoadArtwork");
	}

    public void Back()
    {
        Data.Instance.LoadLevel("Galleries");
    }
    public void OnSelect(int id)
    {   
		Data.Instance.artData.SetSelectedArtworkByThumbID(id);
		Data.Instance.isArtworkInfo2Place = true;  
        Data.Instance.LoadLevel("ConfirmArtWork");
		// evitamos mostrar la info al seleccionar la obra y la pone directo en la pared
		//Data.Instance.LoadLevel ("ArtPlaced");
    }
}
