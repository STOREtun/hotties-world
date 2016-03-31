using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {


    public AudioSource menuClickedEffect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
     
    public void OnMenuSelected(GameObject o) {
        Debug.Log("OnMenuSelected");

        


        menuClickedEffect.Play();
    }
}
