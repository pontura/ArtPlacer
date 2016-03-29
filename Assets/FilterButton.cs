using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FilterButton : MonoBehaviour {

    public Text titleField;
    private string title;
    private Filter filter;
    public int id;

	public void Init(Filter filter, string title, int id) {
        this.id = id;
        this.filter = filter;
        this.title = title;
        titleField.text = title;
	}
    public void Clicked()
    {
        filter.Clicked(id);
    }
}
