using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public GameObject helpButton;
    private bool helpWasActive;
    private bool open;
    public GameObject fullVersionAsset;

    void Start()
    {
        Events.ToggleUnit += ToggleUnit;
        //fullVersionAsset.SetActive(!StoreData.Instance.fullVersion);
    }
    void ToggleUnit()
    {
        if (open)
            Close();
    }
    public void Open()
    {
        open = true;
        GetComponent<Animation>().Play("MainMenuOpen");
        if (helpButton.activeSelf)
        {
            helpWasActive = true;
            helpButton.SetActive(false);
        }
        else
        {
            helpWasActive = false;
        }
    }
    public void NewRoom()
    {
		Data.Instance.isPhoto4Room=true;
        if (Data.Instance.areaData.areas.Count == 0)        {

            Data.Instance.LoadLevel("Rooms");
        }
        else
        {
            if (Data.Instance.roomsData.changesMade)
                Data.Instance.LoadLevel("NewRoomConfirmation");
            else
                Data.Instance.LoadLevel("Rooms");
        }
        Close();
    }
    public void Rooms()
    {	
        Data.Instance.roomsData.type = RoomsData.types.LOCAL;
		Data.Instance.lastArtTexture = null;
		Data.Instance.isPhoto4Room=true;
        Data.Instance.LoadLevel("Rooms");
        Close();
    }
    public void Galleries()
    {
		Data.Instance.LoadLevel("SelectArtworks");
        Close();
    }
    public void Close()
    {
        open = false;
        GetComponent<Animation>().Play("MainMenuClose");
        Invoke("ResetClose", 0.5f);
        if (helpWasActive)
        {
            helpButton.SetActive(true);
        }
    }
    void ResetClose()
    {
        gameObject.SetActive(false);
    }
    public void FullVersionClicked()
    {
        Data.Instance.LoadLevel("FullVersion");
        Close();
    }
}
