using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuScript : MonoBehaviour
{
	[SerializeField] TMP_InputField nameField;
	[SerializeField] TextMeshProUGUI warningMessage;
	[SerializeField] GameObject initialMenuOptions;
	[SerializeField] GameObject newGameOptions;
	[SerializeField] GameObject settingsOptions;
	[SerializeField] TextMeshProUGUI difficultyText;

	private bool nameGood;

    public void CheckCurrentName() {
		if (nameField.text.Length > 0 && nameField.text.Length < 15) {
			nameField.GetComponent<Image>().color = new Color(0.0f, 0.2f, 0.0f, 1.0f);
			nameGood = true;
			warningMessage.text = "";
		} else {
			nameField.GetComponent<Image>().color = new Color(0.2f, 0.0f, 0.0f, 1.0f);
			nameGood = false;
		}
	}

	public void FinalCheckName() {
		if (!nameGood) {
			if (nameField.text.Length == 0) {
				warningMessage.text = "You must enter your name!";
			} else if (nameField.text.Length >= 15) {
				warningMessage.text = "Your name must have less than 15 characters!";
			}
		} else {
			warningMessage.text = "";
		}
	}

	public void OpenNewGameMenu() {
		Camera.main.GetComponent<Animator>().SetBool("newGameMenu", true);
		initialMenuOptions.SetActive(false);
		newGameOptions.SetActive(true);
		UpdateDifficultyText();
	}

	public void OpenSettingsMenu() {
		initialMenuOptions.SetActive(false);
		settingsOptions.SetActive(true);
	}

	public void BackToMainMenu() {
		Camera.main.GetComponent<Animator>().SetBool("newGameMenu", false);
		newGameOptions.SetActive(false);
		settingsOptions.SetActive(false);
		initialMenuOptions.SetActive(true);
	}

	public void SwitchPostProc() {
		GameManager.instance.postProcOn = !GameManager.instance.postProcOn;
		GameObject.Find("PostProcText").GetComponent<TextMeshProUGUI>().text = GameManager.instance.postProcOn ? "On" : "Off";
    }

	public void StartNewGame() {
		if (nameGood) {
			GameManager.instance.playerName = nameField.text;
			int addSizeBasedOnDifficulty = Mathf.Min(GameManager.instance.difficulty, 150);
			GameManager.instance.CaveSizeX = Random.Range(15 + addSizeBasedOnDifficulty, 50 + addSizeBasedOnDifficulty);
			GameManager.instance.CaveSizeY = Random.Range(15 + addSizeBasedOnDifficulty, 50 + addSizeBasedOnDifficulty);
			GameManager.instance.Repeat = 500 + addSizeBasedOnDifficulty * 60;
			SceneManager.LoadScene(1);
		} else {
			FinalCheckName();
		}
	}

	public void DecreaseDifficulty() {
		if (GameManager.instance.difficulty > 1) GameManager.instance.difficulty -= 1;
		UpdateDifficultyText();
	}

	public void IncreaseDifficulty() {
		GameManager.instance.difficulty += 1;
		UpdateDifficultyText();
	}

	private void UpdateDifficultyText() {
		difficultyText.text = "Difficulty: " + GameManager.instance.difficulty +
			"\n\n\tAfter every level, monster attributes increase." +
			"\n\tThere are several that may be modified." +
			"\n\tOne attribute is randomly selected for each difficulty level, and \n\tincreased by fixed amount:" +
			"\n\t\t- Health (+3)\n\t\t- Damage (+0.2)\n\t\t- Attack Speed (+0.03)\n\t\t- Move Speed (+0.02)" +
			"\n\tMore monsters are spawned on higher difficulties." +
			"\n\n\tExperience gained per monster is increased by 2 and \n\t1 additional point per 2 difficulty levels.";
	}

	public void ExitGame() {
		print("clicked exit!");
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#else
		Application.Quit();
#endif
	}
}
