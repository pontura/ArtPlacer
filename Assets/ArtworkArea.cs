using UnityEngine;
using System.Collections;

public class ArtworkArea : MonoBehaviour {

	public GameObject area;

	bool selected = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (selected) {
			gameObject.transform.position = Input.mousePosition;
		}
	}

	public void OnPointerDown()
	{
		selected = true;
	}

	public void OnPointerUp(){
		selected = false;
	}

	public void expandTopRight(){
		Vector3 scale = Input.mousePosition-gameObject.transform.position;
		scale.Scale (new Vector3 (2,2,0));
		gameObject.transform.localScale = new Vector3 (scale.x, scale.y, 0);
		area.transform.localScale = new Vector3 (scale.x, scale.y, 0);

		print ("TR");
	}
}
