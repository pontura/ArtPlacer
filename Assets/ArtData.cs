using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        } 
    }
    void Awake()
    {
        LoadFavorites();
		ReadArtworkData ();
    }
    public GalleryData.ArtData GetArtData(int galleryId, int artId)
    {
        GalleryData galleryData = galleries[galleryId];
        return galleryData.artWorksData[artId];
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

	public void SetSelectedArtwork(int id)
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
		string result = url + ":";

		int id = myArtWorks.artWorksData.Count;

		/*GalleryData.ArtData last = myArtWorks.artWorksData [myArtWorks.artWorksData.Count - 1];

		result += last.url + "_" + last.title + "_" + last.autor + "_" + Math.Round(last.size.x, 2) + "_" + Math.Round(last.size.y, 2) + "_";

		string DataName = "artwork_"+(myArtWorks.artWorksData.Count-1);*/
		

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

				myArtWorks.artWorksData.Add(last);			

			}
		}
	}

	public void DeleteArtworkData (int id){
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
