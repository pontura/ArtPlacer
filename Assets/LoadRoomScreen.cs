using UnityEngine;
using System.Collections;

public class LoadRoomScreen : MonoBehaviour {

    void Start()
    {
        Data.Instance.SetMainMenuActive(false);
    }
    public void TakePhoto()
    {
        
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
        Data.Instance.LoadLevel("SavedPhotoBrowser");        
    }
}

