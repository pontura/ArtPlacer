﻿using UnityEngine;
using System.Collections;

public class LoadRoomScreen : MonoBehaviour {

    public void TakePhoto()
    {
        Data.Instance.LoadLevel("TakePhoto");
    }
}
