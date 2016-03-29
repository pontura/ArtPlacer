using UnityEngine;
using System.Collections;

public class Filter : MonoBehaviour
{
    public FilterButton filterButton;
    public Transform container;

    void Start()
    {
        Data.Instance.SetBackActive(true);
        Data.Instance.SetTitle("Selecting " + Data.Instance.filtersManager.activeFilter);
        Events.Back += Back;
        foreach (FiltersManager.FilterData data in Data.Instance.filtersManager.GetCurrentFilter())
        {
            FilterButton newButton = Instantiate(filterButton);
            newButton.transform.SetParent(container);
            newButton.Init(this, data.name, data.id);
            newButton.transform.localScale = Vector2.one;
        }
    }
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    public void Clicked(int id)
    {
        Data.Instance.filtersManager.SetFilteredValue(id);
        Data.Instance.filtersManager.CreateGalleryBasedOnSelectedFilter();
        Data.Instance.artData.selectedGallery = -3;
        Data.Instance.LoadLevel("Artworks");
    }
    public void Back()
    {
        Data.Instance.LoadLevel("Filters");
    }
}
