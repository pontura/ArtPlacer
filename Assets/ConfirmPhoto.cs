using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public RawImage rawImage;

	void Start ()
    {
        Data.Instance.SetTexture(rawImage, Data.Instance.lastPhotoTexture);
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
