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

        if (Data.Instance.artArea.areas.Count > 0)
        {
            // GetComponent<PhotoAddWall>().DeactiveAdd();
            for (int i = 0; i < Data.Instance.artArea.areas.Count; i++)
            {
                SizeSignal newSizeSignal = Instantiate(sizeSignal);
                newSizeSignal.transform.SetParent(newSizeSignal.transform);

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
        Data.Instance.LoadLevel("Walls");
    }

}
