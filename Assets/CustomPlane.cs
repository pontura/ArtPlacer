using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomPlane : MonoBehaviour {
	
	public int recursionLevel=0;
	int resX = 8; // 2 minimum
	int resY = 8;
	
	public Vector3[] pointers;
	Vector3[] vertices;
	List<Vector3> vertex;
	
	public void SetPointers(Vector3[] p){
		pointers = p;
	}
	
	// Use this for initialization
	public void CustomMesh () {
		// You can change that line to provide another MeshFilter
		MeshFilter filter = gameObject.GetComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		resX=1+(int)Mathf.Pow (2,recursionLevel+1);
		resY=resX;
		#region Vertices
		vertices = new Vector3[ resX * resY ];
		vertices [0] = pointers [0];
		vertices [(int)Mathf.Pow (2,recursionLevel+1)] = pointers [2];
		vertices [(int)((1+Mathf.Pow (2,recursionLevel+1))*Mathf.Pow (2,recursionLevel+1))] = pointers [3];
		vertices [vertices.Length-1] = pointers [1];

		MakeGrid (pointers [3], pointers [0], pointers [2], pointers [1], recursionLevel, 0, 0);
		#endregion
				
		//for (int i=0; i<vertices.Length; i++)Debug.Log ("Post"+i+": "+vertices [i]);
				
		/*float length = 1f;
		float width = 1f;
		float invW = 1f/(resX-1);
		float invH = 1f/(resY-1);
		#region Vertices		
		Vector3[] vertices = new Vector3[ resX * resY ];
		for(int y = 0; y < resY; y++)
		{
			// [ -length / 2, length / 2 ]
			float yPos_ = ((float)y / (resY - 1) - .5f) * length;
			for(int x = 0; x < resX; x++)
			{
				float posy0 = pointers[0].y + (pointers[2].y - pointers[0].y)*invW*x;
				float posy1 = pointers[3].y + (pointers[1].y - pointers[3].y)*invW*x;
				float yPos = posy0 + (posy1-posy0)*invH*y;
				// [ -width / 2, width / 2 ]
				float posx0 = pointers[0].x + (pointers[3].x - pointers[0].x)*invH*y;
				float posx1 = pointers[2].x + (pointers[1].x - pointers[2].x)*invH*y;
				float xPos_ = ((float)x / (resX - 1) - .5f) * width;
				float xPos = posx0 + (posx1-posx0)*invW*x;
				vertices[ x + y * resX ] = new Vector3( xPos, yPos, 0f );
				Debug.Log ("Vertex"+(x + y * resX)+": "+vertices[ x + y * resX ]+"_"+xPos_+", "+yPos_);
			}
		}
		#endregion*/
		
		#region Normales
		Vector3[] normales = new Vector3[ vertices.Length ];
		for( int n = 0; n < normales.Length; n++ )
			normales[n] = Vector3.up;
		#endregion
		
		#region UVs		
		Vector2[] uvs = new Vector2[ vertices.Length ];
		for(int v = 0; v < resY; v++)
		{
			for(int u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resY - 1) );
			}
		}
		#endregion
		
		#region Triangles
		int nbFaces = (resX - 1) * (resY - 1);
		int[] triangles = new int[ nbFaces * 6 ];
		int t = 0;
		for(int face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resY - 1) * resX);
			
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
			
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		gameObject.GetComponent< MeshCollider >().sharedMesh = mesh;
		
	}
	
	bool LineIntersectionPoint(out Vector3 result, Vector3 ps1, Vector3 pe1, Vector3 ps2, Vector3 pe2){
		
		result = Vector3.zero;
		
		Vector3 lineVec1 = (pe1 - ps1).normalized;
		Vector3 lineVec2 = (pe2 - ps2).normalized;
		
		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
		
		float d = a*e - b*b;
		
		//lines are not parallel
		if(d != 0.0f){
			
			Vector3 r = ps1 - ps2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
			
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
			
			result = ps1 + lineVec1 * s;
			
			return true;
		}
		
		else{
			return false;
		}
	}
	
	void MakeGrid(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, int maxRecur, int recurStep, int indOffS){//point0 es top Left y sigue counter clockwise
		//print ("pre VCount: " + vertex.Count);
		Vector3 res, vp1, vp2, mLeft, mRight, mTop, mBottom;
		res = new Vector3 ();
		LineIntersectionPoint (out res, point0, point2, point1, point3);
		Vector3 center = new Vector3(res.x,res.y,res.z);
		if (LineIntersectionPoint (out res, point0, point3, point1, point2)) {
			vp1 = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point0, point1, vp1, center);
			mLeft = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point3, point2, vp1, center);
			mRight = new Vector3(res.x,res.y,res.z);
		} else if (LineIntersectionPoint (out res, point3, point0, point2, point1)) {
			vp1 = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point0, point1, vp1, center);
			mLeft = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point3, point2, vp1, center);
			mRight = new Vector3(res.x,res.y,res.z);
		}else{
			//Debug.Log ("Paralelas Verticales");
			mLeft = point1+(point0-point1)*0.5f;
			mRight = point2+(point3-point2)*0.5f;
		}
		
		if(LineIntersectionPoint (out res, point0, point1, point3, point2)){
			vp2 = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point0, point3, vp2, center);
			mTop = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point1, point2, vp2, center);
			mBottom = new Vector3(res.x,res.y,res.z);
		}if(LineIntersectionPoint (out res, point1, point0, point2, point3)){
			vp2 = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point0, point3, vp2, center);
			mTop = new Vector3(res.x,res.y,res.z);
			LineIntersectionPoint (out res, point1, point2, vp2, center);
			mBottom = new Vector3(res.x,res.y,res.z);
		}else{
			//Debug.Log ("Paralelas Horizotales");
			mTop = point0+(point3-point0)*0.5f;
			mBottom = point1+(point2-point1)*0.5f;
		}

		int step = (int)Mathf.Pow (2,maxRecur-recurStep);//
		
		int mB_ind = indOffS + step;
		int cero_ind = mB_ind - step;
		vertices[mB_ind] = mBottom;
		//print("Nivel "+recurStep+" Sub0 mBottom index: "+mB_ind);
		
		int mL_ind = indOffS + step*(int)(1+Mathf.Pow (2,maxRecur+1));
		vertices[mL_ind] = mLeft;
		//print("Nivel "+recurStep+" Sub0 mLeft index: "+mL_ind);
		
		int center_ind = indOffS + step*(int)(1+Mathf.Pow (2,maxRecur+1))+step;
		vertices[center_ind] = center;
		//print("Nivel "+recurStep+" Sub0 center index: "+center_ind);
		
		
		int mR_ind = indOffS + step*(int)(1+Mathf.Pow (2,maxRecur+1))+2*step;
		vertices[mR_ind] = mRight;
		//print("Nivel "+recurStep+" Sub0 mRight index: "+mR_ind);
		
		int mT_ind = indOffS + 2*step*(int)(1+Mathf.Pow (2,maxRecur+1))+step;
		vertices[mT_ind] = mTop;
		//print("Nivel "+recurStep+" Sub0 mTop index: "+mT_ind);
		
		if(recurStep<maxRecur)MakeGrid (mLeft, point1, mBottom, center, maxRecur, recurStep+1, cero_ind);
		if(recurStep<maxRecur)MakeGrid (center, mBottom, point2, mRight, maxRecur, recurStep+1, mB_ind);
		if(recurStep<maxRecur)MakeGrid (point0, mLeft, center, mTop, maxRecur, recurStep+1, mL_ind);
		if(recurStep<maxRecur)MakeGrid (mTop, center, mRight, point3, maxRecur, recurStep+1, center_ind);		

	}
}
