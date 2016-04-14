using UnityEngine;
using System.Collections;

public class FilterSearchButton : MonoBehaviour {

    public void GotoSearch()
    {
        if (Data.Instance.filtersManager.activeFilter.Count > 0)
        {
            Data.Instance.artData.selectedGallery = -3;
            Data.Instance.LoadLevel("Artworks");
        }
        else
        {
            Data.Instance.LoadLevel("Filters");
        }
    }
}
