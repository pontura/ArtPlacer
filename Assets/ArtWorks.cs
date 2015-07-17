using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ArtWorks : MonoBehaviour
{
    public ThumbImage button;
    public GameObject container;
    public ScrollLimit scrollLimit;

    public Vector2 thumbSize = new Vector2(195, 120);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private int selectionId;
    private bool isOn;
    private int separationY = 0;
    private int separationx = 0;
    private int id;

    void Start()
    {
        Events.OnDropBoxSelect += OnDropBoxSelect;
        thumbSize += separation;
    }
    void OnDestroy()
    {
        Events.OnDropBoxSelect -= OnDropBoxSelect;
    }
   
    public void OnDropBoxSelect(int dropBoxId, int selectionId)
    {
       this.selectionId = selectionId;
       SetOff();

       foreach (ArtData.GalleryData.ArtData data in Data.Instance.artData.galleries[selectionId].artWorksData)
       {
           data.gallery = Data.Instance.artData.galleries[selectionId].title;
           AddThumb(data.url);
       }
       if (separationY > 3) scrollLimit.SetMaxScroll(100);
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
    public void Back()
    {
        Data.Instance.LoadLevel("ArtPlaced");
    }
    public void OnSelect(int id)
    {
        print(selectionId + "    " + id);
        Data.Instance.artData.selectedArtWork = Data.Instance.artData.galleries[selectionId].artWorksData[id];
        Data.Instance.LoadLevel("ConfirmArtWork");
    }
}
