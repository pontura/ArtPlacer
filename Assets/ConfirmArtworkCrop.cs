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

		int sh = Screen.height;
		float invSh = 1f/Screen.height;

		Vector2 relativeCenter = new Vector2 (invSh * rt.localPosition.x * Data.Instance.lastArtTexture.height, invSh * rt.localPosition.y * Data.Instance.lastArtTexture.height);
		Vector2 relativeSize = new Vector2 (invSh * rt.sizeDelta.x * Data.Instance.lastArtTexture.height, invSh * rt.sizeDelta.y * Data.Instance.lastArtTexture.height);

		Vector2 center = new Vector2 (Data.Instance.lastArtTexture.width*0.5f, Data.Instance.lastArtTexture.height*0.5f);
		Vector2 origin = center + relativeCenter - (relativeSize*0.5f);


		Color[] c = Data.Instance.lastArtTexture.GetPixels((int)origin.x, (int)origin.y, (int)relativeSize.x, (int)relativeSize.y);
		Texture2D tex = new Texture2D((int)relativeSize.x, (int)relativeSize.y);
		//Debug.Log ("X: "+rt.position.x+" Y: "+rt.position.y+" W: "+rt.sizeDelta.x+" H: "+rt.sizeDelta.y);
		tex.SetPixels(c);
		tex.Apply ();
		Data.Instance.lastArtTexture = tex;
		Data.Instance.LoadLevel("confirmArtworkSize");
       
    }

}
