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
}
