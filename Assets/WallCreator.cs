using UnityEngine;
using System.Collections;

public class WallCreator : MonoBehaviour {

    public GameObject createdPlane;

    void Start()
    {
        if (Data.Instance.artArea.areas.Count > 0)
        {
           // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.artArea.areas.Count; i++)
            {
                GameObject obj = Instantiate(createdPlane, Data.Instance.artArea.getPosition(i), Quaternion.identity) as GameObject;
                obj.GetComponent<WallPlane>().area.GetComponent<MeshFilter>().mesh.vertices = Data.Instance.artArea.getPointers(i);
                obj.GetComponent<WallPlane>().SetId(i);
            }
        }
    }
}
