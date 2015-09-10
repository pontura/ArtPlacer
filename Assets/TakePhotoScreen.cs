using UnityEngine;
using System.Collections;

public class TakePhotoScreen : MonoBehaviour {

    void Start()
    {
        Data.Instance.SetMainMenuActive(true);
        Data.Instance.cameraData.Calculate(GetComponent<Camera>());
    }
    public void TakePhoto()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
