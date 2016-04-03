using UnityEngine;
using System.Collections;

public class HungryCustomer : MonoBehaviour {

	public Sprite fedSprite;
	public SpriteRenderer speechbubble;
	public TextMesh textMesh;
	public string text;

	// Use this for initialization
	void Start () {
		textMesh.text = resolveTextSize(text);
	}

	// Update is called once per frame
	void Update () {

	}

	public void feed(){
		GetComponent<SpriteRenderer>().sprite = fedSprite;
		gameObject.tag = "FED";
		textMesh.GetComponent<MeshRenderer>().enabled = true;

		speechbubble.enabled = true;
		Invoke("startFadeOutSpeechbubble", 5.0f);
	}

	private void startFadeOutSpeechbubble(){
		StartCoroutine(fadeOutSpeechbubble());
	}

	private IEnumerator fadeOutSpeechbubble(){
		speechbubble.color = new Color(1f, 1f, 1f, 0.9f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.8f);
		yield return new WaitForSeconds(.01f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.7f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.6f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.5f);
		textMesh.GetComponent<MeshRenderer>().enabled = false;
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.4f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.3f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.2f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0.1f);
		yield return new WaitForSeconds(.05f);
		speechbubble.color = new Color(1f, 1f, 1f, 0);
	}

	// Wrap text by line height
	private string resolveTextSize(string input){
		// Split string by char " "
		string[] words = input.Split(" "[0]);

		// Prepare result
		string result = "";

		// Temp line string
		string line = "";

		// for each all words
		foreach(string s in words){
			// Append current word into line
			string temp = line + " " + s;

			// If line length is bigger than lineLength
			if(temp.Length > 11){ // 11 fit the speechbubble width well

				// Append current line into result
				result += line + "\n";
				// Remain word append into new line
				line = s;
			}
			// Append current word into current line
			else {
				line = temp;
			}
		}

	 // Append last line into result
	 result += line;

	 // Remove first " " char
	 return result.Substring(1, result.Length - 1);
	}
}
