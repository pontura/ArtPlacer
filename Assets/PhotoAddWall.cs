using UnityEngine;
using System.Collections;

public class PhotoAddWall : MonoBehaviour {

    public Game gameContainer;
	public GameObject sector;
	public RectTransform menuArea;
	public bool add = true;
	public int selWall;
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
		Events.OnWallActive += OnWallActive;
	}

	void OnDestroy()
	{
		Events.OnWallActive -= OnWallActive;
	}

	void OnWallActive(WallPlane _wallPlane)
	{
		selWall =  _wallPlane.AreaId;
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
				//print ("mouse: "+mousePos);
				if(mousePos.x<menuArea.position.x+menuArea.rect.xMin){				
				
					Vector3 worldPos = cam.ScreenToWorldPoint (mousePos);
					//print ("worldPos: "+worldPos);
					GameObject obj = Instantiate (sector, new Vector3 (worldPos.x, worldPos.y, 1f), Quaternion.identity) as GameObject;
					//GameObject obj = Instantiate (sector, new Vector3 (mousePos.x, mousePos.y, 9.9f), Quaternion.identity) as GameObject;
					//GameObject obj = Instantiate (sector, new Vector3 (0, 0, 10), Quaternion.identity) as GameObject;

	                obj.transform.SetParent(gameContainer.transform);
					WallPlane wp = obj.GetComponent<WallPlane>();

					add=false;
	                numWalls++;
					wp.SetId(-1*numWalls);
					wp.EnableAreaCollider(true);
					wp.EnableMoveArea(true);
	                Events.OnNumWallsChanged(numWalls);
				}
			}
		}		
	}

	public void RemoveArea(){
		Events.OnWallEdgeUnSelected ();
		GameObject obj = GameObject.Find ("CreatedPlane_" + selWall);
		GameObject.Destroy (obj);
		if (selWall<0) {			
			numWalls--;
			Events.OnNumWallsChanged (numWalls);
		} else {			
			Data.Instance.areaData.areas.RemoveAt(selWall);
			if (Data.Instance.areaData.areas.Count == 0) {
				Events.OnNumWallsChanged (0);
			} else {
				Events.OnRemoveArea ();
			}
		}
	}
}
