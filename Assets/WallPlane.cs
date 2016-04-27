using UnityEngine;
using System.Collections;

public class WallPlane : MonoBehaviour {
	
	public GameObject[] pointer;
	public GameObject[] arrows;
	public GameObject area;
	public int AreaId=-1;
    public int areaHeight;

    public GameObject helpField;
	public GameObject artWork;
	Camera cam;
	
	public int select = -1;
	public int lastSelect = -1;
	
	LineRenderer lineRenderer;
	
	public int artWorkNumber = 0;
	
	Mesh areaMesh;

	bool moveArea = false;

	Vector3 offset;

	private Color normalColor = new Color(0.87f,0.13f,0.235f,1f);
	private Color selColor = Color.yellow;

	private float moveStep = 0.005f;
	
	// Use this for initialization
	void Start () {

        GameObject walls = GameObject.Find("walls");
        if(walls && walls.GetComponent<Walls>() && walls.GetComponent<Walls>().totalWalls == 1)
            helpField.SetActive(true);
        else if (helpField != null) 
            helpField.SetActive(false);


		Events.SaveAreas += SaveArea;
		Events.MoveButton += MoveButton;
		Events.ResetPointers += ResetPointers;
		
		foreach (Camera c in Camera.allCameras) {
			if(c.name == "CameraWallArea"){
				cam = c;
				break;
			}
		}		

		areaMesh = area.GetComponent<MeshFilter> ().mesh;
		
		for(int i=0;i<areaMesh.vertexCount;i++){
			Vector3 vertexWorldPos = area.transform.TransformPoint(areaMesh.vertices[i]);
			vertexWorldPos = new Vector3(vertexWorldPos.x,vertexWorldPos.y,vertexWorldPos.z);			
			//pointer[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
			pointer[i].transform.position = vertexWorldPos;
			//Debug.Log ("Punto"+i+": "+pointer[i].transform.position);
			pointer[i].name = "Pointer_"+gameObject.GetInstanceID()+"_"+i;
			if(i<arrows.Length)arrows[i].name = "Arrow_"+gameObject.GetInstanceID()+"_"+i;

		}	
		
		//LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
		//lineRenderer.SetColors(Color.blue,Color.blue);
		//lineRenderer.SetWidth(50F, 50F);
		
		//RedrawLine ();
		if (arrows.Length > 3) {
			UpdateArrow (0);
			UpdateArrow (1);
			UpdateArrow (2);
			UpdateArrow (3);
		}
        Events.OnWallActive(this);
	}
	
	public void EnableAreaCollider(bool enable){
		if (enable) {

			if (areaMesh == null)
				areaMesh = area.GetComponent<MeshFilter> ().mesh;
			area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
		} 
		area.GetComponent<MeshCollider> ().enabled = enable;
	}

	public void EnableMoveArea(bool enable){
		moveArea = enable;
	}
	
