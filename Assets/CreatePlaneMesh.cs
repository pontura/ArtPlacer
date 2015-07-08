using UnityEngine;
using System.Collections;

public class CreatePlaneMesh : MonoBehaviour {

	public float width = 1000f;
	public float height = 1000f;
	//public Mesh mesh;
	public GameObject[] pointer;

	int select = -1;

	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter> ();
		//mesh = new Mesh ();
		//mf.mesh = mesh;
		//mesh = mf.mesh;

		//foreach (Vector3 vertex in mesh.vertices) {
		for(int i=0;i<mf.mesh.vertexCount;i++){
			Vector3 vertexWorldPos = transform.TransformPoint(mf.mesh.vertices[i]);
			vertexWorldPos = new Vector3(vertexWorldPos.x,vertexWorldPos.y,-0.01f);
			//Debug.DrawLine (transform.position, vertexWorldPos, Color.blue, 10.0f);
			//Debug.Log (vertexWorldPos);
			//Debug.Log (pointer[i]);
			//Instantiate(pointer[i], vertexWorldPos, Quaternion.identity);
			pointer[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
			pointer[i].transform.position = vertexWorldPos;
			pointer[i].transform.localScale += new Vector3(100F, 100F, 0);
			pointer[i].tag = "Pointer"+i;
			pointer[i].transform.parent = transform;

		}
		
		/*
		//Vertices
		Vector3[] vertex = new Vector3[4]
		{
			new Vector3 (0, 0, 0),new Vector3 (width, 0, 0),new Vector3 (0, height, 0),new Vector3 (width, height, 0)
		};

		//Triangles
		int[] tri = new int[6];
		tri [0] = 0;
		tri [1] = 2;
		tri [2] = 1;

		tri [3] = 2;
		tri [4] = 3;
		tri [5] = 1;

		//Normals
		Vector3[] normals = new Vector3[4];
		normals [0] = Vector3.forward;
		normals [1] = Vector3.forward;
		normals [2] = Vector3.forward;
		normals [3] = Vector3.forward;

		//Uvs
		Vector2[] uv = new Vector2[4];
		uv [0] = new Vector2 (0, 0);
		uv [1] = new Vector2 (1, 0);
		uv [2] = new Vector2 (0, 1);
		uv [3] = new Vector2 (1, 1);

		//Assign to mesh
		mesh.vertices = vertex;
		mesh.triangles = tri;
		mesh.normals = normals;
		mesh.uv = uv;*/


	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		if( Input.GetButton ("Fire1")) {
			Vector3 mousePos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

			Ray screenRay = Camera.main.ScreenPointToRay(mousePos);
			RaycastHit rayhit;
			if(Physics.Raycast(screenRay, out rayhit)){
				MeshFilter mf = GetComponent<MeshFilter> ();

				if(select<0){
					for(int i=0;i<mf.mesh.vertexCount;i++){
						if(rayhit.collider.tag == "Pointer"+i){ 
							//Debug.Log ("Pointer Sel: "+i);
							select=i;
							Vector3 pos = screenRay.GetPoint(rayhit.distance);

							//Vector3 localPos = transform.InverseTransformDirection(pos - transform.position);
							Vector3 localPos = new Vector3(1f*pos.x/transform.localScale.x,1f*pos.y/transform.localScale.y,0);

							//Debug.Log (localPos);

							//Debug.Log (worldPos);
							//Debug.Log (Input.mousePosition.x);
							//mesh.vertices[0].Set(worldPos.x,worldPos.y,0.0f);
							
							Vector3[] vertex = new Vector3[4];
							vertex = mf.mesh.vertices;							

							//vertex[i] = new Vector3(vertex[i].x/transform.localScale.x,vertex[i].y/transform.localScale.y,0);
							vertex[i] = localPos;
							mf.mesh.vertices = vertex;
							pointer[i].transform.position = new Vector3(pos.x,pos.y,-0.001f);
						}
					}
				}else{

					if(rayhit.collider.tag == "WallPlane"){ 
						//Debug.Log ("Pointer Move: "+select);
						Vector3 pos = screenRay.GetPoint(rayhit.distance);						
						
						//Vector3 localPos = transform.InverseTransformDirection(pos - transform.position);
						Vector3 localPos = new Vector3(1f*pos.x/transform.localScale.x,1f*pos.y/transform.localScale.y,0);
						
						//Debug.Log (worldPos);
						//Debug.Log (Input.mousePosition.x);
						//mesh.vertices[0].Set(worldPos.x,worldPos.y,0.0f);
						
						Vector3[] vertex = new Vector3[4];
						vertex = mf.mesh.vertices;
						
						//vertex[select] = new Vector3(vertex[select].x/transform.localScale.x,vertex[select].y/transform.localScale.y,0);
						vertex[select] = localPos;
						//Debug.Log (localPos);
						
						mf.mesh.vertices = vertex;
						pointer[select].transform.position = new Vector3(pos.x,pos.y,-0.01f);
					}

				}
			}
		}else{
				if(select>-1){
					select=-1;
					//Debug.Log ("Pointer Reset");
				}
		}


			#else

			#endif

		
	}
}
