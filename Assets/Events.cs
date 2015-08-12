using UnityEngine;
using System.Collections;

public static class Events {

    public static System.Action<string> OnSoundFX = delegate { };
    public static System.Action<int, int> OnDropBoxSelect = delegate { };
    public static System.Action<int> OnNumWallsChanged = delegate { };
	public static System.Action SaveAreas = delegate { };
    public static System.Action OnWallEdgeSelected = delegate { };
	public static System.Action OnSelectFooterArtwork = delegate { };
    
}
