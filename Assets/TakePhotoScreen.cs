using UnityEngine;
using System.Collections;

public class TakePhotoScreen : MonoBehaviour {

	void Start () {
	
	}
    public void TakePhoto()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
