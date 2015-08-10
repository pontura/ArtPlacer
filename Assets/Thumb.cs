using UnityEngine;
using System.Collections;

public class Thumb : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mPos = Input.mousePosition;
		mPos.z = 1.0f;
		gameObject.transform.position = Camera.main.ScreenToWorldPoint(mPos);	
	}
}
