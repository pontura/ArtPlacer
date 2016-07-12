using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using System.Text.RegularExpressions;

public class ArtData : MonoBehaviour {

    public GalleryData.ArtData selectedArtWork;

    public int selectedGallery;

    public GalleryData[] galleries;
	public GalleryData myArtWorks;
    public List<Favourite> favorites;

    public List<Favourite> filter;

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
        public class ArtDataFilters
        {
            public List<int> color;
           // public List<int> style;
            public List<int> orientation;
            public List<int> technique;
            public List<int> size;
            public List<int> shape;
        }
        [Serializable]
        public class ArtData
        {
            public string title;
            public string url;
            public string thumbnail;
            public string gallery;
            public int galleryId;
            public int artId;
            public string autor;
            public string technique;
            public ArtDataFilters filters;
            public Vector2 size;
			public bool isLocal;

			public string GetUrl(bool thumb){
				string result = "";
				if (galleryId == -2) {
					string folder = Data.Instance.GetArtPath ();		
					result = Path.Combine (folder, url + ".png");
				} else {
                    if (thumb)
                        result = thumbnail;
                    else
					    result = url;
				}
				return result;
			}

			public void Clone(ArtData source){
				title = source.title;
				url = source.url;
				thumbnail = source.thumbnail;
				gallery = source.gallery;
				galleryId = source.galleryId;
				artId = source.artId;
				autor = source.autor;
				technique = source.technique;
				filters = source.filters;
				size = source.size;
				isLocal = source.isLocal;
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

		var N = JSON.Parse(json_data);
		galleries = new GalleryData[N ["galleries"].Count];
        string jsonArtworks = Data.Instance.json_artworks_jsonUrl;

		for (int i=0; i<galleries.Length; i++) {
			galleries [i] = new GalleryData();
			galleries [i].title = N ["galleries"] [i] ["title"];
            string phoneNum = N["galleries"][i]["phone"];
            if (phoneNum!= null)
                galleries[i].phone = Regex.Replace(phoneNum, "[^0-9]", "");
            galleries[i].email = N["galleries"][i]["email"];

            string url = N["galleries"][i]["website"];
            if (url != null)
            {
                 if( url.Substring(0, 4) == "http")
                    galleries[i].web = url;
                else 
                    galleries[i].web = "http://" + url;
            }
            int id = int.Parse(N ["galleries"] [i] ["id"]);
			galleries [i].id = id;
            galleries[i].thumbnail = (N["galleries"][i]["thumbnail"]);
			galleries [i].artWorksData = new List<GalleryData.ArtData>();
		}
        //carga las obras:
        for (int i = 0; i < galleries.Length; i++)
        {
			StartCoroutine(GetArtworksData(galleries[i], jsonArtworks + galleries[i].id + "&no-cache=" + UnityEngine.Random.value));
        }	
	}


