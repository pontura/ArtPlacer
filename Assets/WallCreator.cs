using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallCreator : MonoBehaviour {

    public Game gameContainer;
    public GameObject createdPlane;

    public List<WallPlane> createdPlanes;

    public Material SelectedMaterial;
    public Material UnselectedMaterial;

    void Start()
    {
        if (Data.Instance.areaData.areas.Count > 0)
        {
           // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                GameObject obj = Instantiate(createdPlane, Data.Instance.areaData.getPosition(i), Quaternion.identity) as GameObject;
                obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.areaData.getPointers(i);
                obj.GetComponent<WallPlane>().SetId(i);

                obj.transform.SetParent(gameContainer.transform);

                createdPlanes.Add(obj.GetComponent<WallPlane>());
            }
        }
        foreach (WallPlane wallPlanes in createdPlanes) {
			wallPlanes.EnableAreaCollider (true);
			wallPlanes.EnableMoveArea (true);
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
