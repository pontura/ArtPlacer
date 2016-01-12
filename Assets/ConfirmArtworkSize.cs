using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkSize : MonoBehaviour {

    public ArtworkSignal artworkSignal;
    public Animation tooltipSizes;
    public GameObject container;  

	private ArtworkSignal signal;

    void Start()
    {
        Events.HelpHide();
        tooltipSizes.gameObject.SetActive(false);
        Events.Back += Back;

        //if (Data.Instance.areaData.areas.Count > 0)
        //AddConfirmSizes(Data.Instance.areaData.areas[0]);

        signal = Instantiate(artworkSignal) as ArtworkSignal;
		signal.transform.SetParent(container.transform);
		signal.transform.localPosition = new Vector3(140,70,0);
		//signal.transform.localScale = new Vector3(1f,0.8f,1f);
		signal.transform.localScale = Vector3.one;
		signal.GetComponent<Canvas> ().sortingOrder = 1;

		if (Data.Instance.artData.selectedGallery == -2) {
			signal.Init (Data.Instance.artData.selectedArtWork.size.x, Data.Instance.artData.selectedArtWork.size.y);
			signal.name.text = Data.Instance.artData.selectedArtWork.title;
			signal.author.text = Data.Instance.artData.selectedArtWork.autor;
		} else {
			signal.Init (50, 50);
		}

        //Invoke("startTooltip", 0.5f);
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    void startTooltip()
    {
        tooltipSizes.gameObject.SetActive(true);
        tooltipSizes.Play("tooltipOnVertical");
    }
	public void Back()
	{
		//Data.Instance.LoadLevel("Walls");
    }
    public void Ready()
    {
		float aspect = 1f*Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height;
		int w = (int)(signal.GetHeight () * aspect);
		Data.Instance.SavePhotoArt (signal.GetName (), signal.GetAuthor(), w, signal.GetHeight ());		      
       	Data.Instance.LoadLevel("Artworks");
       
    }

}
