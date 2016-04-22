using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour
{
    public GameObject container;
    public float[] points;

    ScrollRect scrollRect;
    public int active;

    public int totalItems;
    public int freeSpaces;

    public bool calculate = false;
    public bool repositionate;
    public float itemHeight;
    public float snapSpeed = 0.1f;
    

    // Use this for initialization
    bool started;
    public void Init(int activeID)
    {
        scrollRect = GetComponent<ScrollRect>();

        int qty = totalItems + (freeSpaces*2);
        points = new float[qty];
        for (int a = 0; a < qty; a++)
        {
            points[a] = 20 + (a * itemHeight);
        }

        this.active = activeID;
        container.GetComponent<RectTransform>().sizeDelta = new Vector2(container.GetComponent<RectTransform>().sizeDelta.x, qty * itemHeight + (qty - 1f) * 0);

        Repositionate(points[active + freeSpaces]);
        started = true;
    }
    public int GetActive()
    {
        return active;
    }
    public void ChangeValue(int activeID)
    {
        this.active = activeID;
        Repositionate(points[active + freeSpaces]);
    }

    void Update()
    {
        if (!started) return;
        if (calculate && Mathf.Abs(scrollRect.velocity.y) < itemHeight)
        {
            repositionate = true;
            scrollRect.inertia = false;

            int activeID = (int)(container.transform.localPosition.y / itemHeight);
            this.active = activeID - freeSpaces;

            float TargetY = points[activeID];

            float newY = Mathf.Lerp(container.transform.localPosition.y, TargetY, snapSpeed * Time.deltaTime);
            Repositionate(newY);
        }
        else
        {
            scrollRect.inertia = true;
        }
    }
    void Repositionate(float newY)
    {
        container.transform.localPosition = new Vector2(0, newY);
    }
    public void DragEnd()
    {
        calculate = true;
    }

    public void OnDrag()
    {
        calculate = false;
        repositionate = false;
    }

    int FindNearest(float f)
    {
        float distance = Mathf.Infinity;
        int output = 0;
        for (int index = 0; index < points.Length; index++)
        {
            if (Mathf.Abs(points[index] - f) < distance)
            {
                distance = Mathf.Abs(points[index] - f);
                output = index;
            }
        }
        return output;
    }
}