	// Update is called once per frame
	void Update () {
		if (Data.Instance.selectedArea == int.MaxValue || Data.Instance.selectedArea == AreaId) {
			if (Input.GetButton ("Fire1")) {
				Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
			
				//Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
				Vector3 worldPos = cam.ScreenToWorldPoint (mousePos);
			
				//Ray screenRay = Camera.main.ScreenPointToRay(mousePos);
				Ray screenRay = cam.ScreenPointToRay (mousePos);
				RaycastHit rayhit;
				if (Physics.Raycast (screenRay, out rayhit)) {				
					if (select < 0) {
						if (rayhit.collider.gameObject == area && moveArea) {
							Events.OnWallEdgeSelected ();
                            Events.OnWallActive(this);
							select = 9;
							Vector3 mPos = Input.mousePosition;
							mPos.z = 1.0f;
							offset = cam.ScreenToWorldPoint (mPos) - transform.position;
							//transform.position = cam.ScreenToWorldPoint(mPos);
							//SetPointersFromArea();
							Data.Instance.selectedArea = AreaId;
							Events.ResetPointers();
							foreach(GameObject go in pointer)go.GetComponent<SpriteRenderer>().color = selColor;
							foreach(GameObject go in arrows)go.GetComponent<SpriteRenderer>().color = selColor;
						} else {
                            if (helpField != null) helpField.SetActive(false);
                            Events.OnWallActive(this);
							Events.OnWallEdgeSelected ();
							//print ("Mesh Id Selected: "+mesh.GetInstanceID());
							for (int i=0; i<areaMesh.vertexCount; i++) {
								if (rayhit.collider.name == "Pointer_" + gameObject.GetInstanceID () + "_" + i) { 
									//Debug.Log ("Pointer Sel: "+i);
									Events.ResetPointers();
									rayhit.collider.gameObject.GetComponent<SpriteRenderer>().color = selColor;
									select = i;
									Data.Instance.selectedArea = GetId ();
									//GameObject localArea = GameObject.Find("Area_" + gameObject.GetInstanceID ());
									Vector3 pos = screenRay.GetPoint (rayhit.distance);                            
									Vector3 areaPos = area.transform.InverseTransformPoint (pos);
								
									Vector3[] vertex = new Vector3[4];
									vertex = areaMesh.vertices;														
								
									vertex [select] = new Vector3 (areaPos.x, areaPos.y, vertex [select].z);
									areaMesh.vertices = vertex;
									if (area.GetComponent<MeshCollider> () != null) {
										area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
										area.GetComponent<MeshCollider> ().enabled = false;
										area.GetComponent<MeshCollider> ().enabled = true;
									}
								
									//pointer[select].transform.position = new Vector3(pos.x,pos.y,-0.001f);
									//GameObject pointer = GameObject.Find("Area_" + gameObject.GetInstanceID ());
									rayhit.collider.transform.position = new Vector3 (pos.x, pos.y, rayhit.collider.transform.parent.transform.position.z);						

								
								}else if (rayhit.collider.name == "Arrow_" + gameObject.GetInstanceID () + "_" + i) { 
									//Debug.Log ("Pointer Sel: "+i);								
									select = 4+i;
									Events.ResetPointers();
									rayhit.collider.gameObject.GetComponent<SpriteRenderer>().color = selColor;
									Data.Instance.selectedArea = GetId ();
									offset = rayhit.collider.transform.position;
									//GameObject localArea = GameObject.Find("Area_" + gameObject.GetInstanceID ());
									/*Vector3 pos = screenRay.GetPoint (rayhit.distance);                            
									Vector3 areaPos = area.transform.InverseTransformPoint (pos);

									rayhit.collider.transform.position = new Vector3 (pos.x, pos.y, rayhit.collider.transform.parent.transform.position.z);						*/

								}
							}
						}
						if(lastSelect!=-1&&lastSelect!=select){
							if(lastSelect<4){
								pointer[lastSelect].GetComponent<SpriteRenderer>().color = normalColor;
							}else if(lastSelect<9){
								arrows[lastSelect-4].GetComponent<SpriteRenderer>().color = normalColor;
							}
						}
					} else {
						if (rayhit.collider.name == "Pointer_" + gameObject.GetInstanceID () + "_" + select) { 						
							//Debug.Log ("Pointer Move: "+select);
							Vector3 pos = screenRay.GetPoint (rayhit.distance);						
							Vector3 areaPos = area.transform.InverseTransformPoint (pos);					
						
							Vector3[] vertex = new Vector3[4];
							vertex = areaMesh.vertices;					
							//vertex[select] = areaPos;						
							vertex [select] = new Vector3 (areaPos.x, areaPos.y, vertex [select].z);
							areaMesh.vertices = vertex;
							if (area.GetComponent<MeshCollider> () != null) {
								area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
								area.GetComponent<MeshCollider> ().enabled = false;
								area.GetComponent<MeshCollider> ().enabled = true;
							}
							rayhit.collider.transform.position = new Vector3 (pos.x, pos.y, rayhit.collider.transform.parent.transform.position.z);
							UpdateArrow(select);
							//rayhit.collider.transform.position = new Vector3(pos.x,pos.y,0f);
							//RedrawLine();					
						}else if (rayhit.collider.name == "Arrow_" + gameObject.GetInstanceID () + "_" + (select-4)) { 						
							//Debug.Log ("Pointer Move: "+select);
							Vector3 pos = screenRay.GetPoint (rayhit.distance);						
							Vector3 areaPos = area.transform.InverseTransformPoint (pos);					
							
							Vector3[] vertex = new Vector3[4];
							vertex = areaMesh.vertices;					
							//vertex[select] = areaPos;						



							if(select-4==0||select-4==2){
								rayhit.collider.transform.position = new Vector3 (pos.x, rayhit.collider.transform.position.y, rayhit.collider.transform.parent.transform.position.z);
								if(select-4==0){
									vertex [3] = new Vector3 (vertex[3].x+(pos.x-offset.x), vertex[3].y, vertex [3].z);
									vertex [0] = new Vector3 (vertex[0].x+(pos.x-offset.x), vertex[0].y, vertex [0].z);
									pointer[3].transform.position = new Vector3 (pointer[3].transform.position.x+(pos.x-offset.x), pointer[3].transform.position.y, pointer[3].transform.position.z);
									UpdateArrow(3);
									pointer[0].transform.position = new Vector3 (pointer[0].transform.position.x+(pos.x-offset.x), pointer[0].transform.position.y, pointer[0].transform.position.z);
									UpdateArrow(0);
								}else if(select-4==2){
									vertex [1] = new Vector3 (vertex[1].x+(pos.x-offset.x), vertex[1].y, vertex [1].z);
									vertex [2] = new Vector3 (vertex[2].x+(pos.x-offset.x), vertex[2].y, vertex [2].z);
									pointer[1].transform.position = new Vector3 (pointer[1].transform.position.x+(pos.x-offset.x), pointer[1].transform.position.y, pointer[3].transform.position.z);
									UpdateArrow(1);
									pointer[2].transform.position = new Vector3 (pointer[2].transform.position.x+(pos.x-offset.x), pointer[2].transform.position.y, pointer[0].transform.position.z);
									UpdateArrow(2);
								}
							}else{
								if(select-4==1){
									vertex [2] = new Vector3 (vertex[2].x, vertex[2].y+(pos.y-offset.y), vertex [2].z);
									vertex [0] = new Vector3 (vertex[0].x, vertex[0].y+(pos.y-offset.y), vertex [0].z);
									pointer[2].transform.position = new Vector3 (pointer[2].transform.position.x, pointer[2].transform.position.y+(pos.y-offset.y), pointer[2].transform.position.z);
									UpdateArrow(2);
									pointer[0].transform.position = new Vector3 (pointer[0].transform.position.x, pointer[0].transform.position.y+(pos.y-offset.y), pointer[0].transform.position.z);
									UpdateArrow(0);
								}else if(select-4==3){
									vertex [3] = new Vector3 (vertex[3].x, vertex[3].y+(pos.y-offset.y), vertex [3].z);
									vertex [1] = new Vector3 (vertex[1].x, vertex[1].y+(pos.y-offset.y), vertex [1].z);
									pointer[3].transform.position = new Vector3 (pointer[3].transform.position.x, pointer[3].transform.position.y+(pos.y-offset.y), pointer[3].transform.position.z);
									UpdateArrow(3);
									pointer[1].transform.position = new Vector3 (pointer[1].transform.position.x, pointer[1].transform.position.y+(pos.y-offset.y), pointer[1].transform.position.z);
									UpdateArrow(1);
								}
								rayhit.collider.transform.position = new Vector3 (rayhit.collider.transform.position.x, pos.y, rayhit.collider.transform.parent.transform.position.z);
							}
							areaMesh.vertices = vertex;
							if (area.GetComponent<MeshCollider> () != null) {
								area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
								area.GetComponent<MeshCollider> ().enabled = false;
								area.GetComponent<MeshCollider> ().enabled = true;
							}
							//rayhit.collider.transform.position = new Vector3(pos.x,pos.y,0f);
							//RedrawLine();
							offset = pos;
						} else if (moveArea&&select==9) {
							Vector3 mPos = Input.mousePosition;
							mPos.z = 1.0f;
							transform.position = cam.ScreenToWorldPoint (mPos) - offset;
							//SetPointersFromArea();
						}
					}
				}
			} else {
				if (select > -1) {
					lastSelect = select;
					select = -1;
				}
				Data.Instance.selectedArea = int.MaxValue;
			}
		}
		
		if (transform.hasChanged){
			//RedrawLine();
		}
	}
	
