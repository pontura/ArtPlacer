using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonFeedback : MonoBehaviour {

	public float time;

	public Animation infoAnim;
	private Image infoImage;

	void Awake () {
		infoAnim = gameObject.GetComponent<Animation> ();
		infoImage = gameObject.GetComponentInChildren<Image>();
	}

	public IEnumerator InfoButtonFeedback(){
		infoAnim.Play ();
		infoImage.color = Data.Instance.selectedColor;
		yield return new WaitForSeconds(time);
		infoAnim.Stop ();
		infoImage.color = Color.white;
	}

}
