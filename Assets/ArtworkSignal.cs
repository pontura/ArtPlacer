using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtworkSignal : MonoBehaviour {

	public InputField name;
	public InputField author;
   
	public string GetName()
	{
		return name.text;
	}

	public string GetAuthor()
	{
		return author.text;
	}
}
