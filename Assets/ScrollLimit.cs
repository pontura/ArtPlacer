using UnityEngine;
using System.Collections;

public class ScrollLimit : MonoBehaviour {

    public Vector2 limitY;
    public RectTransform container;
    private Vector3 initialPos;
    public bool horizontal;

    void Start()
    {
        initialPos = container.anchoredPosition;
    }
	void Update () {
        if (horizontal)
        {

            if (container.anchoredPosition.x > limitY.x)
                container.anchoredPosition = new Vector3(limitY.x, initialPos.y, 0);
            else if (container.anchoredPosition.x < limitY.y)
                container.anchoredPosition = new Vector3(limitY.y, initialPos.y, 0);

            return;
        }
        if (container.anchoredPosition.y > limitY.x)
            container.anchoredPosition = new Vector3(initialPos.x, limitY.x, 0);
        else if (container.anchoredPosition.y < limitY.y)
            container.anchoredPosition = new Vector3(initialPos.x, limitY.y, 0);
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
