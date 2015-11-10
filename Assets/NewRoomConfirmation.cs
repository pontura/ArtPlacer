using UnityEngine;
using System.Collections;

public class NewRoomConfirmation : MonoBehaviour {
	  
    void Start()
    {
        Data.Instance.SetTitle("NEW ROOM");
        Data.Instance.SetMainMenuActive(true);
        Events.Back += Back;
        Events.HelpHide();
    }
    public void Discard()
    {
        Back();
    }
	void Back()
    {
        Data.Instance.LoadLevel("Rooms");
    }
	void OnDestroy(){
        Events.Back -= Back;
	}
    public void Save()
    {
        Events.OnLoading(true);
        Data.Instance.SaveRoom();
        Data.Instance.lastArtTexture = null;
        Data.Instance.areaData.areas.Clear();
        Invoke("ReadyJump", 1);
    }
    void ReadyJump()
    {
        Data.Instance.LoadLevel("Room");
    }
}

