using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AreaData : MonoBehaviour {

	public List<Area> areas;
    public string url="";
    public int id;

	[Serializable]
	public class Area{
		public Vector3[] pointers;
		public Vector3 position;
		public float width;
        public float height;
		public List<ArtWork> artworks;
		public int artworkCount;

		public Area(){
			pointers = new Vector3[4];
			artworks = new List<ArtWork>();
		}

		public Vector3 GetArtWorkScale(int i){
			float scaleY = 1f*artworks[i].height/height;
			float scaleX = scaleY * artworks [i].width / artworks [i].height;
			return new Vector3 (scaleX, scaleY, 1f);
		}

		public void AddArtWork(int w, int h, Texture2D tex, ArtData.GalleryData.ArtData artdata){
			ArtWork artwork = new ArtWork (	w, h, tex);
			artwork.SetGalleryData (artdata);
			artworks.Add (artwork);
		}
	}

	[Serializable]
	public class ArtWork{
		public string url;
		public Vector3 position;
		public int width;
		public int height;
		public Texture2D texture;
		public int id;
		public int galleryID;
		public int galleryArtID;

		public ArtWork(int w, int h, Texture2D tex){
			width = w;
			height = h;
			texture = tex;
		}

		public void SetGalleryData(ArtData.GalleryData.ArtData artdata){
			galleryID = artdata.galleryId;
			galleryArtID = artdata.artId;
		}
	}
	public void Clear(){
		areas.Clear();
		url = "";
	}
    public void AddAreas(int id, Vector3[] pointers, Vector3 position, float width, float height)
    {
		if (id < 0) {
			Area area = new Area();
			area.pointers = pointers;
			area.position = position;
            area.height = height;
            area.width = width;
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
			foreach (ArtWork artwork in area.artworks) {
				result += Math.Round(artwork.position.x, 2) + "/" + Math.Round(artwork.position.y, 2) + "/" + artwork.galleryID + "/" + artwork.galleryArtID;
				result += "*";
			}
			result += "+";
        }
			
		int _id;
		string DataName = GetComponent<RoomsData>().GetRoomName(out _id,url);
		Data.Instance.roomsData.actualRoomId = _id;

        PlayerPrefs.SetString(DataName, result);

        print("graba: " + DataName + " : " + result);

        GetComponent<RoomsData>().ReadRoomsData();
    }
    
    public int CountWalls()
    {
        int num = 0;
        foreach (Area area in areas)
            num++;
        return num;
    }
    public int CountArtPlaced()
    {
        int num = 0;
        foreach (Area area in areas)
            foreach (ArtWork art in area.artworks)
                num++;

        return num;
    }

	public void SetAsNew(){
		url="";
	}

}
