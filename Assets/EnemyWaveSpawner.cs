using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWaveSpawner : MonoBehaviour
{
	[Header("Enemy Setup")]
	public GameObject enemyPrefab;
	public int enemiesPerWave = 5;

	[Header("Win Requirement")]
	public int enemiesNeededToWin = 50;
	public GameObject winScreen;
	public float winScreenTime = 3f;

	[Header("Main Menu")]
	public string mainMenuSceneName = "MainMenu";

	[Header("Wave Settings")]
	public float timeBetweenWaves = 2f;
	public bool startOnAwake = true;

	[Header("Spawn Area")]
	public float spawnRadius = 10f;
	public float spawnHeightOffset = 1f;

	[Header("Optional")]
	public bool showSpawnRadius = true;

	private readonly List<GameObject> aliveEnemies = new List<GameObject>();

	private int enemiesDefeated;
	private bool spawningWave;
	private bool gameWon;

	private void Start()
	{
		if (winScreen != null)
		{
			winScreen.SetActive(false);
		}

		if (startOnAwake)
		{
			StartCoroutine(WaveLoop());
		}
	}

	private IEnumerator WaveLoop()
	{
		while (!gameWon)
		{
			yield return StartCoroutine(SpawnWave());

			yield return new WaitUntil(() => AllEnemiesDefeatedOrGameWon());

			if (!gameWon)
			{
				yield return new WaitForSeconds(timeBetweenWaves);
			}
		}
	}

	private IEnumerator SpawnWave()
	{
		spawningWave = true;
		aliveEnemies.Clear();

		int enemiesLeftToSpawn = enemiesNeededToWin - enemiesDefeated;
		int amountToSpawn = Mathf.Min(enemiesPerWave, enemiesLeftToSpawn);

		for (int i = 0; i < amountToSpawn; i++)
		{
			Vector3 spawnPosition = GetRandomSpawnPosition();

			GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
			aliveEnemies.Add(enemy);

			yield return null;
		}

		spawningWave = false;
	}

	private Vector3 GetRandomSpawnPosition()
	{
		Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

		Vector3 spawnPosition = transform.position + new Vector3(
			randomCircle.x,
			spawnHeightOffset,
			randomCircle.y
		);

		return spawnPosition;
	}

	private bool AllEnemiesDefeatedOrGameWon()
	{
		CountDestroyedEnemies();

		if (enemiesDefeated >= enemiesNeededToWin && !gameWon)
		{
			StartCoroutine(WinGame());
			return true;
		}

		return aliveEnemies.Count == 0 && !spawningWave;
	}

	private void CountDestroyedEnemies()
	{
		for (int i = aliveEnemies.Count - 1; i >= 0; i--)
		{
			if (aliveEnemies[i] == null)
			{
				aliveEnemies.RemoveAt(i);
				enemiesDefeated++;
			}
		}
	}

	private IEnumerator WinGame()
	{
		gameWon = true;

		if (winScreen != null)
		{
			winScreen.SetActive(true);
		}

		yield return new WaitForSeconds(winScreenTime);

		SceneManager.LoadScene(mainMenuSceneName);
	}

	private void OnDrawGizmosSelected()
	{
		if (!showSpawnRadius) return;

		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}