	public void SetId(int id){
		AreaId = id;
		gameObject.name = "CreatedPlane_" + AreaId;		
	}

	public int GetId(){
		return AreaId;
	}
	
	void RedrawLine(){
		int lineSize1 = (int)Vector3.Distance(pointer[0].transform.position,pointer[2].transform.position);
		int lineSize2 = (int)Vector3.Distance(pointer[2].transform.position,pointer[1].transform.position);
		int lineSize3 = (int)Vector3.Distance(pointer[1].transform.position,pointer[3].transform.position);
		int lineSize4 = (int)Vector3.Distance(pointer[3].transform.position,pointer[0].transform.position);
		int totalLineS = lineSize1 + lineSize2 + lineSize3 + lineSize4;
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
		
		lineRenderer.SetColors(Color.black, Color.black);
		lineRenderer.SetVertexCount(totalLineS);
		for(int j=0;j<totalLineS;j++){
			if(j<lineSize1){
				lineRenderer.SetPosition(j, Vector3.Lerp(pointer[0].transform.position,pointer[2].transform.position,1f*j/lineSize1));
			}else if(j<lineSize1+lineSize2){
				lineRenderer.SetPosition(j, Vector3.Lerp(pointer[2].transform.position,pointer[1].transform.position,1f*(j-lineSize1)/lineSize2));		
			}else if(j<lineSize1+lineSize2+lineSize3){
				lineRenderer.SetPosition(j, Vector3.Lerp(pointer[1].transform.position,pointer[3].transform.position,1f*(j-lineSize1-lineSize2)/lineSize3));		
			}else if(j<lineSize1+lineSize2+lineSize3+lineSize4){
				lineRenderer.SetPosition(j, Vector3.Lerp(pointer[3].transform.position,pointer[0].transform.position,1f*(j-lineSize1-lineSize2-lineSize4)/lineSize4));		
			}
		}
	}
	
