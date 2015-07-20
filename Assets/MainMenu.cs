using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    void Start()
    {
        GetComponent<Animation>().Play("MainMenuOpen");
    }
    public void NewRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }

    public void Close()
    {
        GetComponent<Animation>().Play("MainMenuClose");
        Invoke("ResetClose", 0.5f);
    }
    void ResetClose()
    {
        Destroy(gameObject);
    }
}
