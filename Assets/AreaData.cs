using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AreaData : MonoBehaviour {

	public List<Area> areas;
    public string url;
    public int id;

	[Serializable]
	public class Area{
		public Vector3[] pointers;
		public Vector3 position;
		public int width;
        public int height;
		public List<ArtWork> artworks;

		public Area(){
			pointers = new Vector3[4];
			artworks = new List<ArtWork>();
		}

		public Vector3 GetArtWorkScale(int i){
			float scaleY = 1f*artworks[i].height/height;
			float scaleX = scaleY * artworks [i].width / artworks [i].height;
			return new Vector3 (scaleX, scaleY, 1f);
		}

		public void AddArtWork(int w, int h, Texture tex){
			ArtWork artwork = new ArtWork (w, h, tex);
			artworks.Add (artwork);
		}
	}

	[Serializable]
	public class ArtWork{
		public Vector3 position;
		public int width;
		public int height;
		public Texture texture;
		
		public ArtWork(int w, int h, Texture tex){
			width = w;
			height = h;
			texture = tex;
		}
	}

    public void AddAreas(int id, Vector3[] pointers, Vector3 position, int height)
    {
		if (id < 0) {
			Area area = new Area();
			area.pointers = pointers;
			area.position = position;
            area.height = height;
			areas.Add (area);
		} else {
			areas[id].pointers = pointers;
			areas[id].position = position;
		}		

	}

	public Vector3[] getPointers(int id){
		return areas[id].pointers;
	}

	public Vector3 getPosition(int id){
		return areas[id].position;
	}

    public void Save()
    {
        string result = url + ":";

        foreach (Area area in areas)
        {
			result += area.width + "_" + area.height + "_" + Math.Round(area.position.x, 2) + "_" + Math.Round(area.position.y,2) + "_";
            foreach (Vector3 pointers in area.pointers)
            {
                result += Math.Round(pointers.x, 2) + "_" + Math.Round(pointers.y, 2) + "_";
            }
            result += "+";
        }
        string DataName = GetComponent<RoomsData>().GetRoomName(url);

        PlayerPrefs.SetString(DataName, result);

        print("graba: " + DataName + " : " + result);

        GetComponent<RoomsData>().ReadRoomsData();
    }

}
