﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ArtArea : MonoBehaviour {

	public List<Area> areas;

	[Serializable]
	public class Area{
		public Vector3[] pointers;
		public Vector3 position;
        public int height;
		public Area(){
			pointers = new Vector3[4];
		}
	}

	public void AddAreas(int id, Vector3[] pointers, Vector3 position){
		if (id < 0) {
			Area area = new Area();
			area.pointers = pointers;
			area.position = position;
			areas.Add (area);
		} else {
			areas[id].pointers = pointers;
			areas[id].position = position;
		}		
	}

	public Vector3[] getPointers(int id){
		return areas[id].pointers;
	}

	public Vector3 getPosition(int id){
		return areas[id].position;
	}

    public void Save()
    {
        string result = "";
        foreach (Area area in areas)
        {
            result += Math.Round(area.position.x, 2) + "_" + Math.Round(area.position.y,2) + "_";
            foreach (Vector3 pointers in area.pointers)
            {
                result += Math.Round(pointers.x, 2) + "_" + Math.Round(pointers.y, 2) + "_";
            }
            result += "+";
        }
        print("Guarda:   " + result);
    }
}
