using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ScrollRectSnap : MonoBehaviour {
    GridLayoutGroup grid;
    RectTransform rect;
    ScrollRect scrollRect;
    Vector2 targetPos;
    bool done = false;
    float t = 0;
    void Start() {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
        scrollRect = GetComponentInParent<ScrollRect>();
        // auto adjust the width of the grid to have space for all the childs
        rect.sizeDelta = new Vector2((transform.childCount + 2f) * grid.cellSize.x + (transform.childCount - 1f) * grid.spacing.x, rect.sizeDelta.y);
    }
    public void Update() {
        t = Time.deltaTime * 15f;
        if (t > 1f) {
            t = 1f;
        }
        if (Mathf.Abs(scrollRect.velocity.x) > 800f && !done) {
            touchUp();
        }
        if (!done && Mathf.Abs(scrollRect.velocity.x) < 800f) {
            rect.localPosition = Vector2.Lerp(rect.localPosition, targetPos, t);
            if (Vector3.Distance(rect.localPosition, targetPos) < 0.001f) {
                rect.localPosition = targetPos;
                done = true;
            }
        }
        Vector2 tempPos = new Vector2(Mathf.Round(rect.localPosition.x / (grid.cellSize.x + grid.spacing.x)) * (grid.cellSize.x + grid.spacing.x) * -1f, 0);
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.localPosition.x == tempPos.x) {
                // do what you want with the child
                child.localScale = Vector3.Lerp(child.localScale, new Vector3(1.5f, 1.5f, 1f), t);
            }
            else {
                child.localScale = Vector3.Lerp(child.localScale, new Vector3(1f, 1f, 1f), t);
            }
        }
    }
    public void touchDown() {
        done = true;
    }
    public void touchUp() {
        float newX = Mathf.Round(rect.localPosition.x / (grid.cellSize.x + grid.spacing.x)) * (grid.cellSize.x + grid.spacing.x);
        newX = Mathf.Sign(newX) * Mathf.Min(Mathf.Abs(newX), (rect.rect.width - scrollRect.GetComponent<RectTransform>().rect.width) / 2f);
        targetPos = new Vector2(newX, 0);
        done = false;
    }
}