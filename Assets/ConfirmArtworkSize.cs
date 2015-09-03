using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkSize : MonoBehaviour {

    public ArtworkSignal artworkSignal;
	public ArtworkArea artworkArea;
    public Animation tooltipSizes;
    public GameObject container;  

	private ArtworkSignal signal;
	public ArtworkArea area;

    void Start()
    {
        tooltipSizes.gameObject.SetActive(false);       

        //if (Data.Instance.areaData.areas.Count > 0)
        //AddConfirmSizes(Data.Instance.areaData.areas[0]);

        signal = Instantiate(artworkSignal) as ArtworkSignal;
		signal.transform.SetParent(container.transform);
		signal.transform.localPosition = new Vector3(0,0,0);
		signal.transform.localScale = Vector3.one;

		if (Data.Instance.artData.selectedGallery == -2) {
			signal.Init(Data.Instance.artData.selectedArtWork.size.x, Data.Instance.artData.selectedArtWork.size.y);
			signal.name.text = Data.Instance.artData.selectedArtWork.title;
			signal.author.text = Data.Instance.artData.selectedArtWork.autor;
		}

		/*area = Instantiate (artworkArea);
		area.transform.SetParent(container.transform);
		area.transform.localPosition = new Vector3(0,0,0);
		area.transform.localScale = Vector3.one;*/

        Invoke("startTooltip", 0.5f);
    }   
    
    void startTooltip()
    {
        tooltipSizes.gameObject.SetActive(true);
        tooltipSizes.Play("tooltipOnVertical");
    }
	public void Back()
	{
		Data.Instance.LoadLevel("Walls");
    }
    public void Ready()
    {
		Data.Instance.SavePhotoArt (signal.GetName (), signal.GetAuthor(), signal.GetWidth ()*100, signal.GetHeight ()*100);
		      
       	Data.Instance.LoadLevel("Galleries");
       
    }

}
