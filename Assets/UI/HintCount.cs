using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HintCount : MonoBehaviour {

    private int hintCount = -1;


	void Update () {
        // if (hintCount != Global.instance.iapManager.getHints()) {
        //     hintCount = Global.instance.iapManager.getHints();
        //     updateHintCountText();
        // }
	}

    private void updateHintCountText() {
        Text hintCountText = GetComponent<Text>();
        hintCountText.text = hintCount.ToString();
    }
}
