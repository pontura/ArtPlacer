using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Dropbox : MonoBehaviour {

    public Text title;
    public int id;
    public Color activeColor;
    private Button selfButton;
    public DropboxButton button;
    public GameObject container;
    public Dropbox otherDropBox; 

    [Serializable]
    public class DropData
    {
        public string title;
        public int id;
    }

    public List<DropData> data;
    public int separation;

    public bool isOn;

    void Start()
    {
        selfButton = GetComponent<Button>();
        switch (id)
        {
            case 1: 
                foreach( ArtData.GalleryData thisData in Data.Instance.artData.galleries)
                {
                    DropData dropData = new DropData();
                    dropData.title = thisData.title;
                    data.Add(dropData);
                }
                break;
        }
        if (isOn)
            SetOn();
    }
    public void Toogle()
    {
        if (isOn)
            SetOff();
        else
        {
            SetOn();
            otherDropBox.SetOff();
        }
    }
	void SetOn () {
        isOn = true;
        selfButton.GetComponent<Image>().color = activeColor;
        
        int id = 1;
        foreach (DropData dropData in data)
        {
            print("title" + dropData.title);
            DropboxButton newButton = Instantiate(button) as DropboxButton;
            newButton.transform.SetParent( container.transform );
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = new Vector3(0, (-separation /2) + (- 1 * (separation * id)), 0);
            newButton.Init(this, dropData, id-1);
            id++;
        }
	}
    void SetOff()
    {
        isOn = false;
        selfButton.GetComponent<Image>().color = Color.black;
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);

        foreach (DropboxButton child in container.GetComponentsInChildren<DropboxButton>())
        {
            Destroy(child.gameObject);
        }
    }
    public void OnSelect(DropData data, int selectedId)
    {
        title.text = data.title;
        Events.OnDropBoxSelect(id, selectedId);
        SetOff();
    }
}
