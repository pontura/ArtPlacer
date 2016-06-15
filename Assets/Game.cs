using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public SpriteRenderer background;
    private bool photoLoadead;
    public Camera myCamera;

    private float scaleFor_4x3 = 0.28f;
    private float scaleFor_16x9 = 0.28f;  

	void Start () {
        LoadLastPhotoTexture();
	}
	
	// Update is called once per frame
	public void LoadLastPhotoTexture () {

        if (photoLoadead) return;
        photoLoadead = true;

        //hack para que se vea de una:
        myCamera.enabled = false;
        myCamera.enabled = true;

        Texture2D texture = Data.Instance.lastPhotoTexture;
		/*Texture2D texture = new Texture2D (432, 720);
		Color32[] c = new Color32[texture.GetPixels32(0).Length];
		for (int i=0; i<c.Length; i++)
			c[i] = Color.red;
		texture.SetPixels32(c);*/        

		//Debug.Log ("TW: " + texture.width + " TH: " + texture.height);

		// Si la foto es para no es para un room busca la textura en lastArtTexture

        //achica las imagenes cargadas de la compu:
        float factor = 1;

        if (!Data.Instance.isPhoto4Room)
        {
            texture = Data.Instance.lastArtTexture;
        }
        else if(Data.Instance.RoomFromLocalFiles)
        {
            factor = 1/1.2f;
        }

		Rect rect = new Rect();

		float maxWidth = Data.Instance.defaultCamSize.x;
		float maxHeight = Data.Instance.defaultCamSize.y;
		float aspect = maxWidth / maxHeight;
		float textAspect = 1f*texture.width / texture.height;
		if (aspect > textAspect) {
			rect = new Rect (0, 0, (int)(maxHeight * textAspect), (int)maxHeight);
			texture = TextureUtils.ResizeTexture(texture,TextureUtils.ImageFilterMode.Nearest,maxHeight/texture.height);
		} else if (aspect < textAspect) {
			rect = new Rect (0, 0, (int)maxWidth, (int)(maxWidth / textAspect));
			texture = TextureUtils.ResizeTexture(texture,TextureUtils.ImageFilterMode.Nearest,maxWidth/texture.width);
		} else {
			rect = new Rect (0, 0, (int)(maxWidth), (int)(maxHeight));
			texture = TextureUtils.ResizeTexture(texture,TextureUtils.ImageFilterMode.Nearest,maxWidth/texture.width);
			//texture.Resize(texture.width,texture.height);
			//texture.Apply();
		}        

		Sprite sprite = Sprite.Create( texture,  rect, new Vector2(0.5f,0.5f));
        background.sprite = sprite;
        float scaleFor = 0;

		if (!Data.Instance.lastScene.Equals ("ConfirmArtworkCrop")) {
			Data.Instance.cameraData.Calculate (myCamera);
			switch (Data.Instance.cameraData.aspect) {
			case CameraData.aspects._3_2:
			case CameraData.aspects._4_3:
                scaleFor = scaleFor_4x3;
                background.transform.localScale = new Vector3(scaleFor, scaleFor, 1);
				break;
			default:
                scaleFor = scaleFor_16x9;
                background.transform.localScale = new Vector3(scaleFor, scaleFor, 1);
				break;
			}
		}
    }
}
