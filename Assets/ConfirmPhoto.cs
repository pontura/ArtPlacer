using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public Game game;
    public Animation toolTipConfirm;

    void Start()
    {
        toolTipConfirm.gameObject.SetActive(false);
        Invoke("OpenTooltip", 0.7f);
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
        Data.Instance.LoadLevel("TakePhoto");
    }
}
