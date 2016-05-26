using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using Soomla.Store.Example;

public class FullVersion : MonoBehaviour {

    public Text desc;
 
    void Start()
    {
        desc.text = StoreData.Instance.GetComponent<StoreSettings>().msg_general;
        Data.Instance.SetTitle("");
        Data.Instance.SetMainMenuActive(true);
        Events.Back += Back;
        Events.HelpHide();
        StoreEvents.OnMarketPurchase += onMarketPurchase;
    }
    void OnDestroy()
    {
        StoreEvents.OnMarketPurchase -= onMarketPurchase;
        Events.Back -= Back;
    }
    public void Discard()
    {
        Back();
    }
    void Back()
    {
        Data.Instance.Back();
    }
    public void GetIt()
    {
        Debug.Log("Get the full Version");
        Events.OnLoading(true);
        Invoke("OnDelay", 5);
        StoreInventory.BuyItem(StoreAssets.FULL_VERSION_PRODUCT_ID);
    }

	public void ResoreIt()
	{
		Debug.Log("Restore the full Version");
		Events.OnLoading(true);
		Invoke("OnDelay", 5);
		SoomlaStore.RestoreTransactions ();
		//StoreInventory.BuyItem(StoreAssets.FULL_VERSION_PRODUCT_ID);
	}

    void OnDelay()
    {
        Events.OnLoading(false);
    }
    void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    {
        StoreInventory.BuyItem(StoreAssets.FULL_VERSION_PRODUCT_ID);
        Invoke("GotoMainMenu", 0.1f);
    }
    void GotoMainMenu()
    {
        Data.Instance.LoadLevel("Intro");
        Events.OnLoading(false);
    }

}
