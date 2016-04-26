using UnityEngine;
using System.Collections;

using System;
using UnityEngine.UI;

public class ParentalController : MonoBehaviour {

	public GameObject numbers;
	private string resultString,
	answer;

	public Main main;

	private int firstNumber,
	secondNumber;

	public void presentParentalGate(){
		main.ui.hideBottomPopup();
		main.ui.showNotebook(UI.NotebookMode.CLOSED, false);

		firstNumber 	= UnityEngine.Random.Range(2, 10);
		secondNumber 	= UnityEngine.Random.Range(2, 10);

		string _text = "What is " + firstNumber + " x " + secondNumber + "?";
		numbers.GetComponent<Text>().text = _text;

		gameObject.SetActive(true);
	}

	void OnGUI(){
		string _answer = GUI.TextField(new Rect(Screen.width/2.5f, Screen.height/1.7f, 300, 40), answer, 25);
		answer = _answer;

		if(GUI.Button(new Rect(Screen.width/2-70, Screen.height/1.5f, 60, 40), "OK")){
			int i = 0;

			try{
				i = int.Parse(answer);
			}catch(Exception e){} // no exception handlin… it does not matter

			if(i == (firstNumber * secondNumber) ){ // passed
				gameObject.SetActive(false);
				main.ui.showNotebook(UI.NotebookMode.HELP_TAB, true);
			}
		}

		if(GUI.Button(new Rect(Screen.width/2+70, Screen.height/1.5f, 60, 40), "Cancel")){
			gameObject.SetActive(false);
			main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
		}
	}

	// Use this for initialization
	void Start () {
		answer = "Write Your Answer Here";
	}

	// Update is called once per frame
	void Update () {

	}
}
