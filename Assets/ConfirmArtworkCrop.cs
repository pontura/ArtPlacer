using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmArtworkCrop : MonoBehaviour {

    public ArtworkArea artworkArea;
   // public Animation tooltipSizes;
    public GameObject container;
	public Camera cam;
	public Text debugText;
	public Canvas canvas;

	private ArtworkArea area;

    void Start()
    {
        Events.HelpHide();
        Data.Instance.SetBackActive(false);
        // tooltipSizes.gameObject.SetActive(false);               

		area = Instantiate (artworkArea);
		area.transform.SetParent(container.transform);
		area.transform.localPosition = new Vector3(0,0,0);
		area.transform.localScale = Vector3.one;

        Invoke("startTooltip", 0.5f);
    }   
    
    void startTooltip()
    {
        // tooltipSizes.gameObject.SetActive(true);
        // tooltipSizes.Play("tooltipOnVertical");
    }
	public void Back()
	{
		Data.Instance.LoadLevel("Walls");
    }
    public void Ready()
    {
        Events.OnLoading(true);
		area.gameObject.SetActive (false);
		Invoke("cropImage", 0.5f);

		/*RectTransform rt = area.GetComponent<RectTransform> ();
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


		/*RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
		cam.targetTexture = rt;
		Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

		cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);*/       
    }

	void cropImage(){

		RectTransform zona = area.GetComponent<RectTransform> ();

		RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
		cam.targetTexture = rt;		
		cam.Render();
		RenderTexture.active = rt;

		Texture2D image = new Texture2D((int)(zona.rect.width*canvas.scaleFactor), (int)(zona.rect.height*canvas.scaleFactor));
		/*string debug = "ScreenW: " + Screen.width + " ScreenH: " + Screen.height+"\n"; 
		debug += "Pos: " + zona.position + " pos2: " + zona.rect.position+"\n"; 
		debug += "xMin: " + zona.rect.xMin + " xMax: " + zona.rect.xMax+" "; 
		debug += "yMin: " + zona.rect.yMin + " yMax: " + zona.rect.yMax+"\n";
		debug += "Pos: "+ (zona.position.x + zona.rect.xMin)+", "+(zona.position.y - zona.rect.yMin)+" width: "+zona.rect.width+" height: "+zona.rect.height+"\n";
		debug += "ScaleFactor: "+canvas.scaleFactor+"\n";
		debug += "Pos: "+ (zona.position.x + zona.rect.xMin)+", "+(zona.position.y - zona.rect.yMin)+" width: "+zona.rect.width+" height: "+zona.rect.height+"\n";
		debug += "Pos: "+ (zona.position.x*canvas.scaleFactor+zona.rect.xMin*canvas.scaleFactor)+", "+(Screen.height*canvas.scaleFactor-(zona.position.y *canvas.scaleFactor + zona.rect.yMax *canvas.scaleFactor))+" width: "+(zona.rect.width*canvas.scaleFactor)+" height: "+(zona.rect.height*canvas.scaleFactor)+"\n";*/

		//#if UNITY_EDITOR
		image.ReadPixels(new Rect(zona.position.x+zona.rect.xMin*canvas.scaleFactor, Screen.height-(zona.position.y + zona.rect.yMax *canvas.scaleFactor), zona.rect.width*canvas.scaleFactor, zona.rect.height*canvas.scaleFactor), 0, 0);
		//else 
		//image.ReadPixels(new Rect(zona.position.x+zona.rect.xMin*canvas.scaleFactor, (zona.position.y + zona.rect.yMin*canvas.scaleFactor), zona.rect.width*canvas.scaleFactor, zona.rect.height*canvas.scaleFactor), 0, 0);
		//#endif

		image.Apply();
		Data.Instance.lastArtTexture = image;
		//debugText.text = debug;
		Data.Instance.LoadLevel("confirmArtworkSize");
	}

}
