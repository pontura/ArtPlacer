using UnityEngine;
using System.Collections;

public class ScrollLimit : MonoBehaviour {

    public Vector2 limitY;
    public GameObject container;
    public RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
	void Update () {

        if (rectTransform.anchoredPosition.y > limitY.x)
            rectTransform.anchoredPosition = new Vector3(0, limitY.x, 0);
        else if (rectTransform.anchoredPosition.y < limitY.y)
            rectTransform.anchoredPosition = new Vector3(0, limitY.y, 0);
	}
}
