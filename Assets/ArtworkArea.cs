using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArtworkArea : MonoBehaviour {

	public GameObject area;
	public Image TL;
	public Image TR;
	public Image BR;
	public Image BL;

	Color selectedColor = Color.gray;
	Color normalColor = Color.white;

	Vector2 deltaMove;

	RectTransform transform;
	RectTransform areaTransform;

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
			gameObject.transform.position = Input.mousePosition;
		} else if (selectMove == SelectMove.TL) {
			Vector2 scale = new Vector2(deltaMove.x/Input.mousePosition.x,Input.mousePosition.y/deltaMove.y);
			transform.sizeDelta = new Vector2(transform.sizeDelta.x*scale.x,transform.sizeDelta.y*scale.y);
			areaTransform.sizeDelta = new Vector2(areaTransform.sizeDelta.x*scale.x,areaTransform.sizeDelta.y*scale.y);
			deltaMove = Input.mousePosition;
		} else if (selectMove == SelectMove.TR) {
			Vector2 scale = new Vector2(Input.mousePosition.x/deltaMove.x,Input.mousePosition.y/deltaMove.y);
			transform.sizeDelta = new Vector2(transform.sizeDelta.x*scale.x,transform.sizeDelta.y*scale.y);
			areaTransform.sizeDelta = new Vector2(areaTransform.sizeDelta.x*scale.x,areaTransform.sizeDelta.y*scale.y);
			deltaMove = Input.mousePosition;
		} else if (selectMove == SelectMove.BL) {
			Vector2 scale = new Vector2(deltaMove.x/Input.mousePosition.x,deltaMove.y/Input.mousePosition.y);
			transform.sizeDelta = new Vector2(transform.sizeDelta.x*scale.x,transform.sizeDelta.y*scale.y);
			areaTransform.sizeDelta = new Vector2(areaTransform.sizeDelta.x*scale.x,areaTransform.sizeDelta.y*scale.y);
			deltaMove = Input.mousePosition;
		} else if (selectMove == SelectMove.BR) {
			Vector2 scale = new Vector2(Input.mousePosition.x/deltaMove.x,deltaMove.y/Input.mousePosition.y);
			transform.sizeDelta = new Vector2(transform.sizeDelta.x*scale.x,transform.sizeDelta.y*scale.y);
			areaTransform.sizeDelta = new Vector2(areaTransform.sizeDelta.x*scale.x,areaTransform.sizeDelta.y*scale.y);
			deltaMove = Input.mousePosition;
		}
	}

	public void OnPointerDownArea()
	{
		selectMove = SelectMove.Area;
	}

	public void OnPointerDownTL()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.TL;
		TL.color = selectedColor;
	}

	public void OnPointerDownTR()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.TR;
		TR.color = selectedColor;
	}

	public void OnPointerDownBR()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.BR;
		BR.color = selectedColor;
	}

	public void OnPointerDownBL()
	{
		deltaMove = Input.mousePosition;
		selectMove = SelectMove.BL;
		BL.color = selectedColor;
	}

	public void OnPointerUp(){
		selectMove = SelectMove.None;
		BL.color = normalColor;
		BR.color = normalColor;
		TL.color = normalColor;
		TR.color = normalColor;
	}

}
