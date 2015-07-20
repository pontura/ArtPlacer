using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public void Open()
    {
        GetComponent<Animation>().Play("MainMenuOpen");
    }
    public void NewRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
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
