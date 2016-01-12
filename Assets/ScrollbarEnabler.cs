using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollbarEnabler : MonoBehaviour
{
    public RectTransform container;        // my Scroll Rect RectTransform
    public RectTransform contents;        // my Scroll Panel RectTransform
    public Scrollbar scrollbar;        // my Scroll Panel RectTransform

    void Update()
    {
        if ((contents.offsetMax.y - contents.offsetMin.y) > (container.offsetMax.y - container.offsetMin.y))
        {
            scrollbar.gameObject.SetActive(true);
        }
        else
        {
            scrollbar.gameObject.SetActive(false);
        }
    }
}