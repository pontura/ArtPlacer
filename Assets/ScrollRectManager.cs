using UnityEngine;
using System.Collections;

public class ScrollRectManager : MonoBehaviour {

    public string symbol;
    public ScrollContent scrollContent;
    public Transform container;
    public ScrollSnap scrollSnap;

	void Start () {

        for (int a = 0; a < scrollSnap.freeSpaces; a++)
            Add(-1);

        for (int a = 0; a < scrollSnap.totalItems; a++)
            Add(a);

        for (int a = 0; a < scrollSnap.freeSpaces; a++)
            Add(-1);
	}
    void Add(int id)
    {
        ScrollContent newScrollContent = Instantiate(scrollContent);
        newScrollContent.Init(id, symbol);
        newScrollContent.transform.SetParent(container);
    }
}
