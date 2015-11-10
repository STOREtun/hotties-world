using UnityEngine;
using System.Collections;
using Unibill;
using System;

public class IAPManager {

    //Unibiller.CreditBalance means add to balance, DebitBalance means subtract from 
    //balance (only for manually adjusting, all purchases are auto managed).


    private readonly int START_HINT_AMOUNT = 3;

    public void init() {
        if (UnityEngine.Resources.Load("unibillInventory.json") == null) {
            Debug.LogError("You must define your purchasable inventory within the inventory editor!");
            //this.gameObject.SetActive(false);
            return;
        }

        // We must first hook up listeners to Unibill's events.
        Unibiller.onBillerReady += onBillerReady;
        Unibiller.onTransactionsRestored += onTransactionsRestored;
        Unibiller.onPurchaseCancelled += onCancelled;
        Unibiller.onPurchaseFailed += onFailed;
        Unibiller.onPurchaseCompleteEvent += onPurchased;
        Unibiller.onPurchaseDeferred += onDeferred;
        
        // Now we're ready to initialise Unibill.
        Unibiller.Initialise();

    }

    public bool isReady() {
        return Unibiller.Initialised;
    }

    public int getHints() {
        try {
            return (int)Unibiller.GetCurrencyBalance("hint");
        } catch (Exception e) {
        }
        return 0;
    }

    public void consumeHint() {
        if (getHints() > 0) {
            Unibiller.DebitBalance("hint", 1);
        }
    }

    public void initiatePurchase(string iapID) {
        PurchasableItem purchaseItem = Unibiller.GetPurchasableItemById(iapID);
        Unibiller.initiatePurchase(purchaseItem);
    }

    public string getItemTitle(string iapID) {
        PurchasableItem item = Unibiller.GetPurchasableItemById(iapID);
        string str = item.localizedTitle;
        return str == null ? "" : str;
    }

    public string getItemDescription(string iapID) {
        PurchasableItem item = Unibiller.GetPurchasableItemById(iapID);
        string str = item.localizedDescription;
        return str == null ? "" : str;
    }

    public string getItemIsoCurrencySymbol(string iapID) {
        PurchasableItem item = Unibiller.GetPurchasableItemById(iapID);
        string str = item.isoCurrencySymbol;
        return str == null ? "" : str;
    }

    public string getItemPriceString(string iapID) {
        PurchasableItem item = Unibiller.GetPurchasableItemById(iapID);
        string str = item.localizedPriceString;
        return str == null ? "" : str;
    }


    /// <summary>
    /// This will be called when Unibill has finished initialising.
    /// </summary>
    private void onBillerReady(UnibillState state) {
        UnityEngine.Debug.Log("onBillerReady:" + state + " hints=" + getHints());

        //Unibiller.DebitBalance("hint", getHints()); //reset .. for testing

        if (Global.instance.gamelaunchcount <= 0 && getHints() == 0) {
            //this is the first time game runs (as good as it gets for now)
            Debug.Log("first time running, adding " + START_HINT_AMOUNT + " hints");
            Unibiller.CreditBalance("hint", START_HINT_AMOUNT);
        }

    }

    /// <summary>
    /// This will be called after a call to Unibiller.restoreTransactions().
    /// </summary>
    private void onTransactionsRestored(bool success) {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
    private void onPurchased(PurchaseEvent e) {
        Debug.Log("Purchase OK: " + e.PurchasedItem.Id);
        Debug.Log("Receipt: " + e.Receipt);
        Debug.Log(string.Format("{0} has now been purchased {1} times.",
                                 e.PurchasedItem.name,
                                 Unibiller.GetPurchaseCount(e.PurchasedItem)));
    }

    /// <summary>
    /// This will be called if a user opts to cancel a purchase
    /// after going to the billing system's purchase menu.
    /// </summary>
    private void onCancelled(PurchasableItem item) {
        Debug.Log("Purchase cancelled: " + item.Id);
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    /// 
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void onDeferred(PurchasableItem item) {
        Debug.Log("Purchase deferred blud: " + item.Id);
    }

    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    private void onFailed(PurchasableItem item) {
        Debug.Log("Purchase failed: " + item.Id);
    }



}
