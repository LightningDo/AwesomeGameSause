using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string firstLevelName = "Level1";

	public void StartGame()
	{
		SceneManager.LoadScene(firstLevelName);
	}

	public void QuitGame()
	{
		Debug.Log("Quit Game");
		Application.Quit();
	}
}