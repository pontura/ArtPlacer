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
}
