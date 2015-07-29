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
				//Debug.Log (Data.Instance.areaData.getPosition(i));
                GameObject obj = Instantiate(CreatedPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
                obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
				obj.GetComponent<WallPlane>().SetId(i);
                PlaceArt(i);
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
		GameObject area = GameObject.Find ("CreatedPlane_" + n);
		Debug.Log ("ACA1");
		Debug.Log ("Area Artworks " + Data.Instance.areaData.areas [n].artworks);
		Debug.Log ("ArtW Count"+Data.Instance.areaData.areas [n].artworks.Count);
		if (Data.Instance.areaData.areas[n].artworks.Count > 0) {
			Debug.Log ("ACA2");
			for (int i=0; i<Data.Instance.areaData.areas[n].artworks.Count; i++) {
				GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
				artWork.transform.localScale = Data.Instance.areaData.areas[n].GetArtWorkScale(i);
				artWork.transform.position = area.transform.position;
				artWork.transform.position = Data.Instance.areaData.areas[n].artworks[i].position;
				artWork.transform.SetParent(area.transform);
				artWork.name = "ArtWork_" + n +"_"+ area.GetComponent<WallPlane>().artWorkNumber;
				area.GetComponent<WallPlane> ().artWorkNumber++;
				artWork.GetComponent<Renderer>().material.mainTexture = Data.Instance.areaData.areas[n].artworks[i].texture;
				area.GetComponent<Homography> ().SetHomography (artWork.name);

				artWork.GetComponent<DragArtWork> ().SetAreaId(n);
				artWork.GetComponent<DragArtWork> ().SetArtWorkId(i);
			}

		}

		if (Data.Instance.lastArtTexture != null) {

			GameObject artWork = Instantiate (area.GetComponent<WallPlane> ().artWork, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;

			int w = (int)Data.Instance.artData.selectedArtWork.size.x;
			int h = (int)Data.Instance.artData.selectedArtWork.size.y;
			Data.Instance.areaData.areas[n].AddArtWork(w,h,Data.Instance.lastArtTexture);			

			artWork.transform.position = area.transform.position;

			//artWork.transform.position = new Vector3 (wall.transform.position.x, wall.transform.position.y, 9);
			artWork.transform.localScale = Data.Instance.areaData.areas[n].GetArtWorkScale(Data.Instance.areaData.areas[n].artworks.Count-1);

			/*Vector3[] am = new Vector3[4];
			for (int i=0; i<am.Length; i++) {
				am[i] = Data.Instance.areaData.areas[n].pointers[i] * 0.5f;
				
			}
			float top = (am [3].y + am [1].y) * 0.5f;
			float left = (am [3].x + am [0].x) * 0.5f;
			float bottom = (am [0].y + am [2].y) * 0.5f;
			float right = (am [1].x + am [2].x) * 0.5f;
			float centerX = (left + right) * 0.5f;
			float centerY = (top + bottom) * 0.5f;
			Vector3 pos = artWork.transform.position;
			artWork.transform.position = new Vector3 (pos.x + centerX, pos.y + centerY, pos.z);*/

			artWork.transform.SetParent (area.transform);

			artWork.name = "ArtWork_" + n + "_" + area.GetComponent<WallPlane> ().artWorkNumber;
			area.GetComponent<WallPlane> ().artWorkNumber++;

			//artWork.GetComponent<Renderer>().material.shader = Shader.Find("Custom/HomographyOren-NayarTransparentDiffuse");
			artWork.GetComponent<Renderer> ().material.mainTexture = Data.Instance.lastArtTexture;
			area.GetComponent<Homography> ().SetHomography (artWork.name);
			Data.Instance.lastArtTexture = null;
			artWork.GetComponent<DragArtWork> ().SetAreaId(n);
			artWork.GetComponent<DragArtWork> ().SetArtWorkId(Data.Instance.areaData.areas[n].artworks.Count-1);

			artCount++;
		}
	}
}
