using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON;

public class FiltersManager : MonoBehaviour {

    private string URL = "http://www.artplacer.com/getalldata.php?type=filters";

    [Serializable]
    public class FilterData
    {
        public int id;
        public string name;
    }
    private bool allArtworksLoaded;
    public List<FilterData> color;
    public List<FilterData> style;
    public List<FilterData> orientation;
    public List<FilterData> technique;
    public List<FilterData> autor;
    public List<FilterData> size;
    public List<FilterData> shape;

    public int activeValue;
    public string activeFilter;

    void Start()
    {
        allArtworksLoaded = false;
        StartCoroutine(LoadData());
    }
	public void CheckToAddFilter(string filter, string value)
    {
        switch (filter)
        {
            //case "color": CheckToCreate(color, value); break;
            //case "style": CheckToCreate(style, value); break;
            //case "orientation": CheckToCreate(orientation, value); break;
            //case "technique": CheckToCreate(technique, value); break;
            case "autor": CheckToCreate(autor, value); break;
            //case "size": CheckToCreate(size, value); break;
            //case "shape": CheckToCreate(shape, value); break;
        }
    }
    public List<FilterData> GetCurrentFilter()
    {
        switch (activeFilter)
        {
            case "color": return color;
         //   case "style": return style;
            case "orientation": return  orientation;
            case "technique": return  technique;
            case "autor": return  autor;
            case "size": return size;
            case "shape": return shape;
        }
        return null;
    }
    public void SetFilteredValue(int value)
    {
        this.activeValue = value;
    }
    public string GetActiveFilter()
    {
        List<FilterData> arr = null;
        switch (activeFilter)
        {
            case "color": arr = color; break;
         //   case "style": arr = style; break;
            case "orientation": arr = orientation; break;
            case "technique": arr = technique; break;
            case "autor": arr = autor; break;
            case "size": arr = size; break;
            case "shape": arr = shape; break;
        }
        if (arr != null)
        {
            foreach (FilterData data in arr)
            {
                if (data.id == activeValue)
                {
                    return data.name;
                }
            }
        }
        return null;
    }
    void CheckToCreate(List<FilterData> arr, string value)
    {
        if (value == null || value == "") return;

        int a = 1;
        foreach (FilterData existingValue in arr)
        {
            a++;
            if (existingValue.name == value)
                return;
        }

        FilterData data = new FilterData();
        data.id = a;
        data.name = value;
        arr.Add(data);
    }
    public void CreateGalleryBasedOnSelectedFilter()
    {
        Data.Instance.artData.filter.Clear();
        
        foreach(ArtData.GalleryData galleryData in Data.Instance.artData.galleries)
        {
            foreach (ArtData.GalleryData.ArtData artData in galleryData.artWorksData)
            {
                List<int> arr = new List<int>();
                switch (activeFilter)
                {
                    case "color": arr = artData.filters.color; break;
                    //case "style": arr = artData.filters.style; break;
                    case "orientation": arr = artData.filters.orientation; break;
                    case "technique": arr = artData.filters.technique; break;                  
                    case "size": arr = artData.filters.size; break;
                    case "shape": arr = artData.filters.shape; break;
                    case "autor":
                        if (GetActiveFilter() == artData.autor)
                        {
                            ArtData.Favourite filters = new ArtData.Favourite();
                            filters.artId = artData.artId;
                            filters.galleryId = artData.galleryId;
                            Data.Instance.artData.filter.Add(filters);
                        }
                        break;
                }
                if (arr != null)
                {
                    foreach (int value in arr)
                    {
                        if (value == activeValue)
                        {
                            ArtData.Favourite filters = new ArtData.Favourite();
                            filters.artId = artData.artId;
                            filters.galleryId = artData.galleryId;
                            Data.Instance.artData.filter.Add(filters);
                        }
                    }
                }
            }
        }        
    }
    public IEnumerator LoadData()
    {
        WWW textURLWWW = new WWW(URL);
        yield return textURLWWW;

        var N = JSON.Parse(textURLWWW.text);

        for (int a = 0; a < 50; a++)
        {
            if (N["filters"]["shapes"][a] != null)
            {
                FilterData data = new FilterData();
                data.id = a;
                data.name = N["filters"]["shapes"][a];
                shape.Add(data);
            }
            if (N["filters"]["techniques"][a] != null)
            {
                FilterData data = new FilterData();
                data.id = a;
                data.name = N["filters"]["techniques"][a];
                technique.Add(data);
            }
            if (N["filters"]["sizes"][a] != null)
            {
                FilterData data = new FilterData();
                data.id = a;
                data.name = N["filters"]["sizes"][a];
                size.Add(data);
            }
            if (N["filters"]["orientations"][a] != null)
            {
                FilterData data = new FilterData();
                data.id = a;
                data.name = N["filters"]["orientations"][a];
                orientation.Add(data);
            }
            if (N["filters"]["colors"][a] != null)
            {
                FilterData data = new FilterData();
                data.id = a;
                data.name = N["filters"]["colors"][a];
                color.Add(data);
            }
        }
    }
}
