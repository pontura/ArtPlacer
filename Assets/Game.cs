using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public SpriteRenderer background;
    private bool photoLoadead;
    public Camera myCamera;

    private float scaleFor_4x3 = 0.43f;
    private float scaleFor_16x9 = 0.48f;  

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

		Sprite sprite = Sprite.Create( texture,  new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
        background.sprite = sprite;

        background.transform.localScale = new Vector3(scaleFor_16x9, scaleFor_16x9, scaleFor_16x9);

#if UNITY_IOS
        background.transform.localScale = new Vector3(scaleFor_4x3, scaleFor_4x3, scaleFor_4x3);
#endif
#if UNITY_IPHONE
        background.transform.localScale = new Vector3(scaleFor_16x9, scaleFor_16x9, scaleFor_16x9);
#endif

    }
}
