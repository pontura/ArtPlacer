using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FilterButton : MonoBehaviour {

    public Text titleField;
    private string title;
    private Filter filter;

	public void Init(Filter filter, string title) {
        this.filter = filter;
        this.title = title;
        titleField.text = title;
	}
    public void Clicked()
    {
        filter.Clicked(title);
    }
}
