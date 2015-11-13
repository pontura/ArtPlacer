using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public Game game;

    void Start()
    {
        Events.Back += Back;
        Data.Instance.SetTitle("");

        Invoke("OpenTooltip", 0.7f);
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    void Back()
    {
        Data.Instance.LoadLevel("TakePhoto");
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
}