    public IEnumerator GetArtworksData(GalleryData gdata, string url)
    {
       // url = "http://localhost/madrollers/artplacer.json";
		//Debug.Log (url);
        WWW textURLWWW = new WWW(url);
        yield return textURLWWW;
		//Debug.Log (url+" "+textURLWWW.text.Length+" : "+textURLWWW.text);

        if (textURLWWW.text.Contains("<html>"))
        {
            print("____________________________ vuelve a intentar");
            StartCoroutine(GetArtworksData(gdata, url));
        }
        else
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
                adata.thumbnail = N["artworks"][i]["thumbnail"];

                Data.Instance.filtersManager.CheckToAddFilter("autor", adata.autor);
                float h = float.Parse(N["artworks"][i]["height"]);
                adata.filters = new GalleryData.ArtDataFilters();

                adata.filters.color = new List<int>();
                adata.filters.size = new List<int>();
                adata.filters.shape = new List<int>();
                adata.filters.orientation = new List<int>();
                adata.filters.technique = new List<int>();

                for (int b = 0; b < N["artworks"][i]["filter"]["color"].Count; b++)
                {
                    //  Data.Instance.filtersManager.CheckToAddFilter("color", N["artworks"][i]["filter"]["color"][b]);                            
                    adata.filters.color.Add(int.Parse(N["artworks"][i]["filter"]["color"][b]));
                }

                for (int b = 0; b < N["artworks"][i]["filter"]["size"].Count; b++)
                {
                    // Data.Instance.filtersManager.CheckToAddFilter("size", N["artworks"][i]["filter"]["size"][b]);                            
                    adata.filters.size.Add(int.Parse(N["artworks"][i]["filter"]["size"][b]));
                }

                for (int b = 0; b < N["artworks"][i]["filter"]["shape"].Count; b++)
                {
                    //  Data.Instance.filtersManager.CheckToAddFilter("shape", N["artworks"][i]["filter"]["shape"][b]);                           
                    adata.filters.shape.Add(int.Parse(N["artworks"][i]["filter"]["shape"][b]));
					//Debug.Log ("Shape"+i+": "+int.Parse(N["artworks"][i]["filter"]["shape"][b]));
                }

                for (int b = 0; b < N["artworks"][i]["filter"]["orientation"].Count; b++)
                {
                    // Data.Instance.filtersManager.CheckToAddFilter("orientation", N["artworks"][i]["filter"]["orientation"][b]);                           
                    adata.filters.orientation.Add(int.Parse(N["artworks"][i]["filter"]["orientation"][b]));
					//Debug.Log ("Orientation"+i+": "+int.Parse(N["artworks"][i]["filter"]["orientation"][b]));
                }

                for (int b = 0; b < N["artworks"][i]["filter"]["technique"].Count; b++)
                {
                    //  Data.Instance.filtersManager.CheckToAddFilter("technique", N["artworks"][i]["filter"]["technique"][b]);                           
                    adata.filters.technique.Add(int.Parse(N["artworks"][i]["filter"]["technique"][b]));
                } 

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
        if (galleryId == -2)
        {
            artData = myArtWorks.artWorksData.Find(x => x.artId == artId);
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
    public GalleryData GetGallery(int id)
    {
        return Array.Find(galleries, p => p.id == id);
    }
    public GalleryData GetCurrentGallery()
	{
        if (selectedGallery == -1)
            return GetFavourites();
        else if (selectedGallery == -2)
            return myArtWorks;
        else if (selectedGallery == -3)
            return GetFiltered();
        else
            return Array.Find(galleries, p => p.id == selectedGallery);
    }
    public GalleryData GetFavourites()
    {
        GalleryData galleryData = new GalleryData();
        galleryData.title = "Favorites";
        galleryData.artWorksData = new List<GalleryData.ArtData>();

        foreach (Favourite favorite in favorites)
        {
            GalleryData.ArtData artData = GetArtData(favorite.galleryId, favorite.artId);
            galleryData.artWorksData.Add(artData);
        }
        return galleryData;
    }
    public GalleryData GetFiltered()
    {
        GalleryData galleryData = new GalleryData();
        string filters = "Filter:";
        for (int a = 0; a < Data.Instance.filtersManager.activeFilter.Count; a++)
        {
            filters += " " + Data.Instance.filtersManager.GetActiveFilter(a);
            if(a<Data.Instance.filtersManager.activeFilter.Count)
                filters +=",";
        }
        galleryData.title = filters;
        galleryData.artWorksData = new List<GalleryData.ArtData>();

        foreach (Favourite favorite in filter)
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

		if (selectedGallery == -1) {
			Favourite fav = favorites.Find(x => x.artId==id);
			GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId==id));
        }
        else if (selectedGallery == -3)
        {
            Favourite fav = filter.Find(x => x.artId == id);
            GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId == id));
        }
        else if (selectedGallery == -2)
        {
			selectedArtWork.Clone(myArtWorks.artWorksData.Find(x => x.artId == id));
        }        
        else
        {
            GalleryData gd = Array.Find(galleries, p => p.id == selectedGallery);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId == id));
        }
	}

	public void SetSelectedArtworkByArtID(int id)
	{	
		//print("Gallery Id: "+selectedGallery+" Artworks Id: "+ id);
		if (selectedGallery == -1) {
			Favourite fav = favorites.Find(x => x.artId==id);
			GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId==id));
        }
        else if (selectedGallery == -3)
        {
            Favourite fav = filter.Find(x => x.artId == id);
            GalleryData gd = Array.Find(galleries, p => p.id == fav.galleryId);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId == id));
        }
        else if (selectedGallery == -2)
        {
			selectedArtWork.Clone(myArtWorks.artWorksData.Find(x => x.artId == id));
        }
        else
        {
            GalleryData gd = Array.Find(galleries, p => p.id == selectedGallery);
			selectedArtWork.Clone(gd.artWorksData.Find(x => x.artId == id));
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
        print("SAVE: id:  " + id);
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
