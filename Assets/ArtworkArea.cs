using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArtworkArea : MonoBehaviour {

	public GameObject area;
	public Image TL;
	public Image TR;
	public Image BR;
	public Image BL;

	public Color selectedColor;
	public Color normalColor;

	Vector2 deltaMove;

	RectTransform transform;
	RectTransform areaTransform;

	Vector3 offset;

	enum SelectMove {
		None,
		Area,
		TL,
		TR,
		BR,
		BL
	}

	SelectMove selectMove;

	// Use this for initialization
	void Start () {
		transform = gameObject.GetComponent<RectTransform>();
		areaTransform = area.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {

			if (selectMove == SelectMove.Area) {
				Vector3 pos = Input.mousePosition-offset;
				/*if ((pos.x - transform.sizeDelta.x * 0.5f > limitLeft) && (pos.x + transform.sizeDelta.x * 0.5 < limitRight) && (pos.y - transform.sizeDelta.y * 0.5 > limitBottom) && (pos.y + transform.sizeDelta.y * 0.5 < limitTop))		gameObject.transform.position = pos;
				pos.x = pos.x - transform.sizeDelta.x * 0.5f<limitLeft?limitLeft + transform.sizeDelta.x * 0.5f:pos.x;
				pos.x=pos.x + transform.sizeDelta.x * 0.5f>limitRight?limitRight - transform.sizeDelta.x * 0.5f:pos.x;
				pos.y = pos.y - transform.sizeDelta.y * 0.5f<limitBottom?limitBottom + transform.sizeDelta.y * 0.5f:pos.y;
				pos.y = pos.y + transform.sizeDelta.y * 0.5f >limitTop?limitTop - transform.sizeDelta.y * 0.5f:pos.y;*/
				gameObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
			} else if (selectMove == SelectMove.TL) {
				RectTransform rt = gameObject.GetComponent<RectTransform>();
				RectTransform aRt = area.GetComponent<RectTransform>();
				rt.offsetMin = new Vector2(rt.offsetMin.x+(Input.mousePosition.x-deltaMove.x),rt.offsetMin.y);
				rt.offsetMax = new Vector2(rt.offsetMax.x,rt.offsetMax.y+(Input.mousePosition.y-deltaMove.y));
				aRt.offsetMin = new Vector2(aRt.offsetMin.x+(Input.mousePosition.x-deltaMove.x),aRt.offsetMin.y);
				aRt.offsetMax = new Vector2(aRt.offsetMax.x,aRt.offsetMax.y+(Input.mousePosition.y-deltaMove.y));
				deltaMove = Input.mousePosition;
			} else if (selectMove == SelectMove.TR) {
				RectTransform rt = gameObject.GetComponent<RectTransform>();
				RectTransform aRt = area.GetComponent<RectTransform>();
				rt.offsetMax = new Vector2(rt.offsetMax.x+(Input.mousePosition.x-deltaMove.x),rt.offsetMax.y+(Input.mousePosition.y-deltaMove.y));
				rt.offsetMin = new Vector2(rt.offsetMin.x,rt.offsetMin.y);
				aRt.offsetMax = new Vector2(aRt.offsetMax.x+(Input.mousePosition.x-deltaMove.x),aRt.offsetMax.y+(Input.mousePosition.y-deltaMove.y));
				aRt.offsetMin = new Vector2(aRt.offsetMin.x,aRt.offsetMin.y);
				deltaMove = Input.mousePosition;
			} else if (selectMove == SelectMove.BL) {
				RectTransform rt = gameObject.GetComponent<RectTransform>();
				RectTransform aRt = area.GetComponent<RectTransform>();
				rt.offsetMin = new Vector2(rt.offsetMin.x+(Input.mousePosition.x-deltaMove.x),rt.offsetMin.y+(Input.mousePosition.y-deltaMove.y));
				rt.offsetMax = new Vector2(rt.offsetMax.x,rt.offsetMax.y);
				aRt.offsetMin = new Vector2(aRt.offsetMin.x+(Input.mousePosition.x-deltaMove.x),aRt.offsetMin.y+(Input.mousePosition.y-deltaMove.y));
				aRt.offsetMax = new Vector2(aRt.offsetMax.x,aRt.offsetMax.y);
				deltaMove = Input.mousePosition;
			} else if (selectMove == SelectMove.BR) {
				RectTransform rt = gameObject.GetComponent<RectTransform>();
				RectTransform aRt = area.GetComponent<RectTransform>();
				rt.offsetMax = new Vector2(rt.offsetMax.x+(Input.mousePosition.x-deltaMove.x),rt.offsetMax.y);
				rt.offsetMin = new Vector2(rt.offsetMin.x,rt.offsetMin.y+(Input.mousePosition.y-deltaMove.y));
				aRt.offsetMax = new Vector2(aRt.offsetMax.x+(Input.mousePosition.x-deltaMove.x),aRt.offsetMax.y);
				aRt.offsetMin = new Vector2(aRt.offsetMin.x,aRt.offsetMin.y+(Input.mousePosition.y-deltaMove.y));
				deltaMove = Input.mousePosition;
			}
		//}
	}

	public void OnPointerDownArea()
	{
		selectMove = SelectMove.Area;
		offset = Input.mousePosition - transform.position;
	}

	public void OnPointerDownTL()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.TL;
		TL.color = selectedColor;

		RectTransform aRt = area.GetComponent<RectTransform>();
		aRt.anchorMin = new Vector2(1, 0);
		aRt.anchorMax = new Vector2(1, 0);
		aRt.pivot = new Vector2(0.5f, 0.5f);
		Vector3 pos = aRt.anchoredPosition;
		aRt.anchoredPosition = new Vector3 (Mathf.Abs (pos.x) * -1, Mathf.Abs (pos.y), pos.z);
	}

	public void OnPointerDownTR()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.TR;
		TR.color = selectedColor;

		RectTransform aRt = area.GetComponent<RectTransform>();
		aRt.anchorMin = new Vector2(0, 0);
		aRt.anchorMax = new Vector2(0, 0);
		aRt.pivot = new Vector2(0.5f, 0.5f);
		Vector3 pos = aRt.anchoredPosition;
		aRt.anchoredPosition = new Vector3(Mathf.Abs(pos.x),Mathf.Abs(pos.y),pos.z);
	}

	public void OnPointerDownBR()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.BR;
		BR.color = selectedColor;

		RectTransform aRt = area.GetComponent<RectTransform>();
		aRt.anchorMin = new Vector2(0, 1);
		aRt.anchorMax = new Vector2(0, 1);
		aRt.pivot = new Vector2(0.5f, 0.5f);
		Vector3 pos = aRt.anchoredPosition;
		aRt.anchoredPosition = new Vector3(Mathf.Abs(pos.x),Mathf.Abs(pos.y)*-1,pos.z);
	}

	public void OnPointerDownBL()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.BL;
		BL.color = selectedColor;

		RectTransform aRt = area.GetComponent<RectTransform>();
		aRt.anchorMin = new Vector2(1, 1);
		aRt.anchorMax = new Vector2(1, 1);
		aRt.pivot = new Vector2(0.5f, 0.5f);
		Vector3 pos = aRt.anchoredPosition;
		aRt.anchoredPosition = new Vector3 (Mathf.Abs (pos.x) * -1, Mathf.Abs (pos.y)*-1, pos.z);
	}

	public void OnPointerUp(){
		selectMove = SelectMove.None;
		BL.color = normalColor;
		BR.color = normalColor;
		TL.color = normalColor;
		TR.color = normalColor;
		/*Debug.Log ("ScreenW: " + Screen.width + " ScreenH: " + Screen.height); 
		Debug.Log ("Pos: " + transform.position + " pos2: " + transform.rect.position); 
		Debug.Log ("xMin: " + transform.rect.xMin + " xMax: " + transform.rect.xMax); 
		Debug.Log ("yMin: " + transform.rect.yMin + " yMax: " + transform.rect.yMax);
		Debug.Log ("Pos: "+ (transform.position.x + transform.rect.xMin)+", "+(transform.position.y + transform.rect.yMax)+" width: "+transform.rect.width+" height: "+transform.rect.height);*/
	}

}