	void SaveArea()
	{
        float areaWidth= GetComponent<SizeCalculator>().CalculateWidth(areaHeight);
        Data.Instance.AddArea(AreaId, areaMesh.vertices, transform.position, areaWidth, areaHeight);
	}

	
	void OnDestroy()
	{
		Events.SaveAreas -= SaveArea;
		Events.MoveButton -= MoveButton;
		Events.ResetPointers -= ResetPointers;
	}

	void UpdateArrow(int select){
		switch (select) {
		case 0:
			arrows [0].transform.position = 0.5f * (pointer [0].transform.position + pointer [3].transform.position);
			arrows [1].transform.position = 0.5f * (pointer [0].transform.position + pointer [2].transform.position);
			break;
		case 1:
			arrows [2].transform.position = 0.5f * (pointer [2].transform.position + pointer [1].transform.position);
			arrows [3].transform.position = 0.5f * (pointer [3].transform.position + pointer [1].transform.position);
			break;
		case 2:
			arrows [2].transform.position = 0.5f * (pointer [2].transform.position + pointer [1].transform.position);
			arrows [1].transform.position = 0.5f * (pointer [2].transform.position + pointer [0].transform.position);
			break;
		case 3:
			arrows [0].transform.position = 0.5f * (pointer [3].transform.position + pointer [0].transform.position);
			arrows [3].transform.position = 0.5f * (pointer [3].transform.position + pointer [1].transform.position);
			break;
		default:
			break;
		}
	}

	void MoveButton(int moveId){

		if (lastSelect > -1) {
			Vector3[] vertex = new Vector3[4];
			vertex = areaMesh.vertices;
			Vector3 temp1,temp2;
			Vector3 posDif = Vector3.zero;

			if (moveId == 1) {//LEFT
				posDif = new Vector3(-moveStep,0f,0f);
			} else if (moveId == 2) {//RIGHT
				posDif = new Vector3(moveStep,0f,0f);
			} else if (moveId == 3) {//UP
				posDif = new Vector3(0f,moveStep,0f);
			} else if (moveId == 4) {//DOWN
				posDif = new Vector3(0f,-moveStep,0f);
			}


			if (lastSelect < 4) {				
				vertex[lastSelect]+=posDif;				
				areaMesh.vertices = vertex;
				if (area.GetComponent<MeshCollider> () != null) {
					area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
					area.GetComponent<MeshCollider> ().enabled = false;
					area.GetComponent<MeshCollider> ().enabled = true;
				}
				pointer [lastSelect].transform.position += posDif;
				UpdateArrow (lastSelect);				
			} else if (lastSelect < 9){
				if(lastSelect-4==0||lastSelect-4==2){
					if(lastSelect-4==0){
						vertex [3] += posDif;
						vertex [0] += posDif;
						pointer[3].transform.position += posDif;
						UpdateArrow(3);
						pointer[0].transform.position += posDif;
						UpdateArrow(0);
					}else if(lastSelect-4==2){
						vertex [1] += posDif;
						vertex [2] += posDif;
						pointer[1].transform.position += posDif;
						UpdateArrow(1);
						pointer[2].transform.position += posDif;
						UpdateArrow(2);
					}
				}else{
					if(lastSelect-4==1){
						vertex [2] += posDif;
						vertex [0] += posDif;
						pointer[2].transform.position += posDif;
						UpdateArrow(2);
						pointer[0].transform.position += posDif;
						UpdateArrow(0);
					}else if(lastSelect-4==3){
						vertex [3] += posDif;
						vertex [1] += posDif;
						pointer[3].transform.position += posDif;
						UpdateArrow(3);
						pointer[1].transform.position += posDif;
						UpdateArrow(1);
					}

				}
				areaMesh.vertices = vertex;
				if (area.GetComponent<MeshCollider> () != null) {
					area.GetComponent<MeshCollider> ().sharedMesh = areaMesh;
					area.GetComponent<MeshCollider> ().enabled = false;
					area.GetComponent<MeshCollider> ().enabled = true;
				}

			}else if(lastSelect == 9){
				transform.position += posDif;
			}

		}
	}

	void ResetPointers(){
		if (lastSelect > -1) {
			if (lastSelect < 4) {
				pointer [lastSelect].GetComponent<SpriteRenderer> ().color = normalColor;
			} else if (lastSelect < 9) {
				arrows [lastSelect - 4].GetComponent<SpriteRenderer> ().color = normalColor;
			}else if(lastSelect==9){
				foreach(GameObject go in pointer)go.GetComponent<SpriteRenderer>().color = normalColor;
				foreach(GameObject go in arrows)go.GetComponent<SpriteRenderer>().color = normalColor;
			}
			lastSelect = -1;
		}
	}
	
}
