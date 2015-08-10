using UnityEngine;
using System.Collections;
using System;

public class ArtData : MonoBehaviour {

    public GalleryData.ArtData selectedArtWork;

    public int selectedGallery;

    public GalleryData[] galleries;

    [Serializable]
    public class GalleryData
    {
        public string title;
        public ArtData[] artWorksData;

        [Serializable]
        public class ArtData
        {
            public string title;
            public string url;
            public string gallery;
            public string autor;
            public string technique;
            public Vector2 size;            

        } 

    }
    public GalleryData GetCurrentGallery()
    {
        return galleries[selectedGallery];
    }
}
