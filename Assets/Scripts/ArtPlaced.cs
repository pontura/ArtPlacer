using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ArtPlaced : MonoBehaviour {

	public GameObject CreatedPlane;
	
	public RawImage rawImage;
	int artCount = 0;
	
	void Start () {
        Data.Instance.SetTexture(rawImage, Data.Instance.lastPhotoTexture);

        if (Data.Instance.areaData.areas.Count > 0)
		{
			// GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
			{
                GameObject obj = Instantiate(CreatedPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
                obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
				obj.GetComponent<WallPlane>().SetId(i);
                if (Data.Instance.areaData.areas.Count > 0 && Data.Instance.lastArtTexture != null)
                {
					PlaceArt(artCount);
				}
			}
		}





	}

	public void ArtBrowser()
	{
		Data.Instance.LoadLevel("ArtBrowser");
	}

	public void EditWalls()
	{
		Data.Instance.LoadLevel("Walls");
	}

	void PlaceArt(int n){
		GameObject wall = GameObject.Find ("CreatedPlane_" + n);
		print (wall);
		GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Vector3[] am = new Vector3[4];
		for (int i=0; i<am.Length; i++) {
            am[i] = Data.Instance.areaData.areas[n].pointers[i] * 0.5f;

		}
		float top = (am [3].y + am [1].y) * 0.5f;
		float left = (am [3].x + am [0].x) * 0.5f;
		float bottom = (am [0].y + am [2].y) * 0.5f;
		float right = (am [1].x + am [2].x) * 0.5f;
		float centerX = (left + right) * 0.5f;
		float centerY = (top + bottom) * 0.5f;
		q.GetComponent<MeshFilter>().mesh.vertices = am;
		q.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
		q.GetComponent<Renderer>().material.mainTexture = Data.Instance.lastArtTexture;
		q.transform.position = new Vector3 (wall.transform.position.x, wall.transform.position.y, 9);
		q.transform.SetParent(wall.transform);
		Vector3 pos = q.transform.position;
		q.transform.position = new Vector3 (pos.x + centerX, pos.y + centerY, pos.z);

		artCount++;
	}
}
