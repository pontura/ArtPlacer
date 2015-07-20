using UnityEngine;
using System.Collections;

public class TakePhotoScreen : MonoBehaviour {

    void Start()
    {
        Data.Instance.SetMainMenuActive(true);
    }
    public void TakePhoto()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
