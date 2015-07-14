using UnityEngine;
using System.Collections;

public class CreatePlaneMesh : MonoBehaviour {

	public float width = 1000f;
	public float height = 1000f;
	public Vector3 angleSuma;
	//public Mesh mesh;
	public GameObject[] pointer;

	public GameObject planoContenedor;

	int select = -1;
	float angle1;
	float angle2;


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

			//Instantiate(pointer[i], vertexWorldPos, Quaternion.identity);
			pointer[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
			pointer[i].transform.position = vertexWorldPos;
			pointer[i].transform.localScale += new Vector3(100F, 100F, 0);
			pointer[i].tag = "Pointer"+i;
			pointer[i].transform.parent = transform;
			Debug.Log ("Punto"+i+": "+pointer[i].transform.position);

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
							/*Debug.Log ("Pointer Sel: "+i);
							Debug.Log ("Angle1: "+angle1);
							Debug.Log ("Angle2: "+angle2);*/

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

				/*MeshFilter mf2 = GetComponent<MeshFilter> ();
				Vector3 p1 = mf2.mesh.vertices[3]+new Vector3(1.5f,-0.5f);
				Vector3 p2 = mf2.mesh.vertices[1]+new Vector3(0.5f,-0.5f);
				Vector3 p3 = mf2.mesh.vertices[0]+new Vector3(0.5f,0.5f);
				Vector3 p4 = mf2.mesh.vertices[2]+new Vector3(0.5f,0.5f);
				Debug.Log ("P1: "+p1);
				Debug.Log ("P2: "+p2);
				/*Debug.Log ("P3: "+p3);
				Debug.Log ("P3: "+p4);*/

				/*angle1 = Vector3.Angle(p1,p2);
				angle2 = Vector3.Angle(p3,p4);*/

				//planoContenedor.transform.rotation = Quaternion.AngleAxis(angle1, Vector3.up);
				//planoContenedor.transform.Rotate (Vector3.up, angle1 - 90, Space.Self);

					Transform2d();
				}
		}


			#else

			#endif






	}

	Matrix4x4 GetAdjugate(Matrix4x4 m) { // Compute the adjugate of m
		Matrix4x4 result = Matrix4x4.identity;

		result [0, 0] = m [1,1] * m [2,2] - m[1,2] * m[2,1];
		result [0, 1] = m [0,2] * m [2,1] - m[0,1] * m[2,2];
		result [0, 2] = m [0,1] * m [1,2] - m[0,2] * m[1,1];

		result [1, 0] = m [1,2] * m [2,0] - m[1,0] * m[2,2];
		result [1, 1] = m [0,0] * m [2,2] - m[0,2] * m[2,0];
		result [1, 2] = m [0,2] * m [1,0] - m[0,0] * m[1,2];

		result [2, 0] = m [1,0] * m [2,1] - m[1,1] * m[2,0];
		result [2, 1] = m [0,1] * m [2,0] - m[0,0] * m[2,1];
		result [2, 2] = m [0,0] * m [1,1] - m[0,1] * m[1,0];

		return result;

		/*return [
		        m[4]*m[8]-m[5]*m[7], m[2]*m[7]-m[1]*m[8], m[1]*m[5]-m[2]*m[4],
		        m[5]*m[6]-m[3]*m[8], m[0]*m[8]-m[2]*m[6], m[2]*m[3]-m[0]*m[5],
		        m[3]*m[7]-m[4]*m[6], m[1]*m[6]-m[0]*m[7], m[0]*m[4]-m[1]*m[3]
		        ];*/
	}

	Matrix4x4 BasisToPoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
		Matrix4x4 m = Matrix4x4.identity;
		Vector4 v41 = p1;
		v41.z = 1;

		Vector4 v42 = p2;
		v42.z = 1;

		Vector4 v43 = p3;
		v43.z = 1;

		/*Vector4 v44 = p4;
		v44.z = 1;*/

		m.SetColumn (0, v41);
		m.SetColumn (1, v42);	
		m.SetColumn (2, v43);
		//m.SetColumn (3, v44);

		m = GetAdjugate (m);
		p4.z = 1;
		Vector3 vt = m.MultiplyPoint3x4(p4);
		Matrix4x4 v = Matrix4x4.identity;

		v [0, 0] = vt.x;
		v [1, 1] = vt.y;
		v [2, 2] = vt.z;

		return m * v;
	}

	Matrix4x4 General2DProjection(Vector3 s1, Vector3 d1, Vector3 s2, Vector3 d2, Vector3 s3, Vector3 d3, Vector3 s4, Vector3 d4) {	
		Matrix4x4 s = BasisToPoints(s1, s2, s3, s4);
		Matrix4x4 d = BasisToPoints(d1, d2, d3, d4);
		return d * GetAdjugate (s);
	}

	void Transform2d() {
		Debug.Log ("S1: " + (new Vector2 (-500, -500) + new Vector2 (500,500)) + " D1: " + (pointer [0].transform.position + new Vector3 (500,500,0)));
		Debug.Log ("S2: " + (new Vector2(500, -500)+new Vector2(500,500)) + " D2: " + (pointer[2].transform.position+new Vector3(500,500,0)));
		Debug.Log ("S3: " + (new Vector2(-500, 500)+new Vector2(500,500)) + " D3: " + (pointer[3].transform.position+new Vector3(500,500,0)));
		Debug.Log ("S4: " + (new Vector2(500, 500)+new Vector2(500,500)) + " D4: " + (pointer[1].transform.position+new Vector3(500,500,0)));

		Matrix4x4 t = General2DProjection(new Vector2(-500,-500)+new Vector2(500,500), pointer[0].transform.position+new Vector3(500,500,0), new Vector2(500, -500)+new Vector2(500,500), pointer[2].transform.position+new Vector3(500,500,0), new Vector2(-500, 500)+new Vector2(500,500), pointer[3].transform.position+new Vector3(500,500,0), new Vector2(500, 500)+new Vector2(500,500), pointer[1].transform.position+new Vector3(500,500,0));
		for(int i=0;i<3;i++){
			for(int j=0;j<3;j++){
				t[i,j] = t[i,j]/t[2,2];

			}
		}

		Vector4 v1 = new Vector4 (t [0,0], t [0,1], 0, t [0,2]);
		Vector4 v2 = new Vector4 (t [1,0], t [1,1], 0, t [1,2]);
		Vector4 v3 = new Vector4 (0, 0, 1, 0);
		Vector4 v4 = new Vector4 (t [2,0], t [2,1], 0, t [2,2]);

		t.SetColumn (0, v1);
		t.SetColumn (1, v2);	
		t.SetColumn (2, v3);
		t.SetColumn (3, v4);

		Vector3 p = t.GetColumn(3);
		p.z -= 1000;

		//planoContenedor.transform.position = p;

		planoContenedor.transform.rotation = QuaternionFromMatrix(t);
		planoContenedor.transform.localEulerAngles+=angleSuma;
		//planoContenedor.transform.rotation.SetLookRotation(Vector3.right);

		//planoContenedor.transform.Rotate (0, 90, 0, Space.Self);
		//planoContenedor.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);


		//planoContenedor.transform.rotation = Quaternion.LookRotation(t.GetColumn(2), t.GetColumn(1));

		//planoContenedor.transform.localScale = new Vector3(width, height, 1);

		// Extract new local scale
		/*Vector3 scale = new Vector3(
			t.GetColumn(0).magnitude*1000,
			t.GetColumn(1).magnitude*1000,
			t.GetColumn(2).magnitude
			);

		planoContenedor.transform.localScale = scale;*/
	}

	public static Quaternion QuaternionFromMatrix2(Matrix4x4 m) {
		Quaternion q = new Quaternion();
		float tr = m [0, 0] + m [1, 1] + m [2, 2];

		if (tr > 0) { 
			//float S = Mathf.Sqrt(tr+1.0f) * 2; // S=4*qw 
			float S = 0.5f / Mathf.Sqrt(tr+1.0f);
			q.w = 0.25f * S;
			q.x = (m[2,1] - m[1,2]) * S;
			q.y = (m[0,2] - m[2,0]) * S; 
			q.z = (m[1,0] - m[0,1]) * S; 
		} else if ((m[0,0] > m[1,1])&(m[0,0] > m[2,2])) { 
			float S = 2.0f * Mathf.Sqrt(1.0f + m[0,0] - m[1,1] - m[2,2]); // S=4*q.x 
			q.w = (m[2,1] - m[1,2]) / S;
			q.x = 0.25f * S;
			q.y = (m[0,1] + m[1,0]) / S; 
			q.z = (m[0,2] + m[2,0]) / S; 
		} else if (m[1,1] > m[2,2]) { 
			float S = 2.0f * Mathf.Sqrt(1.0f + m[1,1] - m[0,0] - m[2,2]); // S=4*q.y`
			q.w = (m[0,2] - m[2,0]) / S;
			q.x = (m[0,1] + m[1,0]) / S; 
			q.y = 0.25f * S;
			q.z = (m[1,2] + m[2,1]) / S; 
		} else { 
			float S = 2.0f * Mathf.Sqrt(1.0f + m[2,2] - m[0,0] - m[1,1]); // S=4*q.z
			q.w = (m[1,0] - m[0,1]) / S;
			q.x = (m[0,2] + m[2,0]) / S;
			q.y = (m[1,2] + m[2,1]) / S;
			q.z = 0.25f * S;
		}

		return q;
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
		q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
		q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
		q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
		q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
		q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
		q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );

		Debug.Log ("A: " + Mathf.Sign (q.x * (m [2, 1] - m [1, 2])));
		Debug.Log ("B: " + Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) ));
		Debug.Log ("C: " + Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) ));


		return q;
	}
}
