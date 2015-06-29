using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Walls : MonoBehaviour {

    public RawImage rawImage;

	void Start () {
        rawImage.texture = Data.Instance.lastPhotoTexture;
	}
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
}
