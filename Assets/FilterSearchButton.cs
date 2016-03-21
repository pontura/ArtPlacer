using UnityEngine;
using System.Collections;

public class FilterSearchButton : MonoBehaviour {

    public void GotoSearch()
    {
        Data.Instance.LoadLevel("Filters");
    }
}
