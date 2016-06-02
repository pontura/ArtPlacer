using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public Game game;
    public SpriteRenderer sprite;

    void Start()
    {
        Events.Back += Back;
        Data.Instance.SetTitle("");

        Invoke("OpenTooltip", 0.5f);
        Events.HelpShow();
    }
    void OpenTooltip()
    {
        Events.HelpChangeState(true);
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    void Back()
    {
        if (Data.Instance.isPhoto4Room)
        {
            Data.Instance.LoadLevel("Rooms");
        }
        else
        {
            Data.Instance.LoadLevel("Artworks");
        }
       // Data.Instance.LoadLevel("TakePhoto");
    }
    void Update()
    {
        game.LoadLastPhotoTexture();
    }
    public void Confirm()
    {
		if (Data.Instance.isPhoto4Room) {
			Data.Instance.areaData.Clear ();

			//Data.Instance.SaveRoom ();
			Data.Instance.LoadLevel ("Walls");
		} else {
			Data.Instance.LoadLevel ("ConfirmArtworkCrop");
		}
    }
    public void TurnPhoto()
	{			
        int degrees = (int)sprite.transform.localEulerAngles.z;
        degrees += 90;
        sprite.transform.localEulerAngles = new Vector3(0, 0, degrees);

        Texture2D image;
        if (Data.Instance.isPhoto4Room)
            image = Data.Instance.lastPhotoTexture;
        else 
            image = Data.Instance.lastArtTexture;

        Texture2D rotated = TextureUtils.Rotate90CCW(image);

        if (Data.Instance.isPhoto4Room)
            Data.Instance.lastPhotoTexture = rotated;
        else
            Data.Instance.lastArtTexture = rotated;  		
    }

	public void FlipHPhoto()
	{		
		sprite.flipX = !sprite.flipX;

		Texture2D image;
		if (Data.Instance.isPhoto4Room)
			image = Data.Instance.lastPhotoTexture;
		else 
			image = Data.Instance.lastArtTexture;

		Texture2D flipedH = TextureUtils.FlipH(image);

		if (Data.Instance.isPhoto4Room)
			Data.Instance.lastPhotoTexture = flipedH;
		else
			Data.Instance.lastArtTexture = flipedH;		
	}

	public void FlipVPhoto()
	{		
		sprite.flipY = !sprite.flipY;

		Texture2D image;
		if (Data.Instance.isPhoto4Room)
			image = Data.Instance.lastPhotoTexture;
		else 
			image = Data.Instance.lastArtTexture;

		Texture2D flipedV = TextureUtils.FlipV(image);

		if (Data.Instance.isPhoto4Room)
			Data.Instance.lastPhotoTexture = flipedV;
		else
			Data.Instance.lastArtTexture = flipedV;		
	}
}
