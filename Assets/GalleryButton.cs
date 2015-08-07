using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GalleryButton : MonoBehaviour {

    public Text title;

    public void Init(Galleries galleries, int id, string _title, string url)
    {
        title.text = _title;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(galleries, id);
        });
    }
    public void OnSelected(Galleries galleries, int id)
    {
        galleries.OnSelect(id);
    }
}
