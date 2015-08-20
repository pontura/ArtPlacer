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
        Data.Instance.roomsData.type = RoomsData.types.ONLINE;
        Data.Instance.LoadLevel("Rooms");        
    }
    public void Open()
    {
        Data.Instance.roomsData.type = RoomsData.types.LOCAL;
        Data.Instance.LoadLevel("Rooms");
    }
    public void Back()
    {
        Data.Instance.Back();
    }
}

