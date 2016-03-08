using UnityEngine;
using System.Collections;

public class TakePhotoScreen : MonoBehaviour {

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
}
