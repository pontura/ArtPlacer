using UnityEngine;
using System.Collections;

public class SelectArtworks : MonoBehaviour {

	void Start () {
        Events.HelpHide();
        Data.Instance.SetMainMenuActive(true);

        Data.Instance.SetTitle("Artworks");
        Events.Back += Back;
	}
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    public void Back()
    {
        Data.Instance.LoadLevel("Intro");
    }
	public void Galleries()
    {
        Data.Instance.LoadLevel("Galleries");        
    }
    public void MyArtworks()
    {
        Data.Instance.artData.selectedGallery = -2;
        Data.Instance.LoadLevel("Artworks");
    }
    public void Favourites()
    {
        Data.Instance.artData.selectedGallery = -1;
        Data.Instance.LoadLevel("Artworks");
    }
    public void MySearch()
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
