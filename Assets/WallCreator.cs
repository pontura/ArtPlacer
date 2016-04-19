using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallCreator : MonoBehaviour {

    public Game gameContainer;
    public GameObject createdPlane;
    public GameObject createdPlaneArtworks;

    public List<WallPlane> createdPlanes;

    public Material SelectedMaterial;
    public Material UnselectedMaterial;

	public bool moveAreas = false;

    void Start()
    {
        if (Data.Instance.areaData.areas.Count > 0)
        {
           // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                createdPlaneArtworks = Instantiate(createdPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
                createdPlaneArtworks.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
                createdPlaneArtworks.GetComponent<WallPlane>().SetId(i);

                createdPlaneArtworks.transform.SetParent(gameContainer.transform);

                createdPlaneArtworks.GetComponent<WallPlane>().areaHeight = (int)Data.Instance.areaData.areas[i].height;

                createdPlanes.Add(createdPlaneArtworks.GetComponent<WallPlane>());
            }
        }
        foreach (WallPlane wallPlanes in createdPlanes) {
			wallPlanes.EnableAreaCollider (moveAreas);
			wallPlanes.EnableMoveArea (moveAreas);
			wallPlanes.area.GetComponent<MeshRenderer> ().material = UnselectedMaterial;
		}
    }
    public void SelectAllAreas()
    {
        foreach (WallPlane wallPlanes in createdPlanes)
            wallPlanes.area.GetComponent<MeshRenderer>().material = SelectedMaterial;
    }
    public void SelectArea(int id)
    {
        
        foreach (WallPlane wallPlanes in createdPlanes)
            wallPlanes.area.GetComponent<MeshRenderer>().material = UnselectedMaterial;

        createdPlanes[id].area.GetComponent<MeshRenderer>().material = SelectedMaterial;
    }
}
