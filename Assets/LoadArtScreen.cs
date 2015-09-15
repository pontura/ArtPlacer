using UnityEngine;
using System.Collections;

public class LoadArtScreen : MonoBehaviour {

	public void TakePhoto()
    {
		Data.Instance.isPhoto4Room = false;
        Data.Instance.LoadLevel("TakePhoto");
    }
    public void Browse()
    {
		Debug.Log ("Browse");      
    }    
    public void Back()
    {
        Data.Instance.Back();
    }
}

