using UnityEngine;
using System.Collections;

public class WallPlane : MonoBehaviour {
    
	public float zPointers = 0;

	public GameObject[] pointer;
	public GameObject area;
	public int AreaId=-1;
	
	public GameObject artWork;
	Camera cam;
	
	int select = -1;

	LineRenderer lineRenderer;

	public int artWorkNumber = 0;
		
	// Use this for initialization
	void Start () {

		Events.SaveAreas += SaveArea;

		foreach (Camera c in Camera.allCameras) {
			if(c.name == "CameraWallArea"){
				cam = c;
				break;
			}
		}

		//area.name = "Area_" + gameObject.GetInstanceID ();


		//MeshFilter mf = GameObject.Find("Area_" + gameObject.GetInstanceID ()).GetComponent<MeshFilter> ();
		MeshFilter mf = area.GetComponent<MeshFilter> ();

		for(int i=0;i<mf.mesh.vertexCount;i++){
            Vector3 vertexWorldPos = area.transform.TransformPoint(mf.mesh.vertices[i]);
			vertexWorldPos = new Vector3(vertexWorldPos.x,vertexWorldPos.y,vertexWorldPos.z);

			//Instantiate(pointer[i], vertexWorldPos, Quaternion.identity);
			//pointer[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
			pointer[i].transform.position = vertexWorldPos;
			//pointer[i].transform.localScale += new Vector3(200F, 200F, 0);
			//pointer[i].tag = "Pointer"+i;
			//pointer[i].transform.parent = transform;
			//Debug.Log ("Punto"+i+": "+pointer[i].transform.position);
			pointer[i].name = "Pointer_"+gameObject.GetInstanceID()+"_"+i;

		}	

		//LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
		//lineRenderer.SetColors(Color.blue,Color.blue);
		//lineRenderer.SetWidth(50F, 50F);

		//RedrawLine ();
	
	}
	
	// Update is called once per frame
	void Update () {
					
		if( Input.GetButton ("Fire1")) {
			Vector3 mousePos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);

			//Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
			
			//Ray screenRay = Camera.main.ScreenPointToRay(mousePos);
			Ray screenRay = cam.ScreenPointToRay(mousePos);
			RaycastHit rayhit;
			if(Physics.Raycast(screenRay, out rayhit)){
				//Mesh mesh = GameObject.Find("Area_" + gameObject.GetInstanceID ()).GetComponent<MeshFilter> ().mesh;
				Mesh mesh = area.GetComponent<MeshFilter> ().mesh;
				
				if(select<0){
					//print ("Mesh Id Selected: "+mesh.GetInstanceID());
					for(int i=0;i<mesh.vertexCount;i++){
						if(rayhit.collider.name == "Pointer_"+gameObject.GetInstanceID()+"_"+i){ 
							//Debug.Log ("Pointer Sel: "+i);
							
							select= i;
							//GameObject localArea = GameObject.Find("Area_" + gameObject.GetInstanceID ());
							Vector3 pos = screenRay.GetPoint(rayhit.distance);                            
							Vector3 areaPos = area.transform.InverseTransformPoint(pos);
							
							Vector3[] vertex = new Vector3[4];
							vertex = mesh.vertices;														
							
							vertex[select] = new Vector3(areaPos.x,areaPos.y,vertex[select].z);
							mesh.vertices = vertex;
							//pointer[select].transform.position = new Vector3(pos.x,pos.y,-0.001f);
							//GameObject pointer = GameObject.Find("Area_" + gameObject.GetInstanceID ());
							rayhit.collider.transform.position = new Vector3(pos.x,pos.y,rayhit.collider.transform.parent.transform.position.z);						
							
						}
					}
				}else{
					if(rayhit.collider.name == "Pointer_"+gameObject.GetInstanceID()+"_"+select){ 

						//Debug.Log ("Pointer Move: "+select);
						Vector3 pos = screenRay.GetPoint(rayhit.distance);						
						Vector3 areaPos = area.transform.InverseTransformPoint(pos);					
						
						Vector3[] vertex = new Vector3[4];
						vertex = mesh.vertices;					
						//vertex[select] = areaPos;						
						vertex[select] = new Vector3(areaPos.x,areaPos.y,vertex[select].z);
						mesh.vertices = vertex;
						rayhit.collider.transform.position = new Vector3(pos.x,pos.y,rayhit.collider.transform.parent.transform.position.z);
						//rayhit.collider.transform.position = new Vector3(pos.x,pos.y,0f);
						//RedrawLine();					
					}
				}
			}
		}else{
			if(select>-1){
				select=-1;
				//Debug.Log ("Pointer Reset");
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
		Mesh mesh = area.GetComponent<MeshFilter> ().mesh;
		/*Debug.Log("AreaZ_0: "+ mesh.vertices[0].z);
		Vector3[] vertices = new Vector3[4];
		for (int i=0; i<vertices.Length; i++)
			vertices [i] = area.transform.TransformPoint(mesh.vertices[i]);			
		Debug.Log("AreaZ_1: "+ vertices[0].z);*/
		Data.Instance.AddArea (AreaId, mesh.vertices, transform.position, 1.0f, 1.0f);
		//Data.Instance.areaData.AddAreas (AreaId, mesh.vertices, transform.position, 0);
	}

	void OnDestroy()
	{
		Events.SaveAreas -= SaveArea;
	}

}