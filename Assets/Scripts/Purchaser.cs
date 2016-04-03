using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour, IStoreListener {

	private static IStoreController m_StoreController;         	// Reference to the Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;

	private static string kProductIDConsumable 			= "consumable";
	private static string bigHintPack_consumeable 	= "com.huusmann.hottiesworld.largehintpack20hints"; // Apple App Store identifier for the consumable product.
	private static string smallHintPack_consumeable = "com.huusmann.hottiesworld.smallhintpack5hints";

	// TODO : need two hint packages for google play store

	// Use this for initialization
	void Start () {
		// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null){
		 // Begin to configure our connection to Purchasing
		 initializePurchasing();
		}
	}

	private void initializePurchasing(){
		// If we have already connected to Purchasing ...
		if (isInitialized()){
		  // ... we are done here.
		  return;
		}

		// Create a builder, first passing in a suite of Unity provided stores.
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
		builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs(){
			{bigHintPack_consumeable, 	AppleAppStore.Name },
			{smallHintPack_consumeable, AppleAppStore.Name },
		}); // Continue adding the non-consumable product.

		UnityPurchasing.Initialize(this, builder);
	}

	private bool isInitialized(){
    // Only say we are initialized if both the Purchasing references are set.
    return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void buyConsumable(){
    // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
    buyProductID(kProductIDConsumable);
	}

	void buyProductID(string productId){
		try{
			// If Purchasing has been initialized ...
			if (isInitialized()){
				// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
				Product product = m_StoreController.products.WithID(productId);
				// If the look up found a product for this device's store and that product is ready to be sold ...
				if (product != null && product.availableToPurchase){
					Debug.Log (string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
					m_StoreController.InitiatePurchase(product);
				} // Otherwise ...
				else{
					// ... report the product look-up failure situation
					Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			// Otherwise ...
			else{
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}catch (Exception e) { // Complete the unexpected exception handling ...
			 // ... by reporting any unexpected exception for later diagnosis.
			 Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
		 }
	 }

	// IStoreListener
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions){

	}

	public void OnInitializeFailed(InitializationFailureReason error){

	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason){

	}

}
