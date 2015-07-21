using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmSizes : MonoBehaviour {

    public RawImage rawImage;
    public SizeSignal sizeSignal;
    public GameObject container;
    public List<SizeSignal> sizeSignals;


    void Start()
    {
        Data.Instance.SetTexture(rawImage, Data.Instance.lastPhotoTexture);

        if (Data.Instance.areaData.areas.Count > 0)
        {
            int id = 0;
            for (int i = 0; i < Data.Instance.areaData.areas.Count; i++)
            {
                float _x = Data.Instance.areaData.areas[i].pointers[1].x + Data.Instance.areaData.areas[i].position.x;
                float _y = Data.Instance.areaData.areas[i].pointers[1].y + Data.Instance.areaData.areas[i].position.y;

               

                SizeSignal newSizeSignal = Instantiate(sizeSignal);
                newSizeSignal.id = id;
                newSizeSignal.transform.SetParent(container.transform);
                newSizeSignal.transform.localPosition = new Vector3(_x * 150, _y * 150, 0);
                newSizeSignal.transform.localScale = Vector3.one;

                newSizeSignal.Init(Data.Instance.areaData.areas[i].height);

                sizeSignals.Add(newSizeSignal);

                id++;
            }
        }

    }
    public void GotoLoadRoom()
    {
       // Data.Instance.LoadLevel("LoadRoom");
    }
    public void Ready()
    {
        foreach (SizeSignal sizeSignal in sizeSignals)
        {
            Data.Instance.areaData.areas[sizeSignal.id].height = int.Parse( sizeSignal.field.text );
        }
        Events.SaveAreas();
        Data.Instance.areaData.Save();
        Data.Instance.LoadLevel("ArtPlaced");
    }

}
