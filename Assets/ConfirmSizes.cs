using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfirmSizes : MonoBehaviour {

    public RawImage rawImage;
    public SizeSignal sizeSignal;
    public GameObject container;
    public List<SizeSignal> sizeSignals_H;
	public List<SizeSignal> sizeSignals_W;


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

                SizeSignal newSizeSignal_H = Instantiate(sizeSignal);
                newSizeSignal_H.id = id;
                newSizeSignal_H.transform.SetParent(container.transform);
				newSizeSignal_H.transform.localPosition = new Vector3(_x * 150 + newSizeSignal_H.transform.localScale.x * 0.30f, _y * 150, 0);
                newSizeSignal_H.transform.localScale = Vector3.one;
                newSizeSignal_H.Init(Data.Instance.areaData.areas[i].height);
                sizeSignals_H.Add(newSizeSignal_H);

				_x = Data.Instance.areaData.areas[i].pointers[3].x + Data.Instance.areaData.areas[i].position.x;
				_y = Data.Instance.areaData.areas[i].pointers[3].y + Data.Instance.areaData.areas[i].position.y;              
				
				SizeSignal newSizeSignal_W = Instantiate(sizeSignal);
				newSizeSignal_W.GetComponentInChildren<Text>().text = "WIDTH";
				newSizeSignal_W.id = id;
				newSizeSignal_W.transform.SetParent(container.transform);
				newSizeSignal_W.transform.localPosition = new Vector3(_x * 150 + newSizeSignal_W.transform.localScale.x * 0.35f, _y * 150 + newSizeSignal_W.transform.localScale.x * 0.35f, 0);
				newSizeSignal_W.transform.localScale = Vector3.one;
				newSizeSignal_W.Init(Data.Instance.areaData.areas[i].width);
				sizeSignals_W.Add(newSizeSignal_W);
				
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
        foreach (SizeSignal sizeSignal in sizeSignals_H)
        {
            Data.Instance.areaData.areas[sizeSignal.id].height = int.Parse( sizeSignal.field.text );
        }
		foreach (SizeSignal sizeSignal in sizeSignals_W)
		{
			Data.Instance.areaData.areas[sizeSignal.id].width = int.Parse( sizeSignal.field.text );
		}
		
		Events.SaveAreas();
        Data.Instance.areaData.Save();
        Data.Instance.LoadLevel("ArtPlaced");
    }

}
