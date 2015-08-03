using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public SpriteRenderer background;
    private bool photoLoadead;

	void Start () {
        LoadLastPhotoTexture();
	}
	
	// Update is called once per frame
	public void LoadLastPhotoTexture () {

        if (photoLoadead) return;
        photoLoadead = true;

        Sprite sprite = Sprite.Create(Data.Instance.lastPhotoTexture,  new Rect(0, 0, 640, 480), new Vector2(0.5f,0.5f));
        background.sprite = sprite;
	}
}
