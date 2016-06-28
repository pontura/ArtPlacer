using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FocusUI : MonoBehaviour {

	public Image focusRect;
	public GameObject fade;
	private bool fading = false;

	void Start(){
		focusRect.color = new Color (0,0,0,0);
	}

	void Update(){
		if (Input.GetButton ("Fire1")) {
			//if (!fading && Input.mousePosition.y > 50 && Input.mousePosition.y < 300 && Input.mousePosition.x < 440) {
			if (!fading && Input.mousePosition.y > Screen.height*0.15f && Input.mousePosition.y < Screen.height*0.85&& Input.mousePosition.x < Screen.width*0.85) {
			//if (!fading) {
				if (focusRect != null) {
					focusRect.rectTransform.position = Input.mousePosition;
					FadeLib focusFade = Instantiate (fade).GetComponent<FadeLib>();
					focusFade.OnLoopMethod = () => {						
						float v = Mathf.Lerp(0f,1f,focusFade.time);
						focusRect.color = new Color(0,0,0,v);
					};
					focusFade.OnEndMethod = () => {
						fading = false;
						focusFade.Destroy();
					};
					focusFade.OnBeginMethod = () => {
						fading = true;
					};
					focusFade.StartFadeOut (0.5f);
				}
			}
			//Debug.Log (Input.mousePosition);
			//ScreenDebugger.Log ("pos: "+Input.mousePosition);
		}
	}

}
