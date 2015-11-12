using UnityEngine;
using System.Collections;
using System;

/*
  This class handles all the in app purchases. It utilizes the Soomla framework.

  (This class can use soomla event handler)
*/

namespace Soomla.Store{

  public class IAPManager{

    public void init(){
      StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreWasInitiated;
      SoomlaStore.Initialize(new HottieIAPAssets());
    }

    public void onSoomlaStoreWasInitiated(){
      Debug.Log("IAPManger, The store was initated. This method should be called after init method is called");
      Debug.Log("IAPManager, storeinventory: " + StoreInventory.GetItemBalance("hints_small_id"));
    }

    //Payload is some text you want returned once the purchase is complete
    public void buyItem(string item, string payload){
      try{
        SoomlaStore.BuyMarketItem(item, payload); //Should match a product id in the HottieIAPAssets virtual goods
      }catch(Exception e){
        Debug.Log("IAPManger, exception caught: " + e.Message);
      }
    }

  }
}
