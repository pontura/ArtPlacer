using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FiltersManager : MonoBehaviour {

    private bool allArtworksLoaded;
    public List<string> color;
    public List<string> style;
    public List<string> orientation;
    public List<string> technique;
    public List<string> autor;
    public List<string> size;
    public List<string> shape;

    public string activeValue;
    public string activeFilter;

    void Start()
    {
        allArtworksLoaded = false;
    }
	public void CheckToAddFilter(string filter, string value)
    {
        switch (filter)
        {
            case "color": CheckToCreate(color, value); break;
            case "style": CheckToCreate(style, value); break;
            case "orientation": CheckToCreate(orientation, value); break;
            case "technique": CheckToCreate(technique, value); break;
            case "autor": CheckToCreate(autor, value); break;
            case "size": CheckToCreate(size, value); break;
            case "shape": CheckToCreate(shape, value); break;
        }
    }
    public List<string> GetCurrentFilter()
    {
        switch (activeFilter)
        {
            case "color": return color;
            case "style": return style;
            case "orientation": return  orientation;
            case "technique": return  technique;
            case "autor": return  autor;
            case "size": return size;
            case "shape": return shape;
        }
        return null;
    }
    public void SetFilteredValue(string value)
    {
        this.activeValue = value;
    }

    void CheckToCreate(List<string> arr, string value)
    {
        foreach (string existingValue in arr)
            if (existingValue == value)
                return;
        arr.Add(value);
    }
    public void CreateGalleryBasedOnSelectedFilter()
    {
        ArtData.Favourite filters = new ArtData.Favourite();

        Data.Instance.artData.filter.Clear();
        
        foreach(ArtData.GalleryData galleryData in Data.Instance.artData.galleries)
        {
            foreach (ArtData.GalleryData.ArtData artData in galleryData.artWorksData)
            {
                List<string> arr = new List<string>();
                switch (activeFilter)
                {
                    case "color": arr = artData.filters.color; break;
                    case "style": arr = artData.filters.style; break;
                    case "orientation": arr = artData.filters.orientation; break;
                    case "technique": arr = artData.filters.technique; break;
                    case "autor": arr.Add( artData.autor ); break;
                    case "size": arr = artData.filters.size; break;
                    case "shape": arr = artData.filters.shape; break;
                }
                if (arr != null)
                {
                    foreach (string value in arr)
                    {
                        if (value == activeValue)
                        {                            
                            filters.artId = artData.artId;
                            filters.galleryId = artData.galleryId;
                            Data.Instance.artData.filter.Add(filters);
                        }
                    }
                }
            }
        }        
    }
}
