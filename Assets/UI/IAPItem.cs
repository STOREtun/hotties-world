using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class IAPItem : MonoBehaviour {

    public string iapIdentifierString = "EXAMPLE!com.company.100goldcoins";

	// Use this for initialization
	void OnEnable () {
        Debug.Log(this.gameObject.name + ".OnEnable()");
        string title = "?";
        string descr = "?";
        string pricetag = "?";
        IAPManager iapManager = Global.instance.iapManager;
        if (iapManager != null) {
            if (iapManager.isReady()) {
                try {
                    title = iapManager.getItemTitle(iapIdentifierString);
                    descr = iapManager.getItemDescription(iapIdentifierString);
                    pricetag = iapManager.getItemPriceString(iapIdentifierString);
                } catch (Exception e) {
                    title = descr = pricetag = "error";
                }
            }
        }

        Text[] textFields = GetComponentsInChildren<Text>();
        foreach (Text textField in textFields) {
            if (textField.name == "ItemTitleText")
                textField.text = title;
            if (textField.name == "ItemDescriptionText")
                textField.text = descr;
            if (textField.name == "ItemPriceTagText")
                textField.text = pricetag;
        }
	}
	
}
