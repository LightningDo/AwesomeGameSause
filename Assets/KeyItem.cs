using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyItem : MonoBehaviour, IInteractable
{
	[Header("Scene Loading")]
	public bool loadNextSceneInBuildOrder = true;
	public string specificSceneName;

	private bool collected;

	public void Interact()
	{
		Debug.Log("KEY INTERACTED!");

		if (collected) return;

		collected = true;

		if (loadNextSceneInBuildOrder)
		{
			int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
			int nextSceneIndex = currentSceneIndex + 1;

			Debug.Log("Loading scene index: " + nextSceneIndex);

			if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
			{
				SceneManager.LoadScene(nextSceneIndex);
			}
			else
			{
				Debug.LogWarning("No next scene found in Build Settings.");
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(specificSceneName))
			{
				Debug.Log("Loading scene name: " + specificSceneName);
				SceneManager.LoadScene(specificSceneName);
			}
			else
			{
				Debug.LogWarning("Specific scene name is empty.");
			}
		}
	}
}