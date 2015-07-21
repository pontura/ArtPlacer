using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoomsData : MonoBehaviour {

    public int actualRoomId;

    [Serializable]
    public class Room
    {
        public string url;
        public List<RoomArea> area;
    }
    [Serializable]
    public class RoomArea
    {
        public int height;
        public Vector3[] pointers;
        public Vector3 position;
    }
    public List<Room> rooms;

	void Start () {
        ReadRoomsData();
	}
    public void ReadRoomsData()
    {
        rooms.Clear();
        for (var id = 0; id < 100; id++)
        {
            string roomData = PlayerPrefs.GetString("room_" + id);

            if (roomData != "" && roomData != null)
            {
                print("room_" + id + "   ---->  " + roomData);
                Room room = new Room();

                string[] result = roomData.Split(":"[0]);
                room.url = result[0];

                string[] areas = result[1].Split("+"[0]);

                room.area = new List<RoomArea>();

                foreach (string area in areas)
                {
                    string[] res = area.Split("_"[0]);
                    print("area: " + area);
                    if (res.Length > 1)
                    {
                        RoomArea roomArea = new RoomArea();
                        roomArea.height = int.Parse(res[0]);
                        roomArea.position = new Vector3(GetFloat(res[1]), GetFloat(res[2]), 0);

                        roomArea.pointers = new Vector3[4];

                        roomArea.pointers[0] = new Vector3(GetFloat(res[3]), GetFloat(res[4]), 0);
                        roomArea.pointers[1] = new Vector3(GetFloat(res[5]), GetFloat(res[6]), 0);
                        roomArea.pointers[2] = new Vector3(GetFloat(res[7]), GetFloat(res[8]), 0);
                        roomArea.pointers[3] = new Vector3(GetFloat(res[9]), GetFloat(res[10]), 0);

                        room.area.Add(roomArea);
                    }
                }
                rooms.Add(room);
            }
        }
    }
    public string GetRoomName(string url)
    {
        int id = 0;
        foreach (Room room in rooms)
        {
            if (room.url == url)
            {
                return "room_" + id;
             }
            id++;
        }
        return "room_" + rooms.Count;
    }
    private float GetFloat(string stringValue)
    {
        float result = 0;
        float.TryParse(stringValue, out result);
        return result;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
