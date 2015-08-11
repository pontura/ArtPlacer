using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ArtData : MonoBehaviour {

    public GalleryData.ArtData selectedArtWork;

    public int selectedGallery;

    public GalleryData[] galleries;
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
    void Start()
    {
        LoadFavorites();
    }
    public GalleryData.ArtData GetArtData(int galleryId, int artId)
    {
        GalleryData galleryData = galleries[galleryId];
        return galleryData.artWorksData[artId];
    }
    public GalleryData GetCurrentGallery()
    {
        if (selectedGallery == -1)
            return GetFavourites();
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

        print("LoadFavorites: " + str);

        
    }
}
