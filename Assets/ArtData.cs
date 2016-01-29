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
        public string phone;
        public string email;
        public string web;
        public string thumbnail;

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
				if (size.x == -1) {
					size.x = Mathf.Round(size.y*Data.Instance.lastArtTexture.width/Data.Instance.lastArtTexture.height);
				}
			}

			public string getSizeWUnits(){

				if (size.x == -1) {
                    size.x = Mathf.Round(size.y * Data.Instance.lastArtTexture.width / Data.Instance.lastArtTexture.height);
				}

				string result = "";
				if (Data.Instance.unidad == Data.UnitSys.CM) {
					result = size.x+" x "+size.y+" cm";
				} else if (Data.Instance.unidad == Data.UnitSys.INCHES) {
					result = Mathf.Round(CustomMath.cm2inches(size.x))+" x "+Mathf.Round(CustomMath.cm2inches(size.y))+" inches";
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
	public void LoadGalleryData(string json_data){
		Debug.Log (json_data.Length);
		var N = JSON.Parse(json_data);
		galleries = new GalleryData[N ["galleries"].Count];
        string jsonArtworks = Data.Instance.json_artworks_jsonUrl;

		for (int i=0; i<galleries.Length; i++) {
			galleries [i] = new GalleryData();
			galleries [i].title = N ["galleries"] [i] ["title"];
            int id = int.Parse(N ["galleries"] [i] ["id"]);
			galleries [i].id = id;
            galleries[i].thumbnail = (N["galleries"][i]["thumbnail"]);
			galleries [i].artWorksData = new List<GalleryData.ArtData>();
		}
        //carga las obras:
        for (int i = 0; i < galleries.Length; i++)
        {
            StartCoroutine(GetArtworksData(galleries[i], jsonArtworks + galleries[i].id));
        }	
	}


    public IEnumerator GetArtworksData(GalleryData gdata, string url)
    {
        WWW textURLWWW = new WWW(url);
        yield return textURLWWW;
        LoadArtWorkFromGallery(gdata, textURLWWW.text);
    }


    public void LoadArtWorkFromGallery(GalleryData gdata, string json_data)
    {
        var N = JSON.Parse(json_data);

        for (int i = 0; i < N["artworks"].Count; i++)
        {
            GalleryData.ArtData adata = new GalleryData.ArtData();
            adata.title = N["artworks"][i]["title"];
            adata.url = N["artworks"][i]["url"];

            if (gdata != null)
            {
                adata.gallery = gdata.title;
                adata.galleryId = gdata.id;

                adata.artId = int.Parse(N["artworks"][i]["id"]);
                adata.autor = N["artworks"][i]["author"];
                adata.technique = N["artworks"][i]["technique"];
                float h = float.Parse(N["artworks"][i]["height"]);
                h = CustomMath.inches2cm(h);
                //float h = float.Parse(N ["artworks"][i]["height"]);
                Vector2 size = new Vector2(-1, h);
                adata.size = size;
                adata.isLocal = false;

                gdata.artWorksData.Add(adata);
            }
        }
    }

    public GalleryData.ArtData GetArtData(int galleryId, int artId)
    {
		GalleryData galleryData = Array.Find(galleries, p => p.id == galleryId);
        GalleryData.ArtData artData = null;
        try
        {
            artData = galleryData.artWorksData.Find(x => x.artId == artId);
        }
        catch
        {
            artData = null;
        }
        return artData;
		/*if (artId < galleryData.artWorksData.Count)
			return galleryData.artWorksData [artId];
		else
			return null;*/

    }

	public List<GalleryData.ArtData> GetArtDataList(int galleryId)
	{
		GalleryData galleryData = Array.Find(galleries, p => p.id == galleryId);
		return galleryData.artWorksData;
		/*if (artId < galleryData.artWorksData.Count)
			return galleryData.artWorksData [artId];
		else
			return null;*/
		
	}

    public GalleryData GetCurrentGallery()
	{	
        if (selectedGallery == -1)
			return GetFavourites ();
		else if (selectedGallery == -2)
			return myArtWorks;
        else
			return Array.Find(galleries, p => p.id == selectedGallery);
    }
    public GalleryData GetFavourites()
    {
        GalleryData galleryData = new GalleryData();
        galleryData.title = "Favourites";
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
			Favourite fav = favorites.Find(x => x.artId==id);
			GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork = gd.artWorksData.Find(x => x.artId==id);
			/*selectedArtWork = galleries [Data.Instance.artData.favorites [id].galleryId].artWorksData [id];
			selectedArtWork.gallery = galleries [Data.Instance.artData.favorites [id].galleryId].title;
			selectedArtWork.galleryId = favorites [id].galleryId;
			selectedArtWork.artId = favorites [id].artId;*/
        }
        else if (selectedGallery == -2)
        {
            selectedArtWork = myArtWorks.artWorksData.Find(x => x.artId == id);
        }
        else
        {
            GalleryData gd = Array.Find(galleries, p => p.id == selectedGallery);
            selectedArtWork = gd.artWorksData.Find(x => x.artId == id);
            /*selectedArtWork = galleries[selectedGallery].artWorksData[id];
            selectedArtWork.gallery = galleries[Data.Instance.artData.selectedGallery].title;
            selectedArtWork.galleryId = selectedGallery;
            selectedArtWork.artId = id;*/
        }
	}

	public void SetSelectedArtworkByArtID(int id)
	{	
		print("Gallery Id: "+selectedGallery+" Artworks Id: "+ id);
		if (selectedGallery == -1) {
			Favourite fav = favorites.Find(x => x.artId==id);
			GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork = gd.artWorksData.Find(x => x.artId==id);
			/*selectedArtWork = galleries [Data.Instance.artData.favorites [id].galleryId].artWorksData.Find(x => x.artId==id);
			selectedArtWork.gallery = galleries [Data.Instance.artData.favorites [id].galleryId].title;
			selectedArtWork.galleryId = favorites [id].galleryId;
			selectedArtWork.artId = favorites [id].artId;*/
        }
        else if (selectedGallery == -2)
        {
            selectedArtWork = myArtWorks.artWorksData.Find(x => x.artId == id);
           // selectedArtWork = myArtWorks.artWorksData[id];
        }
        else
        {
            GalleryData gd = Array.Find(galleries, p => p.id == selectedGallery);
            selectedArtWork = gd.artWorksData.Find(x => x.artId == id);
            /*selectedArtWork.gallery = gd.title;
            selectedArtWork.galleryId = selectedGallery;
            selectedArtWork.artId = id;*/
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
		int id = GetUnexistingID();
		SaveArtWork (url, name, author, width, height, id);
	}
    public int GetUnexistingID()
    {
        for (int a = 0; a < 100; a++)
        {
            bool thisIDExists = false;
            foreach(GalleryData.ArtData artData in myArtWorks.artWorksData)
            {
                if (artData.artId == a) thisIDExists = true;                
            }
            if (!thisIDExists) return a;
        }
        return 0;
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
