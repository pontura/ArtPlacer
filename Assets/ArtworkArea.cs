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

	int limitTop = Screen.height;
	int limitLeft = 0;
	int limitRight = Screen.width;
	int limitBottom = 0;

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
		if (Input.mousePosition.x > 0 && Input.mousePosition.y > 0) {
			if (selectMove == SelectMove.Area) {
				Vector3 pos = Input.mousePosition-offset;
				/*if ((pos.x - transform.sizeDelta.x * 0.5f > limitLeft) && (pos.x + transform.sizeDelta.x * 0.5 < limitRight) && (pos.y - transform.sizeDelta.y * 0.5 > limitBottom) && (pos.y + transform.sizeDelta.y * 0.5 < limitTop))		gameObject.transform.position = pos;
				pos.x = pos.x - transform.sizeDelta.x * 0.5f<limitLeft?limitLeft + transform.sizeDelta.x * 0.5f:pos.x;
				pos.x=pos.x + transform.sizeDelta.x * 0.5f>limitRight?limitRight - transform.sizeDelta.x * 0.5f:pos.x;
				pos.y = pos.y - transform.sizeDelta.y * 0.5f<limitBottom?limitBottom + transform.sizeDelta.y * 0.5f:pos.y;
				pos.y = pos.y + transform.sizeDelta.y * 0.5f >limitTop?limitTop - transform.sizeDelta.y * 0.5f:pos.y;*/
				gameObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
			} else if (selectMove == SelectMove.TL) {
				Vector2 scale = new Vector2 (deltaMove.x / Input.mousePosition.x, Input.mousePosition.y / deltaMove.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				Vector2 deltaSize = new Vector2 (-1*(transform.sizeDelta.x-newSize.x),transform.sizeDelta.y-newSize.y);
				//if ((transform.position.x - newSize.x * 0.5 > limitLeft) && (transform.position.x + newSize.x * 0.5 < limitRight) && (transform.position.y - newSize.y * 0.5 > limitBottom) && (transform.position.y + newSize.y * 0.5 < limitTop)) {
					transform.sizeDelta = newSize;			
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					updatePos(transform.position, deltaSize);
					deltaMove = Input.mousePosition;
				//}
			} else if (selectMove == SelectMove.TR) {
				Vector2 scale = new Vector2 (Input.mousePosition.x / deltaMove.x, Input.mousePosition.y / deltaMove.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				Vector2 deltaSize = new Vector2 (transform.sizeDelta.x-newSize.x,transform.sizeDelta.y-newSize.y);
				//if ((transform.position.x - newSize.x * 0.5 > limitLeft) && (transform.position.x + newSize.x * 0.5 < limitRight) && (transform.position.y - newSize.y * 0.5 > limitBottom) && (transform.position.y + newSize.y * 0.5 < limitTop)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					updatePos(transform.position, deltaSize);
					deltaMove = Input.mousePosition;
				//}
			} else if (selectMove == SelectMove.BL) {
				Vector2 scale = new Vector2 (deltaMove.x / Input.mousePosition.x, deltaMove.y / Input.mousePosition.y);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				Vector2 deltaSize = new Vector2 (-1*(transform.sizeDelta.x-newSize.x),-1*(transform.sizeDelta.y-newSize.y));
				//if ((transform.position.x - newSize.x * 0.5 > limitLeft) && (transform.position.x + newSize.x * 0.5 < limitRight) && (transform.position.y - newSize.y * 0.5 > limitBottom) && (transform.position.y + newSize.y * 0.5 < limitTop)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					updatePos(transform.position, deltaSize);
					deltaMove = Input.mousePosition;
				//}
			} else if (selectMove == SelectMove.BR) {
				Vector2 scale = new Vector2 (Input.mousePosition.x / deltaMove.x, deltaMove.y / Input.mousePosition.y);
				//Vector2 scale = new Vector2 (1.1f,1.1f);
				Vector2 newSize = new Vector2 (transform.sizeDelta.x * scale.x, transform.sizeDelta.y * scale.y);
				Vector2 deltaSize = new Vector2 (transform.sizeDelta.x-newSize.x,-1*(transform.sizeDelta.y-newSize.y));
				//if ((transform.position.x - newSize.x * 0.5 > limitLeft) && (transform.position.x + newSize.x * 0.5 < limitRight) && (transform.position.y - newSize.y * 0.5 > limitBottom) && (transform.position.y + newSize.y * 0.5 < limitTop)) {
					transform.sizeDelta = newSize;
					areaTransform.sizeDelta = new Vector2 (areaTransform.sizeDelta.x * scale.x, areaTransform.sizeDelta.y * scale.y);
					updatePos(transform.position, deltaSize);
					deltaMove = Input.mousePosition;
				//}
			}
		}
	}

	void updatePos(Vector3 pos, Vector2 delta){
		pos.x -= delta.x * 0.375f;
		pos.y -= delta.y * 0.375f;
		gameObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
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
