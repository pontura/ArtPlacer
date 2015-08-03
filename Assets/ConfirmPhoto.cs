using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public Game game;

    void Update()
    {
      //  game.LoadLastPhotoTexture();
    }
    public void Confirm()
    {
        Data.Instance.SavePhotoTaken();
        Data.Instance.LoadLevel("Walls");
    }
    public void Back()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
