using UnityEngine;
using System.Collections;

public class PhotoAddWall : MonoBehaviour {

	public GameObject sector;
	public bool add = true;

	Camera cam;

	// Use this for initialization
	void Start () {
		foreach (Camera c in Camera.allCameras) {
			if(c.name == "CameraWallArea"){
				cam = c;
				break;
			}
		}
	}
	
	// Update is called once per frame

	void Update () {
		if (add) {
			//if (Input.GetKeyDown(KeyCode.Space)) {
			if( Input.GetButton ("Fire1")) {
				Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
				print ("mouse: "+mousePos);
				Vector3 worldPos = cam.ScreenToWorldPoint (mousePos);
				print ("worldPos: "+worldPos);
				GameObject obj = Instantiate (sector, new Vector3 (worldPos.x, worldPos.y, 9.9f), Quaternion.identity) as GameObject;
				//GameObject obj = Instantiate (sector, new Vector3 (mousePos.x, mousePos.y, 9.9f), Quaternion.identity) as GameObject;
				//GameObject obj = Instantiate (sector, new Vector3 (0, 0, 10), Quaternion.identity) as GameObject;
				add=false;
			}
		}
		
		
		
	}
}
