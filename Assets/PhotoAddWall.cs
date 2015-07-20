using UnityEngine;
using System.Collections;

public class PhotoAddWall : MonoBehaviour {

	public GameObject sector;
	public bool add = true;
    private int numWalls;
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
    public void ActiveAdd()
    {
        add = true;
    }
	public void DeactiveAdd()
	{
		add = false;
	}
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
                numWalls++;
				obj.GetComponent<WallPlane>().SetId(-1*numWalls);
                Events.OnNumWallsChanged(numWalls);
			}
		}		
	}

	public void RemoveArea(){
		if (numWalls > 0) {
			GameObject obj = GameObject.Find ("CreatedPlane_" + (-1 * numWalls));
			GameObject.Destroy (obj);
			numWalls--;
			Events.OnNumWallsChanged (numWalls);
		} else {
			int last = Data.Instance.areaData.areas.Count - 1;
			GameObject obj = GameObject.Find ("CreatedPlane_" + last);
			GameObject.Destroy (obj);
            Data.Instance.areaData.areas.RemoveAt(last);
            if (Data.Instance.areaData.areas.Count == 0)
            {
				Events.OnNumWallsChanged (0);
			}
		}
	}
}
