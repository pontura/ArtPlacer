using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmSizes : MonoBehaviour {

    public RawImage rawImage;
    public SizeSignal sizeSignal;
    public GameObject container;


    void Start()
    {
        Data.Instance.SetTexture(rawImage, Data.Instance.lastPhotoTexture);

        if (Data.Instance.areaData.areas.Count > 0)
        {
            // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                float _x = Data.Instance.areaData.areas[i].pointers[1].x + Data.Instance.areaData.areas[i].position.x;
                float _y = Data.Instance.areaData.areas[i].pointers[1].y + Data.Instance.areaData.areas[i].position.y;

                SizeSignal newSizeSignal = Instantiate(sizeSignal);
                newSizeSignal.transform.SetParent(container.transform);
                newSizeSignal.transform.localPosition = new Vector3(_x * 150, _y * 150, 0);
                newSizeSignal.transform.localScale = Vector3.one;
            }
        }

    }
    public void GotoLoadRoom()
    {
       // Data.Instance.LoadLevel("LoadRoom");
    }
    public void Ready()
    {
        Events.SaveAreas();
        Data.Instance.areaData.Save();
        Data.Instance.LoadLevel("ArtPlaced");
    }

}
