using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

    void Start()
    {
        Data.Instance.SetMainMenuActive(false);
    }
    public void LoadRoom()
    {
        Data.Instance.LoadLevel("Rooms");
    }
    public void Galleries()
    {
        Data.Instance.LoadLevel("Galleries");
    }
}
