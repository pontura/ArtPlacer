using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TakePhotoScreen : MonoBehaviour {

	public Slider zSlider;

    void Start()
    {
        Events.HelpHide();
        Events.Back += Back;
        Data.Instance.SetTitle("");
        Data.Instance.SetMainMenuActive(true);
        Data.Instance.cameraData.Calculate(GetComponent<Camera>());
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    void Back()
    {
        if(Data.Instance.isPhoto4Room)
            Data.Instance.LoadLevel("Rooms");
        else
            Data.Instance.LoadLevel("Artworks");
    }
    public void TakePhoto()
    {
        Data.Instance.LoadLevel("ConfirmPhoto");
    }

	public void OnZoom () {		
		Events.OnZoom (zSlider.value);
	}
}
