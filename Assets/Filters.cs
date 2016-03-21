using UnityEngine;
using System.Collections;

public class Filters : MonoBehaviour {

	void Start () {
        Data.Instance.SetBackActive(true);
        Data.Instance.SetTitle("Select a filter");
        Events.Back += Back;
	}
    void OnDestroy()
    {
        Events.Back -= Back;
    }
    public void Clicked(string filter)
    {
        Data.Instance.filtersManager.activeFilter = filter;
        Data.Instance.LoadLevel("Filter");
    }
    public void Back()
    {
        Data.Instance.LoadLevel("Galleries");
    }
}
