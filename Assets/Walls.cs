using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Walls : MonoBehaviour {

    public GameObject ResetedContainer;
    public GameObject ReadyContainer;
	public GameObject sector;

    public RawImage rawImage;

	void Start () {
        rawImage.texture = Data.Instance.lastPhotoTexture;
        Reseted();
        Events.OnNumWallsChanged += OnNumWallsChanged;
		if (Data.Instance.artArea.areas.Count > 0) {
			Started();
			GetComponent<PhotoAddWall>().DeactiveAdd();
			for(int i=0;i<Data.Instance.artArea.areas.Count;i++){
				GameObject obj = Instantiate (sector,Data.Instance.artArea.getPosition(i), Quaternion.identity) as GameObject;
				obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter> ().mesh.vertices = Data.Instance.artArea.getPointers(i);
				obj.GetComponent<WallPlane>().SetId(i);
			}
		}
	}
    void OnDestroy()
    {
        Events.OnNumWallsChanged -= OnNumWallsChanged;
    }
    void OnNumWallsChanged(int qty)
    {
        if (qty < 1)
            Reseted();
        else
            Started();
    }
    public void GotoLoadRoom()
    {
        Data.Instance.LoadLevel("LoadRoom");
    }
    public void ArtBrowser()
    {
		Events.SaveAreas ();
        Data.Instance.LoadLevel("ArtBrowser");
    }
    public void Reseted()
    {
        ResetedContainer.gameObject.SetActive(true);
        ReadyContainer.gameObject.SetActive(false);
		GetComponent<PhotoAddWall>().ActiveAdd();
    }
    public void Started()
    {
        ResetedContainer.gameObject.SetActive(false);
        ReadyContainer.gameObject.SetActive(true);
    }
}
