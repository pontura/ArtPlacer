using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class ArtData : MonoBehaviour {

    public GalleryData.ArtData selectedArtWork;

    public int selectedGallery;

    public GalleryData[] galleries;
	public GalleryData myArtWorks;
    public List<Favourite> favorites;

    [Serializable]
    public class Favourite
    {
        public int galleryId;
        public int artId;
    }

    [Serializable]
    public class GalleryData
    {
        public string title;
		public int id;
        public List<ArtData> artWorksData;

        [Serializable]
        public class ArtData
        {
            public string title;
            public string url;
            public string gallery;
            public int galleryId;
            public int artId;
            public string autor;
            public string technique;
            public Vector2 size;
			public bool isLocal;

			public string GetUrl(){
				string result = "";
				if (galleryId == -2) {
					string folder = Data.Instance.GetArtPath ();		
					result = Path.Combine (folder, url + ".png");
				} else {
					result = url;
				}
				return result;
			}

			public void setSizes(){
				if (size.y == -1) {
					size.y = Mathf.Round(size.x*Data.Instance.lastArtTexture.height/Data.Instance.lastArtTexture.width);
				}
			}

			public string getSizeWUnits(){

				if (size.y == -1) {
					size.y = Mathf.Round(size.x*Data.Instance.lastArtTexture.height/Data.Instance.lastArtTexture.width);
				}

				string result = "";
				if (Data.Instance.unidad == Data.UnitSys.CM) {
					result = size.x+" cm x "+size.y+" cm";
				} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
					result = Mathf.Round(CustomMath.cm2inches(size.x))+" inches x "+Mathf.Round(CustomMath.cm2inches(size.y))+" inches";
				}        
				return result;

			}
        } 
    }
    void Awake()
    {
        LoadFavorites();
		ReadArtworkData ();
    }
	public void LoadArtFromServer(string json_data){
		//Debug.Log (json_data);
		var N = JSON.Parse(json_data);
		galleries = new GalleryData[N ["galleries"].Count];

		for (int i=0; i<galleries.Length; i++) {
			galleries [i] = new GalleryData();
			galleries [i].title = N ["galleries"] [i] ["title"];
			galleries [i].id = int.Parse(N ["galleries"] [i] ["id"]);
			galleries [i].artWorksData = new List<GalleryData.ArtData>();
		}

		for (int i=0; i<N["artworks"].Count; i++) {
			GalleryData.ArtData adata = new GalleryData.ArtData();
			adata.title = N ["artworks"][i]["title"];
			adata.url = N ["artworks"][i]["url"];

			GalleryData gdata = Array.Find(galleries, g => g.id==int.Parse(N["artworks"][i]["gallery_id"]));
			adata.gallery = gdata.title;
			adata.galleryId = gdata.id;

			adata.artId = int.Parse(N ["artworks"][i]["id"]);
			adata.autor = N ["artworks"][i]["author"];
			adata.technique = N ["artworks"][i]["technique"];
			float w = float.Parse(N ["artworks"][i]["width"]);
			//float h = float.Parse(N ["artworks"][i]["height"]);
			Vector2 size = new Vector2(w,-1);
			adata.size = size;
			adata.isLocal=false;

			gdata.artWorksData.Add(adata);
		}		
	}

    public GalleryData.ArtData GetArtData(int galleryId, int artId)
    {
        GalleryData galleryData = galleries[galleryId];
		if (artId < galleryData.artWorksData.Count)
			return galleryData.artWorksData [artId];
		else
			return null;

    }
    public GalleryData GetCurrentGallery()
	{	
		print ("Selected: " + selectedGallery);
        if (selectedGallery == -1)
			return GetFavourites ();
		else if (selectedGallery == -2)
			return myArtWorks;
        else
            return galleries[selectedGallery];
    }
    public GalleryData GetFavourites()
    {
        GalleryData galleryData = new GalleryData();
        galleryData.title = "FAVORITES";
        galleryData.artWorksData = new List<GalleryData.ArtData>();

        foreach (Favourite favorite in favorites)
        {
            GalleryData.ArtData artData = GetArtData(favorite.galleryId, favorite.artId);            
            galleryData.artWorksData.Add( artData );
        }
        return galleryData;
    }
    public bool isFavorite(int galleryId, int artId)
    {
        foreach (Favourite favourite in favorites)
            if (favourite.galleryId == galleryId && favourite.artId == artId)
                return true;

        return false;
    }

    public void ResetSelectedArtWork()
    {
        selectedArtWork = null;
    }
	public void SetSelectedArtworkByThumbID(int id)
	{	
		print("Gallery Id: "+selectedGallery+" Artworks Id: "+ id);
		if (selectedGallery == -1) {
			selectedArtWork = galleries [Data.Instance.artData.favorites [id].galleryId].artWorksData [id];
			selectedArtWork.gallery = galleries [Data.Instance.artData.favorites [id].galleryId].title;
			selectedArtWork.galleryId = favorites [id].galleryId;
			selectedArtWork.artId = favorites [id].artId;
		} else if (selectedGallery == -2)
			selectedArtWork = myArtWorks.artWorksData [id];
		else {
			selectedArtWork = galleries[selectedGallery].artWorksData[id];
			selectedArtWork.gallery = galleries[Data.Instance.artData.selectedGallery].title;
			selectedArtWork.galleryId = selectedGallery;
			selectedArtWork.artId = id;
		}
	}

	public void SetSelectedArtworkByArtID(int id)
	{	
		print("Gallery Id: "+selectedGallery+" Artworks Id: "+ id);
		if (selectedGallery == -1) {
			selectedArtWork = galleries [Data.Instance.artData.favorites [id].galleryId].artWorksData.Find(x => x.artId==id);
			selectedArtWork.gallery = galleries [Data.Instance.artData.favorites [id].galleryId].title;
			selectedArtWork.galleryId = favorites [id].galleryId;
			selectedArtWork.artId = favorites [id].artId;
		} else if (selectedGallery == -2)
			selectedArtWork = myArtWorks.artWorksData [id];
		else {
			selectedArtWork = galleries[selectedGallery].artWorksData.Find(x => x.artId==id);
			selectedArtWork.gallery = galleries[Data.Instance.artData.selectedGallery].title;
			selectedArtWork.galleryId = selectedGallery;
			selectedArtWork.artId = id;
		}
	}

    public void AddToFavorites()
    {
        Favourite newFavorite = new Favourite();
        newFavorite.galleryId = selectedArtWork.galleryId;
        newFavorite.artId = selectedArtWork.artId;
        favorites.Add(newFavorite);

        SaveFavorites();
    }
    public void RemoveFromFavorites()
    {
        Favourite favoriteToRemove = null;

        foreach (Favourite favorite in favorites)
        {
            if (favorite.galleryId == selectedArtWork.galleryId && favorite.artId == selectedArtWork.artId)
            {
                print("________REMOVE: " + favorite.galleryId + "_" + favorite.artId);
                favoriteToRemove = favorite;
            }
        }

        if (favoriteToRemove != null)
           favorites.Remove(favoriteToRemove);

        SaveFavorites();
    }
    private void SaveFavorites()
    {
        string str = "";
        foreach (Favourite favorite in favorites)
            str += favorite.galleryId + "_" + favorite.artId + ":";

        PlayerPrefs.SetString("favorites", str);


        print("SAVE: " + str);
    }
	public void SaveArtWork(string url, string name, string author, float width, float height)
	{
		int id = myArtWorks.artWorksData.Count;
		SaveArtWork (url, name, author, width, height, id);
	}

	public void SaveArtWork(string url, string name, string author, float width, float height, int id)
	{
		string result = url + ":";				

		result += url + "_" + name + "_" +id+ "_" + author + "_" + width + "_" + height ;

		string DataName = "artwork_"+id;

		PlayerPrefs.SetString(DataName, result);
		
		print("graba: " + DataName + " : " + result);
		
		GetComponent<RoomsData>().ReadRoomsData();

		ReadArtworkData ();
	}

	public void ReadArtworkData()
	{

		myArtWorks.artWorksData.Clear ();
		for (var id = 0; id < 100; id++)
		{
			string artworkData = PlayerPrefs.GetString("artwork_" + id);
			
			if (artworkData != "" && artworkData != null)
			{
				//  print("room_" + id + "   ---->  " + roomData);
				GalleryData.ArtData last = new GalleryData.ArtData();
				
				string[] result = artworkData.Split(":"[0]);
				last.url = result[0];
				string[] res = result[1].Split("_"[0]);
				last.title = res[1];
				last.artId = int.Parse(res[2]);
				last.autor = res[3];
				last.size = new Vector2(float.Parse(res[4]), float.Parse(res[5]));
				last.galleryId=-2;
				last.gallery="My Artworks";
				last.isLocal = true;

				myArtWorks.artWorksData.Add(last);			

			}
		}
	}

	public void DeleteArtworkData (int id){
		string artworkData = PlayerPrefs.GetString("artwork_" + id);
		string path = ""; 
		if (artworkData != "" && artworkData != null) {			
			string[] result = artworkData.Split (":" [0]);
			path = result [0];
		}
		Data.Instance.DeletePhotoArt (path);
		PlayerPrefs.DeleteKey ("artwork_" + id);
		ReadArtworkData ();
	}

    private void LoadFavorites()
    {
        string str = PlayerPrefs.GetString("favorites");
        string[] favoritesArr = str.Split(":"[0]);
        foreach (string result in favoritesArr)
        {
            if (result.Length > 1)
            {
                string[] resultArr = result.Split("_"[0]);

                Favourite newFavorite = new Favourite();
                newFavorite.galleryId = int.Parse(resultArr[0]);
                newFavorite.artId = int.Parse(resultArr[1]);

                favorites.Add(newFavorite);
            }
        }        
    }
}
