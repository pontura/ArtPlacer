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

        if (!Data.Instance.isPhoto4Room)
            texture = Data.Instance.lastArtTexture;

		//Debug.Log ("TW: " + texture.width + " TH: " + texture.height);

		Rect rect = new Rect();

		float maxWidth = Data.Instance.defaultCamSize.x;
		float maxHeight = Data.Instance.defaultCamSize.y;
		float aspect = maxWidth / maxHeight;
		float textAspect = 1f*texture.width / texture.height;
		if (aspect > textAspect) {
			rect = new Rect (0, 0, (maxHeight * textAspect), maxHeight);
			texture = TextureUtils.ResizeTexture(texture,TextureUtils.ImageFilterMode.Nearest,maxHeight/texture.height);
		} else if (aspect < textAspect) {
			rect = new Rect (0, 0, maxWidth, (maxWidth / textAspect));
			texture = TextureUtils.ResizeTexture(texture,TextureUtils.ImageFilterMode.Nearest,maxWidth/texture.width);
		} else {
			rect = new Rect (0, 0, (int)(maxWidth), (int)(maxHeight));
			//texture.Resize(texture.width,texture.height);
			//texture.Apply();
		}

		Sprite sprite = Sprite.Create( texture,  rect, new Vector2(0.5f,0.5f));
        background.sprite = sprite;

        Data.Instance.cameraData.Calculate(myCamera);
        switch (Data.Instance.cameraData.aspect)
        {
            case CameraData.aspects._3_2:
            case CameraData.aspects._4_3:
                background.transform.localScale = new Vector3(scaleFor_4x3, scaleFor_4x3, 1);
                break;
            default:
                background.transform.localScale = new Vector3(scaleFor_16x9, scaleFor_16x9, 1);
                break;
        }

    }
}
