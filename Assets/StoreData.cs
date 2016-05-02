using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store.Example;
using Soomla.Store;
																									
public class StoreData : MonoBehaviour
{
    public float secTime = 2.0f;
    public float totTime = 0.0f;

    public bool fullVersion = false;

    static StoreData mInstance = null;

    public static StoreData Instance
    {
        get
        {
            return mInstance;
        }
    }

    //Load the Scene with the cube/ setup the soomla intergration
    void Start()
    {
        if (!mInstance)
            mInstance = this;

        Application.LoadLevel("Intro");																//Load actual scene
        DontDestroyOnLoad(transform.gameObject);													//Allows this gameObject to remain during level loads, solving restart crashes
        StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreIntitialized;							//Handle the initialization of store events (calls function below - unneeded in this case)
        StoreEvents.OnMarketPurchase += onMarketPurchase;
        SoomlaStore.Initialize(new StoreAssets());												    //Intialize the store
    }

    //this is likely unnecessary, but may be required depending on how you plan on doing IAPS
    public void onSoomlaStoreIntitialized()
    {

    }

    void Update()
    {
        if (Time.timeSinceLevelLoad > totTime)
        {
            CheckIAP_PurchaseStatus();																//Check status of in app purchase (true/false if player has purchased it)
            totTime = Time.timeSinceLevelLoad + secTime;
        }
    }

    //CHECK IAP STATUS (0 = not owned, 1 = owned for GetItemBalance())
    //Check the Status of the In App Purchase (true/false if player has bought it)
    void CheckIAP_PurchaseStatus()
    {
      //  Debug.Log(StoreInventory.GetItemBalance("season2unlock"));							// Print the current status of the IAP
        if (StoreInventory.GetItemBalance(StoreAssets.FULL_VERSION_PRODUCT_ID) >= 1)
        {
            fullVersion = true;		// check if the non-consumable in app purchase has been bought or not
        }
        //if (StoreInventory.GetItemBalance(StoreAssets.SEASON_3_UNLOCK_PRODUCT_ID) >= 1)
        //{
        //    season_3_unlocked = true;		// check if the non-consumable in app purchase has been bought or not
        //}
        //if (StoreInventory.GetItemBalance(StoreAssets.SEASONS_ALL_UNLOCK_PRODUCT_ID) >= 1)
        //{
        //    season_2_unlocked = true;
        //    season_3_unlocked = true;
        //}
    }
    void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    {
        if (pvi.ID == StoreAssets.FULL_VERSION_PRODUCT_ID)
        {
            fullVersion = true;
        }
        //else if (pvi.ID == StoreAssets.SEASON_3_UNLOCK_PRODUCT_ID)
        //{
        //    season_3_unlocked = true;
        //}
        //else if (pvi.ID == StoreAssets.SEASONS_ALL_UNLOCK_PRODUCT_ID)
        //{
        //    season_2_unlocked = true;
        //    season_3_unlocked = true;
        //}
    }
    //GUI ELEMENTS
    //void ___________________OnGUI()
    //{
    //    //Button To PURCHASE ITEM
    //    if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.4f, 150, 150), "Make green?"))
    //    {
    //        try
    //        {
    //            Debug.Log("attempt to purchase");

    //            StoreInventory.BuyItem("turn_green_item_id");										// if the purchases can be completed sucessfully
    //        }
    //        catch (Exception e)
    //        {																						// if the purchase cannot be completed trigger an error message connectivity issue, IAP doesnt exist on ItunesConnect, etc...)
    //            Debug.Log("SOOMLA/UNITY" + e.Message);
    //        }
    //    }
    //    //Button to RESTORE PURCHASES
    //    if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.8f, 150, 150), "Restore\nPurchases"))
    //    {
    //        try
    //        {
    //            SoomlaStore.RestoreTransactions();													// restore purchases if possible
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log("SOOMLA/UNITY" + e.Message);												// if restoring purchases fails (connectivity issue, IAP doesnt exist on ItunesConnect, etc...)
    //        }
    //    }

    //    //Button to RESTART LEVEL (ensure it doesnt crash)
    //    if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.8f, 150, 150), "Restart"))
    //    {
    //        Application.LoadLevel(Application.loadedLevel);
    //    }
    //}
}