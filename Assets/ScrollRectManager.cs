using UnityEngine;
using System.Collections;

public class ScrollRectManager : MonoBehaviour {

    public ScrollContent scrollContent;
    public Transform container;

	void Start () {
        for (int a = 0; a < 100; a++)
        {
            ScrollContent newScrollContent = Instantiate(scrollContent);
            newScrollContent.Init(a);
            newScrollContent.transform.SetParent(container);
        }
	}
}
