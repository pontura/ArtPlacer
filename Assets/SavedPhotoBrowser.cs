using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

public class SavedPhotoBrowser : MonoBehaviour
{
    public Text debbugText;
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

    void Start()
    {
        Data.Instance.SetMainMenuActive(true);

        thumbSize += separation;

        int id = 0;
        int separationY = 0;
        int separationx = 0;


        //FileInfo[] files =  Data.Instance.GetFilesIn("Images");

        foreach (RoomsData.Room room in Data.Instance.roomsData.rooms)
        {
            ThumbImage newButton = Instantiate(button);

            string folder = Data.Instance.GetRoomsPath();

            string imageName = room.url + ".png";
            var filePath = Path.Combine(folder, imageName);

            debbugText.text = filePath;

            print(filePath);

            newButton.InitRoom(this, filePath, id);

            newButton.transform.SetParent(container.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = Vector3.zero;
            float _x = (thumbSize.x / 2) + (thumbSize.x * separationx);
            float _y = (-thumbSize.y / 2) + (-1 * (thumbSize.y * separationY));

            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(_x, _y, 1);

            if (separationx == cols - 1)
            {
                separationY++;
                separationx = 0;
            }
            else separationx++;
            id++;
        }
    }
    public void OnSelect(int id)
    {
        RoomsData.Room room = Data.Instance.roomsData.rooms[id];
        Data.Instance.areaData.url = room.url;
        Data.Instance.areaData.id = 1;
        foreach (RoomsData.RoomArea roomArea in room.area)
        {
            Data.Instance.areaData.AddAreas(-1, roomArea.pointers, roomArea.position, roomArea.height);
        }
        Data.Instance.LoadLevel("ArtPlaced");
    }
    void SetOff()
    {
        foreach (DropboxButton child in container.GetComponentsInChildren<DropboxButton>())
        {
            Destroy(child.gameObject);
        }
    }
}
