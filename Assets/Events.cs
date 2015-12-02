using UnityEngine;
using System.Collections;

public static class Events {

    public static System.Action<string> OnSoundFX = delegate { };
    public static System.Action<int, int> OnDropBoxSelect = delegate { };
    public static System.Action<int> OnNumWallsChanged = delegate { };
	public static System.Action SaveAreas = delegate { };
    public static System.Action OnWallEdgeSelected = delegate { };
	public static System.Action OnSelectFooterArtwork = delegate { };
	public static System.Action<bool> ArtworkPreview = delegate { };
    public static System.Action<string> OnGenerateRoomThumb = delegate { };
    public static System.Action<Vector2> OnScrollSizeRefresh = delegate { };
    public static System.Action<bool> OnLoading = delegate { };
	public static System.Action<bool> OnLoadingMouse = delegate { };
    public static System.Action<string> OnTooltipOn = delegate { };
    public static System.Action OnTooltipOff = delegate { };  
	public static System.Action ConvertUnits = delegate { };
    public static System.Action Back = delegate { };

    //help:
    public static System.Action HelpHide = delegate { };
    public static System.Action HelpShow = delegate { };
    public static System.Action<bool> HelpChangeState = delegate { };
    public static System.Action<int> HelpChangeStep = delegate { };
    
    
}
