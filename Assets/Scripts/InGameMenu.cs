using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InGameMenu : MonoBehaviour
{
	public void BackToMainMenu() {
		GameManager.instance.ResetPlayerAndMonsterValues();
		Time.timeScale = 1;
		GameManager.instance.score = 0;
		SceneManager.LoadScene(0);
	}

	public void Cancel() {
		gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	public void ExitGame() {
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#else
		Application.Quit();
#endif
	}
}