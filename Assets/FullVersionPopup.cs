using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullVersionPopup : MonoBehaviour {

    public GameObject Panel;
    public Text title;
    public Text desc;
    public Text buttonField;
    public bool isOn;
    public Animation anim;

    void Start()
    {
        Events.OnGetFullVersion += OnGetFullVersion;
        SetOff2();
        buttonField.text = "Full Version";
    }
	void OnDestroy()
	{
		Events.OnGetFullVersion -= OnGetFullVersion;
	}

    void OnGetFullVersion(string field)
    {
        title.text = "GET THE FULL VERSION";
        isOn = true;
        Panel.SetActive(true);
        anim.Play("ContactPopupOn");
        desc.text = field;
    }
    public void SetOff()
    {
        Invoke("SetOff2", 0.12f);
        anim.Play("ContactPopupOff");
    }
    void SetOff2()
    {
        isOn = false;
        Panel.SetActive(false);
    }
    public void GetIT()
    {
        Data.Instance.LoadLevel("FullVersion");
        SetOff();
    }
}
