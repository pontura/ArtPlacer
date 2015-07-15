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
    public void ResetScroll()
    {
        limitY.x = limitY.y;
        Vector2 rt = GetComponent<RectTransform>().anchoredPosition;
        rt.y = limitY.y;
        GetComponent<RectTransform>().anchoredPosition = rt;
    }
    public void SetMaxScroll(int maxScroll)
    {
        limitY.x = maxScroll;
    }
}
