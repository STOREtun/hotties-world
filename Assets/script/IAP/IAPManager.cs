using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
  This class handles all the in app purchases. It utilizes the Soomla framework.

  (This class can use soomla event handler)
*/

namespace Soomla.Store{

  public class IAPManager{

    private const string PREFIX = "com.huusmann.hottiesworld.";

    public void init(){
      StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreWasInitiated;
      StoreEvents.OnMarketPurchase += onMarketPurchase;
      SoomlaStore.Initialize(new HottieIAPAssets());
    }

    public void onSoomlaStoreWasInitiated(){
      //Debug.Log("IAPManger, The store was initated. This method should be called after init method is called");
    }

    //Payload is some text you want returned once the purchase is complete
    public void buyItem(string item, string payload){
      try{
        SoomlaStore.BuyMarketItem(item, payload); //Should match a product id in the HottieIAPAssets virtual goods
      }catch(Exception e){
        Debug.Log("IAPManger, exception caught: " + e.Message);
      }
    }

    // Handles the purchase
    public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra) {
      Debug.Log("IAPManger, payload: " + payload);
      switch(payload){
        case PREFIX+"smallhintpack5hints":
          Debug.Log("IAPManger, should add 5 hints");
          break;

        case PREFIX+"largehintpack20hints":
          Debug.Log("IAPManger, should add 20 hints");
          break;

        default:
          break;
      }

		}
  }
}
