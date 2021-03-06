﻿using UnityEngine;
using System.Collections;

public class LoadingAsset : MonoBehaviour {

    private RectTransform tr;
    public GameObject asset;

	bool mouseMove = false;

    void Start()
    {
        Events.OnLoading += OnLoading;
		Events.OnLoadingMouse += OnLoadingMouse;
        tr = GetComponent<RectTransform>();
        SetOff();
    }
    void OnDestroy()
    {
        Events.OnLoading -= OnLoading;
		Events.OnLoadingMouse -= OnLoadingMouse;
    }
    void Update()
    {
        if (asset.activeSelf)
            tr.localEulerAngles = new Vector3(0, 0, tr.localEulerAngles.z + 2);
		if (mouseMove) {
			asset.transform.position = Input.mousePosition;
			//background.transform.position = Input.mousePosition;
		}
    }
    void OnLoading(bool enable)
    {
        //print("OnLoading" + enable);
        if (enable)
            SetOn();
        else
            SetOff();
    }

	public void OnLoadingMouse(bool enable)
	{
        //print("OnLoadingMouse" + enable);

		if (enable)
			SetOn ();
		else 
			SetOff ();

		mouseMove=enable;
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
