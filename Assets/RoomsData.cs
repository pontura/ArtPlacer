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
        public int id;
        public int height;
        public Vector3[] pointers;
        public Vector3 position;
    }
    public List<Room> rooms;

	void Start () {
        for (var id = 0; id < 100; id++)
        {
            string roomData = PlayerPrefs.GetString("room_" + id);
            if (roomData != "" && roomData != null)
            {
                Room room = new Room();

                string[] result = roomData.Split(":"[0]);
                room.url = result[0];
                string[] res = result[1].Split("_"[0]);

                room.height = int.Parse(res[0]);
                room.position = new Vector3(GetFloat(res[1]), GetFloat(res[2]), 0);
                
                room.pointers = new Vector3[4];

                room.pointers[0] = new Vector3(GetFloat(res[3]), GetFloat(res[4]), 0);
                room.pointers[1] = new Vector3(GetFloat(res[5]), GetFloat(res[6]), 0);
                room.pointers[2] = new Vector3(GetFloat(res[7]), GetFloat(res[8]), 0);
                room.pointers[3] = new Vector3(GetFloat(res[9]), GetFloat(res[10]), 0);


                rooms.Add(room);

               // Data.Instance.areaData.url = areaData.url;
               // Data.Instance.areaData.areas.Add(area);
            }
        }
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
