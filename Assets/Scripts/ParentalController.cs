using UnityEngine;
using System.Collections;

using System;
using UnityEngine.UI;
using TheNextFlow.UnityPlugins;

public static class ParentalController {
	public static void PresentParentalGate(){
		Main.instance.ui.HideBottomPopup();
		Main.instance.ui.ShowNotebook(UI.NotebookMode.CLOSED, false);

		int firstNumber = UnityEngine.Random.Range (2, 10);
		int secondNumber = UnityEngine.Random.Range (2, 10);
		int correctAnswer = firstNumber * secondNumber;

		int thirdNumber = UnityEngine.Random.Range (2, 10);

		int wrongAnswer = firstNumber * thirdNumber;
		if (wrongAnswer == correctAnswer) {
			wrongAnswer -= 1; 
		}

		string question = "What is " + firstNumber + " x " + secondNumber + "?";
		string _correctAnswer = correctAnswer.ToString ();
		string _wrongAnswer = wrongAnswer.ToString();

		string[] answers = {
			_correctAnswer,
			_wrongAnswer
		};

		string firstAnswer, secondAnswer;

		int random = UnityEngine.Random.Range (0, 2);
		if (random == 0) {
			secondAnswer = answers [1];
		}else secondAnswer = answers [0];
		firstAnswer = answers [random];

		MobileNativePopups.OpenAlertDialog(
			"Ask your parents!", question,
			firstAnswer, secondAnswer, "Cancel",
			() => { // first answer was pressed
				if(int.Parse(firstAnswer) == correctAnswer){
					Main.instance.ui.ShowShop();
				}else Main.instance.ui.ShowNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
			},
			() => { // second answer was pressed
				if(int.Parse(secondAnswer) == correctAnswer){
					Main.instance.ui.ShowShop();
				}else Main.instance.ui.ShowNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
			},
			() => { Debug.Log("Cancel was pressed"); }
		);			
	}
}