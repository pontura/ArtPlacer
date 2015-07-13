using UnityEngine;
using System.Collections;
using System;

public class Dropbox : MonoBehaviour {

    public DropboxButton button;
    public GameObject container;

    [Serializable]
    public class DropData
    {
        public string title;
        public int id;
    }

    public DropData[] data;
    public int separation;

    private bool isOn;
    public void Toogle()
    {
        if (isOn)
            SetOff();
        else
            SetOn();
        isOn = !isOn;
    }
	void SetOn () {
        int id = 1;
        foreach (DropData dropData in data)
        {
            DropboxButton newButton = Instantiate(button);
            newButton.transform.SetParent( container.transform );
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = new Vector3(0, (-separation /2) + (- 1 * (separation * id)), 0);
            newButton.Init(dropData.title, dropData.id);
            id++;
        }
	}
    void SetOff()
    {
        foreach (DropboxButton child in container.GetComponentsInChildren<DropboxButton>())
        {
            Destroy(child.gameObject);
        }
    }
}
