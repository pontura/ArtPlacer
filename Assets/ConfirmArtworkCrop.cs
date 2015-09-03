using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkCrop : MonoBehaviour {

    public ArtworkArea artworkArea;
    public Animation tooltipSizes;
    public GameObject container;  

	private ArtworkArea area;

    void Start()
    {
        tooltipSizes.gameObject.SetActive(false);               

		area = Instantiate (artworkArea);
		area.transform.SetParent(container.transform);
		area.transform.localPosition = new Vector3(0,0,0);
		area.transform.localScale = Vector3.one;

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
		RectTransform rt = area.GetComponent<RectTransform> ();

		Color[] c = Data.Instance.lastArtTexture.GetPixels((int)rt.position.x, (int)rt.position.y, (int)rt.sizeDelta.x, (int)rt.sizeDelta.y);
		Texture2D tex = new Texture2D((int)rt.sizeDelta.x, (int)rt.sizeDelta.y);
		Debug.Log ("X: "+rt.position.x+" Y: "+rt.position.y+" W: "+rt.sizeDelta.x+" H: "+rt.sizeDelta.y);
		tex.SetPixels(c);
		tex.Apply ();
		Data.Instance.lastArtTexture = tex;
		Data.Instance.LoadLevel("confirmArtworkSize");
       
    }

}
