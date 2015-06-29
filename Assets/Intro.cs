using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

    public void LoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
}
