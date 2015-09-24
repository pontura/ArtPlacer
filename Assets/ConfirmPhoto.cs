using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public Game game;
    public Animation toolTipConfirm;
	public GameObject photoImage;
	public GameObject loadImage;

    void Start()
    {
        toolTipConfirm.gameObject.SetActive(false);
        Invoke("OpenTooltip", 0.7f);
		if (Data.Instance.lastScene.Equals ("TakePhoto")) {
			photoImage.gameObject.SetActive (true);
			loadImage.gameObject.SetActive (false);
		} else {
			photoImage.gameObject.SetActive (false);
			loadImage.gameObject.SetActive (true);
		}
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
    void OpenTooltip()
    {
        toolTipConfirm.gameObject.SetActive(true);
        toolTipConfirm.Play("tooltipOn");
    }
    public void Back()
    {
        //Data.Instance.LoadLevel("TakePhoto");Data.Instance.lastScene
		Data.Instance.LoadLevel (Data.Instance.lastScene);
    }
}
