using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropboxButton : MonoBehaviour {

    public Text field;
    public int id;

    public void Init(Dropbox dropBox, Dropbox.DropData _data, int id)
    {
        field.text = _data.title;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSelected(dropBox, _data, id);
        });
    }
    public void OnSelected(Dropbox dropBoxParent, Dropbox.DropData data, int id)
    {
        dropBoxParent.OnSelect(data, id);
    }
}
