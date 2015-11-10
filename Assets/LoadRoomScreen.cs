using UnityEngine;
using System.Collections;

public class LoadRoomScreen : MonoBehaviour {
	  
    void Start()
    {
        Data.Instance.SetTitle("PLACE ARTWORK");
        Data.Instance.SetMainMenuActive(true);
        Events.Back += Back;
        Events.HelpHide();
    }
    public void SelectRoom()
    {
		Data.Instance.isPhoto4Room = true;
        Data.Instance.LoadLevel("Rooms");
    }
	void Back()
    {
        Data.Instance.LoadLevel("Artworks");
    }
	void OnDestroy(){
        Events.Back -= Back;
	}
}

