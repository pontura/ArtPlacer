using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmSizes : MonoBehaviour {

    public RawImage rawImage;
    public SizeSignal sizeSignal;
    public GameObject container;


    void Start()
    {
        rawImage.texture = Data.Instance.lastPhotoTexture;
#if UNITY_IOS
       rawImage.transform.localScale = new Vector3(1, -1, 1);
#endif

        if (Data.Instance.artArea.areas.Count > 0)
        {
            // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.artArea.areas.Count; i++)
            {
                float _x = Data.Instance.artArea.areas[i].pointers[1].x + Data.Instance.artArea.areas[i].position.x;
                float _y = Data.Instance.artArea.areas[i].pointers[1].y + Data.Instance.artArea.areas[i].position.y;

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
        Data.Instance.LoadLevel("ArtPlaced");
    }

}
