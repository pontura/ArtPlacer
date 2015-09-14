using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtworkSignal : MonoBehaviour {

    public int id;

	public InputField name;
	public InputField author;

	public GameObject cursor1;
	public GameObject cursor2;
	public GameObject cursor3;
	
	public Text inputField;
	public Text desc;
	
	private int height = 2;
	private int height2 = 0;
	private int height3 = 0;
	
	private int activeNum = 0;
	private ConfirmArtworkSize confirmSizes;
	
	void Start()
	{
		confirmSizes = GameObject.Find("confirmArtworkSize").GetComponent<ConfirmArtworkSize>();
		RefreshField();
	}

	public void Init(float _width , float _height)
	{
		
		height = (int)(_height*0.01);
		height2 = (int)(_height * 0.1 - (height*10));
		height3 = (int)(_height - (height*100) - (height2*10));
		print (_height+" "+height+"."+height2+" "+height3);
		RefreshField();
	}
	public int GetHeight()
	{
		string result = "" + (height + "" + height2 + "" + height3);
		return int.Parse(result);
	}
	public void OnPress(string key)
	{
		switch(key)
		{
		case "<":
			Delete();
			break;
		case "ok":
			Ready();
			break;
		default:
			Add(key);
			break;
		}
	}
	void Add(string key)
	{
		switch (activeNum)
		{
		case 0: height = int.Parse(key); break;
		case 1: height2 = int.Parse(key); break;
		case 2: height3 = int.Parse(key); break;
		}
		if (activeNum < 2)
			activeNum++;
		RefreshField();
	}
	void Ready()
	{
		print("ready " + GetHeight().ToString());
		confirmSizes.Ready();
	}
	void Delete()
	{
		height = 0;
		height2 = 0;
		height3 = 0;
		activeNum = 0;
		RefreshField();
	}
	void Next()
	{
		
	}
	void RefreshField()
	{
		inputField.text = height + "." + height2 + height3;
		desc.text = height + " meters, " + height2 + "" + height3 + " centimeters";
		RefreshCursor();
	}
	void RefreshCursor()
	{
		cursor1.SetActive(false);
		cursor2.SetActive(false);
		cursor3.SetActive(false);
		
		GameObject cursorActive = cursor1;
		if (activeNum == 1) cursorActive = cursor2;
		else if (activeNum == 2) cursorActive = cursor3;
		cursorActive.SetActive(true);
	}

	public string GetName()
	{
		return name.text;
	}

	public string GetAuthor()
	{
		return author.text;
	}
}
