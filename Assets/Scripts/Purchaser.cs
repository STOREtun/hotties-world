using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class Purchaser : MonoBehaviour, IStoreListener {

	private static IStoreController 	m_StoreController;         	// Reference to the Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;

	private static string smallHintPackID = "smallHintPack";
	private static string bigHintPackID		= "bigHintPack";

	private static string bigHintPack_consumeable 	= "com.fanny_posselt.hotdogheroes.largehintpack"; // Apple App Store identifier for the consumable product.
	private static string smallHintPack_consumeable = "com.fanny_posselt.hotdogheroes.smallhintpack";

	private static string android_bigHintPack_consumeable 	= "com.fanny_posselt.hotdogheroes.largehintpack20hints";
	private static string android_smallHintPack_consumeable = "com.fanny_posselt.hotdogheroes.smallhintpack5hints";

	public Text hintNumber;

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

		print("Purchaser, initialization");

		// Create a builder, first passing in a suite of Unity provided stores.
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
		// builder.AddProduct(smallHintPackID, ProductType.Consumable, new IDs(){
		// 	{smallHintPack_consumeable, AppleAppStore.Name },
		// }); // Continue adding the non-consumable product.

		builder.AddProduct(bigHintPackID, ProductType.Consumable, new IDs(){
			{bigHintPack_consumeable, AppleAppStore.Name},
			{android_bigHintPack_consumeable,  GooglePlay.Name},
		});

		builder.AddProduct(smallHintPackID, ProductType.Consumable, new IDs(){
			{smallHintPack_consumeable, AppleAppStore.Name},
			{android_smallHintPack_consumeable,  GooglePlay.Name},
		});

		UnityPurchasing.Initialize(this, builder);
	}

	private bool isInitialized(){
    // Only say we are initialized if both the Purchasing references are set.
    return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void buyConsumable(string consumable){
    // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
    buyProductID(consumable);
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

	public void restorePurchases(){
		if (!isInitialized()){
			    // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			    Debug.Log("RestorePurchases FAIL. Not initialized.");
			    return;
			}

			// If we are running on an Apple device ...
			if (Application.platform == RuntimePlatform.IPhonePlayer ||
			    Application.platform == RuntimePlatform.OSXPlayer){
			    // ... begin restoring purchases
			    Debug.Log("RestorePurchases started ...");

			    // Fetch the Apple store-specific subsystem.
			    var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			    // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			    apple.RestoreTransactions((result) => {
			        // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
			        Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			    });
			}
			// Otherwise ...
			else{
			    // We are not running on an Apple device. No work is necessary to restore purchases.
			    Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
	}

	// IStoreListener
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions){
		// Overall Purchasing system, configured with products for this application.
    m_StoreController = controller;
    // Store specific subsystem, for accessing device-specific store features.
    m_StoreExtensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error){

	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
		if (String.Equals(args.purchasedProduct.definition.id, smallHintPackID, StringComparison.Ordinal)){
			Global.instance.updateHintcountWith(5);
		}else if(String.Equals(args.purchasedProduct.definition.id, bigHintPackID, StringComparison.Ordinal)){
			Global.instance.updateHintcountWith(20);
		}else{
			print("Purchaser, nothing was bought");
		}

		hintNumber.text = Global.instance.hintCount.ToString();

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason){

	}

}
