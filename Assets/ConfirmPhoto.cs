using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPhoto : MonoBehaviour {

    public RawImage rawImage;

	void Start ()
    {
        rawImage.texture = Data.Instance.lastPhotoTexture;        
#if UNITY_IOS
       rawImage.transform.localScale = new Vector3(1, -1, 1);
#endif

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
