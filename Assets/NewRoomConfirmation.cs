using UnityEngine;
using System.Collections;

public class NewRoomConfirmation : MonoBehaviour {
	  
    void Start()
    {
        Data.Instance.SetTitle("NEW ROOM");
        Data.Instance.SetMainMenuActive(true);
        Events.Back += Back;
        Events.HelpHide();
        Data.Instance.lastArtTexture = null;
        Data.Instance.lastArtThumbTexture = null;
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
    public void Edit()
    {
		Data.Instance.LoadLevel("Artplaced");
    }
    void ReadyJump()
    {
        Data.Instance.LoadLevel("Room");
    }
}

