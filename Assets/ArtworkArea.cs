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
		if (Input.mousePosition.x > 0 && Input.mousePosition.y > 0) {
			if (selectMove == SelectMove.Area) {
				if ((Input.mousePosition.x - transform.sizeDelta.x * 0.5 > 0) && (Input.mousePosition.x + transform.sizeDelta.x * 0.5 < Screen.width) && (Input.mousePosition.y - transform.sizeDelta.y * 0.5 > 0) && (Input.mousePosition.y + transform.sizeDelta.y * 0.5 < Screen.height))
				gameObject.transform.position = Input.mousePosition;

			} else if (selectMove == SelectMove.TL) {
				Vector2 scale = new Vector2 (2*deltaMove.x / Input.mousePosition.x, Input.mousePosition.y / deltaMove.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				if ((transform.position.x - newSize.x * 0.5 > 0) && (transform.position.x + newSize.x * 0.5 < Screen.width) && (transform.position.y - newSize.y * 0.5 > 0) && (transform.position.y + newSize.y * 0.5 < Screen.height)) {
					transform.sizeDelta = newSize;			
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					deltaMove = Input.mousePosition;
				}
			} else if (selectMove == SelectMove.TR) {
				Vector2 scale = new Vector2 (Input.mousePosition.x / deltaMove.x, Input.mousePosition.y / deltaMove.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				if ((transform.position.x - newSize.x * 0.5 > 0) && (transform.position.x + newSize.x * 0.5 < Screen.width) && (transform.position.y - newSize.y * 0.5 > 0) && (transform.position.y + newSize.y * 0.5 < Screen.height)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					deltaMove = Input.mousePosition;
				}
			} else if (selectMove == SelectMove.BL) {
				Vector2 scale = new Vector2 (deltaMove.x / Input.mousePosition.x, deltaMove.y / Input.mousePosition.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				if ((transform.position.x - newSize.x * 0.5 > 0) && (transform.position.x + newSize.x * 0.5 < Screen.width) && (transform.position.y - newSize.y * 0.5 > 0) && (transform.position.y + newSize.y * 0.5 < Screen.height)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					deltaMove = Input.mousePosition;
				}
			} else if (selectMove == SelectMove.BR) {
				Vector2 scale = new Vector2 (Input.mousePosition.x / deltaMove.x, deltaMove.y / Input.mousePosition.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				if ((transform.position.x - newSize.x * 0.5 > 0) && (transform.position.x + newSize.x * 0.5 < Screen.width) && (transform.position.y - newSize.y * 0.5 > 0) && (transform.position.y + newSize.y * 0.5 < Screen.height)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					deltaMove = Input.mousePosition;
				}
			}
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
		/*Debug.Log ("ScreenW: " + Screen.width + " ScreenH: " + Screen.height); 
		Debug.Log ("Pos: " + transform.position + " pos2: " + transform.rect.position); 
		Debug.Log ("xMin: " + transform.rect.xMin + " xMax: " + transform.rect.xMax); 
		Debug.Log ("yMin: " + transform.rect.yMin + " yMax: " + transform.rect.yMax);
		Debug.Log ("Pos: "+ (transform.position.x + transform.rect.xMin)+", "+(transform.position.y + transform.rect.yMax)+" width: "+transform.rect.width+" height: "+transform.rect.height);*/
	}

}
