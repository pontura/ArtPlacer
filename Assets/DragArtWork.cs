using UnityEngine;
using System.Collections;

public class DragArtWork : MonoBehaviour {

	Camera cam;
	bool selected = false;
	public Vector3 positionOrigin;
	public bool showZone;
	public Material material;
	public int areaIndex = -1;
	public int artWorkIndex = -1;
	public int artWorkID = -1;
	GameObject zone;


	// Use this for initialization
	void Start () {

		foreach (Camera c in Camera.allCameras) {
			if(c.name == "CameraWallArea"){
				cam = c;
				break;
			}
		}

		positionOrigin = gameObject.transform.parent.transform.position;

		if (showZone) {
			zone = GameObject.CreatePrimitive(PrimitiveType.Quad);
			zone.GetComponent<Renderer>().material = material;
			zone.transform.localScale = new Vector3(0.2f,0.2f,1f);
			zone.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.parent.transform.position.z-0.1f);
			zone.transform.parent = gameObject.transform.parent.transform;
			zone.name = "Cursor_"+gameObject.name;
		}
	}
	
	// Update is called once per frame
	/*void Update () {

		if( Input.GetButton ("Fire1")) {
			Vector3 mousePos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
			
			//Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
			
			//Ray screenRay = Camera.main.ScreenPointToRay(mousePos);
			Ray screenRay = cam.ScreenPointToRay(mousePos);
			RaycastHit rayhit;
			if(Physics.Raycast(screenRay, out rayhit)){
				//Mesh mesh = GameObject.Find("Area_" + gameObject.GetInstanceID ()).GetComponent<MeshFilter> ().mesh;
				Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
				
				if(rayhit.collider == gameObject.GetComponent<Collider>()){ 
							
					selected= true;
					//print ("MousePos: "+mousePos);
					Vector3 pos = screenRay.GetPoint(rayhit.distance);
					//print ("Pos: "+pos);
					Vector3 areaPos = gameObject.transform.parent.transform.InverseTransformPoint(pos);
					//print ("AreaPos: "+areaPos);

					Vector3 parentPos = gameObject.transform.parent.transform.position;
					//print ("ParentPos: "+parentPos);
					//print ("GameOPos: "+gameObject.transform.position);
					gameObject.transform.position = new Vector3(positionOrigin.x+areaPos.x,positionOrigin.y+areaPos.y,positionOrigin.z);
					zone.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.parent.transform.position.z-0.1f);
				}
			}
		}else{
			if(selected){
				selected=false;
				Data.Instance.areaData.areas[areaId].artworks[artWorkId].position = gameObject.transform.position;
			}
		}
	
	}*/
}
