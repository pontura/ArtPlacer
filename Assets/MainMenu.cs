using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public void Open()
    {
        GetComponent<Animation>().Play("MainMenuOpen");
    }
    public void NewRoom()
    {
        Data.Instance.LoadLevel("TakePhoto");
        Close();
    }
    public void PublicRooms()
    {
        Data.Instance.roomsData.type = RoomsData.types.ONLINE;
        Data.Instance.LoadLevel("SavedPhotoBrowser");
        Close();
    }
    public void MyRooms()
    {
        Data.Instance.roomsData.type = RoomsData.types.LOCAL;
        Data.Instance.LoadLevel("SavedPhotoBrowser");
        Close();
    }

    public void Close()
    {
        GetComponent<Animation>().Play("MainMenuClose");
        Invoke("ResetClose", 0.5f);
    }
    void ResetClose()
    {
        gameObject.SetActive(false);
    }
}
