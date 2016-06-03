using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ImageCache{
	public string url;
	public Texture2D texture;

	public ImageCache(string URL, Texture2D tex){
		url = URL;
		texture = tex;
	}
}