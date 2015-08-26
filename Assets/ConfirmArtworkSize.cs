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
        tooltipSizes.gameObject.SetActive(false);       

        //if (Data.Instance.areaData.areas.Count > 0)
        //AddConfirmSizes(Data.Instance.areaData.areas[0]);

		signal = Instantiate (artworkSignal);
		signal.transform.SetParent(container.transform);
		signal.transform.localPosition = new Vector3(0,0,0);
		signal.transform.localScale = Vector3.one;
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
		Debug.Log ("Name: " + signal.GetName ());
		Debug.Log ("Author: " + signal.GetAuthor ());
		Data.Instance.SavePhotoArt (signal.GetName (), signal.GetAuthor(), signal.GetWidth ()*100, signal.GetHeight ()*100);
		      
       	Data.Instance.LoadLevel("Galleries");
       
    }

}
