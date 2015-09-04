using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkCrop : MonoBehaviour {

    public ArtworkArea artworkArea;
    public Animation tooltipSizes;
    public GameObject container;

	public SpriteRenderer photoBackground;

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

		Vector2 center = new Vector2 (Data.Instance.lastArtTexture.width*0.5f, Data.Instance.lastArtTexture.height*0.5f);
		Vector2 origin = center + (Vector2)rt.localPosition - new Vector2 (rt.sizeDelta.x * 0.5f,rt.sizeDelta.y * 0.5f);


		Color[] c = Data.Instance.lastArtTexture.GetPixels((int)origin.x, (int)origin.y, (int)rt.sizeDelta.x, (int)rt.sizeDelta.y);
		Texture2D tex = new Texture2D((int)rt.sizeDelta.x, (int)rt.sizeDelta.y);
		//Debug.Log ("X: "+rt.position.x+" Y: "+rt.position.y+" W: "+rt.sizeDelta.x+" H: "+rt.sizeDelta.y);
		tex.SetPixels(c);
		tex.Apply ();
		Data.Instance.lastArtTexture = tex;
		Data.Instance.LoadLevel("confirmArtworkSize");
       
    }

}
