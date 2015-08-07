using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public SpriteRenderer background;
    private bool photoLoadead;
    public Camera myCamera;

    public float scaleFor_4x3 = 0.43f;
    public float scaleFor_16x9 = 0.48f;  

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

        Sprite sprite = Sprite.Create(Data.Instance.lastPhotoTexture,  new Rect(0, 0, 640, 480), new Vector2(0.5f,0.5f));
        background.sprite = sprite;

        background.transform.localScale = new Vector3(scaleFor_16x9, scaleFor_16x9, scaleFor_16x9);
	}
}
