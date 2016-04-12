using UnityEngine;
using System.Collections;

public class Filters : MonoBehaviour {

    public GameObject filtersButton;

	void Start () {
        Data.Instance.SetBackActive(true);
        SetTitle();
        Events.Back += Back;
        filtersButton.SetActive(false);
        if( Data.Instance.filtersManager.activeFilter.Count>0)
            filtersButton.SetActive(true);
	}
    void SetTitle()
    {
        string title = "Select a filter";
        if (Data.Instance.filtersManager.activeFilter.Count > 0)
        {
            title += " (" + Data.Instance.filtersManager.activeFilter.Count + " selected)";
        }
        Data.Instance.SetTitle(title);
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    public void Clicked(string filter)
    {
        Data.Instance.filtersManager.AddFilter(filter);
        Data.Instance.LoadLevel("Filter");
    }
    public void Back()
    {
        if (Data.Instance.lastScene == "SelectArtworks")
        {
            Data.Instance.LoadLevel("SelectArtworks");
        }
        else
        {
            Data.Instance.LoadLevel("Galleries");
        }
    }
    public void ClearFilters()
    {
        Data.Instance.filtersManager.Clear();
        filtersButton.SetActive(false);
        SetTitle();
    }
}
