using UnityEngine;
using System.Collections;
using System;

public class ArtWorks : MonoBehaviour
{

    public ThumbImage button;
    public GameObject container;

    [Serializable]
    public class ThumbData
    {
        public string title;
        public int id;
    }

    public ThumbData[] data;
    public Vector2 thumbSize = new Vector2(195, 120);
    public Vector2 separation = new Vector2(2, 2);
    public int cols;

    private bool isOn;
    public void Toogle()
    {
        if (isOn)
            SetOff();
        else
            Start();
        isOn = !isOn;
    }
    void Start()
    {
        thumbSize += separation;

        int id = 1;
        int separationY = 0;
        int separationx = 0;
        foreach (ThumbData dropData in data)
        {
            ThumbImage newButton = Instantiate(button);
            newButton.transform.SetParent(container.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = Vector3.zero;
            float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
            float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));
            print(_x);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 0);
          //  newButton.Init(dropData.title, dropData.id);

            if (separationx == cols-1)
            {
                separationY++;
                separationx = 0;
            }
            else separationx++;
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
