using UnityEngine;
using System.Collections;

public class WallCreator : MonoBehaviour {

    public Game gameContainer;
    public GameObject createdPlane;

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
            }
        }
    }
}
