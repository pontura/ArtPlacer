using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ArtPlaced : MonoBehaviour {

	public GameObject CreatedPlane;
	
	public RawImage rawImage;
	
	void Start () {
		rawImage.texture = Data.Instance.lastPhotoTexture;
		if (Data.Instance.artArea.areas.Count > 0) {
			/*GetComponent<PhotoAddWall>().DeactiveAdd();
			for(int i=0;i<Data.Instance.artArea.areas.Count;i++){
				GameObject obj = Instantiate (CreatedPlane,Data.Instance.artArea.getPosition(i), Quaternion.identity) as GameObject;
				obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter> ().mesh.vertices = Data.Instance.artArea.getPointers(i);
				obj.GetComponent<WallPlane>().SetId(i);
			}*/
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

}
