using UnityEngine;
using System.Collections;

public class LoadingAsset : MonoBehaviour {

    private RectTransform tr;
    public GameObject asset;

    void Start()
    {
        Events.OnLoading += OnLoading;
        tr = GetComponent<RectTransform>();
    }
    void OnDestroy()
    {
        Events.OnLoading -= OnLoading;
    }
    void Update()
    {
        if (asset.activeSelf)
            tr.localEulerAngles = new Vector3(0, 0, tr.localEulerAngles.z + 2);
    }
    void OnLoading(bool enable)
    {
        if (enable)
            SetOn();
        else
            SetOff();
    }
    public void SetOn()
    {
        asset.SetActive(true);
    }
    public void SetOff()
    {
        asset.SetActive(false);
    }
}
