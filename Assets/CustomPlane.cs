using UnityEngine;
using System.Collections;

public class CustomPlane : MonoBehaviour {

	public int resX = 2; // 2 minimum
	public int resY = 2;

	public Vector3[] pointers;

	public void SetPointers(Vector3[] p){
		pointers = p;
	}

	// Use this for initialization
	public void CustomMesh () {
		// You can change that line to provide another MeshFilter
		MeshFilter filter = gameObject.GetComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();


		
		float length = 1f;
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
				//Debug.Log ("Vertex"+(x + y * resX)+": "+vertices[ x + y * resX ]+"_"+xPos_+", "+yPos_);
			}
		}

		#endregion
		
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
}
