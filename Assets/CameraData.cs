using UnityEngine;
using System.Collections;

public class CameraData : MonoBehaviour {

    public aspects aspect;

    public enum aspects
    {
        _16_9,
        _3_2,
        _4_3
    }
	void Start () {
	
	}
	public void Calculate (Camera camera) {
        if (camera.aspect >= 1.7)
        {
            //Debug.Log("16:9");
            aspect = aspects._16_9;
        }
        else if (camera.aspect >= 1.5)
        {
            //Debug.Log("3:2");
            aspect = aspects._3_2;
        }
        else
        {
            //Debug.Log("4:3");
            aspect = aspects._4_3;
        }
	}
}
