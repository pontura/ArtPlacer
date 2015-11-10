﻿using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public GameObject helpButton;
    private bool helpWasActive;

    public void Open()
    {
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
        if (helpWasActive)
        {
            helpButton.SetActive(true);
        }
    }
    void ResetClose()
    {
        gameObject.SetActive(false);
    }
}
