using UnityEngine;
using System.Collections;

namespace Soomla.Store.Example															//Allows for access to Soomla API
{
    public class StoreAssets : IStoreAssets 											//Extend from IStoreAssets (required to define assets)
    {
        public int GetVersion()
        {														// Get Current Version
            return 1;
        }

        public VirtualCurrency[] GetCurrencies()
        {										// Get/Setup Virtual Currencies
            return new VirtualCurrency[] { };
        }

        public VirtualGood[] GetGoods()
        {												// Add "TURN_GREEN" IAP to GetGoods
            return new VirtualGood[] { FULL_VERSION };
        }

        public VirtualCurrencyPack[] GetCurrencyPacks()
        {								// Get/Setup Currency Packs
            return new VirtualCurrencyPack[] { };
        }

        public VirtualCategory[] GetCategories()
        {										// Get/ Setup Categories (for In App Purchases)
            return new VirtualCategory[] { };
        }

        //****************************BOILERPLATE ABOVE(modify as you see fit/ if nessisary)***********************
        public const string FULL_VERSION_PRODUCT_ID = "GetFullVersion";				//create a string to store the "turn green" in app purchase
        //public const string SEASON_3_UNLOCK_PRODUCT_ID = "season3unlock";
        //public const string SEASONS_ALL_UNLOCK_PRODUCT_ID = "season4unlock";

        //public const string ENERGY_1_PRODUCT_ID = "10";
        //public const string ENERGY_3_PRODUCT_ID = "25";
        //public const string ENERGY_10_PRODUCT_ID = "100";
        //public const string ENERGY_50_PRODUCT_ID = "500";


        /** Lifetime Virtual Goods (aka - lasts forever **/

        // Create the 'TURN_GREEN' LifetimeVG In-App Purchase
        public static VirtualGood FULL_VERSION = new LifetimeVG(
            "Artplacer Full",														    		// Name of IAP
            "All content unblocked",											// Description of IAP
            "GetFullVersion",													            	// Item ID (different from 'product id" used by itunes, this is used by soomla)

            // 1. assign the purchase type of the IAP (purchaseWithMarket == item cost real money),
            // 2. assign the IAP as a market item (using its ID)
            // 3. set the item to be a non-consumable purchase type

            //			1.					2.						3.
            new PurchaseWithMarket(FULL_VERSION_PRODUCT_ID, 4.99)
        );
       // public static VirtualGood SEASON_3_UNLOCK = new LifetimeVG(
       //     "Season 3 unlock",
       //     "Unlock season 3 to play 8 new levels!",
       //     "season3unlock",
       //     new PurchaseWithMarket(SEASON_3_UNLOCK_PRODUCT_ID, 0.99)
       // );
       // public static VirtualGood ALL_SEASONS_UNLOCK = new LifetimeVG(
       //    "All seasons unlock",
       //    "Unlock all seasons!",
       //    "season4unlock",
       //    new PurchaseWithMarket(SEASONS_ALL_UNLOCK_PRODUCT_ID, 24.99)
       //);
       // public static VirtualGood ENERGY_1 = new SingleUseVG(
       //     "10 Energy",
       //     "Buy 1 energy pack",
       //     "10",
       //     new PurchaseWithMarket(ENERGY_1_PRODUCT_ID, 0.99)
       // );
       // public static VirtualGood ENERGY_3 = new SingleUseVG(
       //     "25 Energy",
       //     "Buy 3 energy packs",
       //     "25",
       //     new PurchaseWithMarket(ENERGY_3_PRODUCT_ID, 1.99)
       // );
       // public static VirtualGood ENERGY_10 = new SingleUseVG(
       //     "100 Energy",
       //     "Buy 10 energy packs",
       //     "100",
       //     new PurchaseWithMarket(ENERGY_10_PRODUCT_ID, 4.99)
       // );
       // public static VirtualGood ENERGY_50 = new SingleUseVG(
       //     "500 Energy",
       //     "Buy 50 energy packs",
       //     "500",
       //     new PurchaseWithMarket(ENERGY_50_PRODUCT_ID, 9.99)
       // );
    }
}
