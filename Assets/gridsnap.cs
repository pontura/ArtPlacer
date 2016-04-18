using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gridsnap : MonoBehaviour {

    public int active;

    GridLayoutGroup grid;
    RectTransform rect;
    ScrollRect scrollRect;

    public int totalNums = 100;
    Vector2 targetPos;
    bool done = false;
    float t = 0;
    private float buttonHeight;    

    void Awake() {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
        scrollRect = GetComponentInParent<ScrollRect>();
        rect.localPosition = new Vector3(rect.localPosition.x, buttonHeight * 2 + (buttonHeight * active), 0);
        done = true;
        rect.sizeDelta = new Vector2(rect.sizeDelta.y, (transform.childCount + 2f) * grid.cellSize.y + (transform.childCount - 1f) * grid.spacing.y);
        buttonHeight = grid.spacing.y + grid.cellSize.y;
    }

    public void SetValue(int active)
    {
        this.active = active;
        rect.localPosition = new Vector3(rect.localPosition.x, buttonHeight * 2 + (buttonHeight*active), 0);
        done = true;
    }

    public void Update() {
        t = Time.deltaTime * 15f;
        if (t > 1f) {
            t = 1f;
        }
        if (Mathf.Abs(scrollRect.velocity.y) > 800f && !done) {
            touchUp();
        }
        if (rect.localPosition.y < (buttonHeight * 2) - 1) rect.localPosition = new Vector3(rect.localPosition.x, (buttonHeight * 2), 0);
        else if (rect.localPosition.y > (buttonHeight * (totalNums + 1)) + 1) rect.localPosition = new Vector3(rect.localPosition.x, (buttonHeight * (totalNums + 1)), 0);

        if (!done && Mathf.Abs(scrollRect.velocity.y) < 800f) {
            rect.localPosition = Vector2.Lerp(rect.localPosition, targetPos, t);
            if (Vector3.Distance(rect.localPosition, targetPos) < 0.001f) {
                rect.localPosition = targetPos;
                done = true;
            }
        }
        float newY = Mathf.Round(rect.localPosition.y / (grid.cellSize.y + grid.spacing.y)) * (grid.cellSize.y + grid.spacing.y) * -1f;
        Vector2 tempPos = new Vector2(0, newY);
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.localPosition.y == tempPos.y) {
                // do what you want with the child
                child.localScale = Vector3.Lerp( new Vector3(1.0f, 1.0f, 0f), child.localScale,  t);
                child.GetComponent<ScrollContent>().field.color = Color.white;
                this.active = child.GetComponent<ScrollContent>().id;
            }
            else {
                child.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), child.localScale, t);
                child.GetComponent<ScrollContent>().field.color = Color.grey;
            }
        }
    }

    public void touchDown() {
        done = true;
    }

    public void touchUp() {
        float newX = Mathf.Round(rect.localPosition.y / (grid.cellSize.y + grid.spacing.y)) * (grid.cellSize.y + grid.spacing.y);
        newX = Mathf.Sign(newX) * Mathf.Min(Mathf.Abs(newX), (rect.rect.height - scrollRect.GetComponent<RectTransform>().rect.height/2));
        targetPos = new Vector2(0, newX);
        done = false;
    }
}