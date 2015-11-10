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
        Data.Instance.LoadLevel("Rooms");
    }
    public void TakePhoto()
    {
        Data.Instance.LoadLevel("ConfirmPhoto");
    }
}
