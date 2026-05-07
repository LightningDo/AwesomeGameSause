using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenuUI;
	public string titleSceneName = "StartScene";

	private bool isPaused = false;

	void Start()
	{
		if (pauseMenuUI != null)
		{
			pauseMenuUI.SetActive(false);
		}

		Time.timeScale = 1f;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isPaused)
				ResumeGame();
			else
				PauseGame();
		}
	}

	public void PauseGame()
	{
		if (pauseMenuUI != null)
		{
			pauseMenuUI.SetActive(true);
		}

		Time.timeScale = 0f;
		isPaused = true;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void ResumeGame()
	{
		if (pauseMenuUI != null)
		{
			pauseMenuUI.SetActive(false);
		}

		Time.timeScale = 1f;
		isPaused = false;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void QuitToTitle()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(titleSceneName);
	}

	public void QuitGame()
	{
		Time.timeScale = 1f;
		Debug.Log("Quit Game");
		Application.Quit();
	}
}