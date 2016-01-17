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

    public void init(){
      StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreWasInitiated;
      // StoreEvents.OnMarketPurchase += onMarketPurchase;
      SoomlaStore.Initialize(new HottieIAPAssets());
    }

    public void onSoomlaStoreWasInitiated(){
      // soomlastore was initialized
    }

    public void buyItem(string item, string payload){
      try{
        SoomlaStore.BuyMarketItem(item, payload); //Should match a product id in the HottieIAPAssets virtual goods
      }catch(Exception e){
        Debug.Log("IAPManger, exception caught: " + e.Message);
      }
    }
  }
}
