using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public void Open()
    {
        GetComponent<Animation>().Play("MainMenuOpen");
    }
    public void NewRoom()
    {
        Data.Instance.LoadLevel("Rooms");
        Close();
    }
    public void Rooms()
    {	
        Data.Instance.roomsData.type = RoomsData.types.LOCAL;
		Data.Instance.lastArtTexture = null;
        Data.Instance.LoadLevel("Rooms");
        Close();
    }
    public void Galleries()
    {
        Data.Instance.LoadLevel("Galleries");
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
