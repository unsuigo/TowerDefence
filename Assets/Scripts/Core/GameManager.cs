using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	[SerializeField]
	Board board = default;

	[SerializeField]
	TileContentFactory tileContentFactory = default;
	
	[SerializeField]
	EnemyFactory enemyFactory = default;
	

	[SerializeField, Range(0.1f, 10f)]
	float spawnSpeed = 1f;

	float spawnProgress;

	TowerType selectedTowerType;

	UpdatersListManager enemies = new UpdatersListManager();
	UpdatersListManager nonEnemies = new UpdatersListManager();

	Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

	static GameManager instance;

	
	void OnEnable () 
	{
		instance = this;
	}

	void Awake () 
	{
		board.Initialize(boardSize, tileContentFactory);
	}

	void OnValidateBoardSize () 
	{
		if (boardSize.x < 5) {
			boardSize.x = 5;
		}
		if (boardSize.y < 5) {
			boardSize.y = 5;
		}
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			HandleTouch();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			HandleAlternativeTouch();
		}
		spawnProgress += spawnSpeed * Time.deltaTime;
		while (spawnProgress >= 1f) 
		{
			spawnProgress -= 1f;
			SpawnEnemy();
		}
		enemies.GameUpdate();
		Physics.SyncTransforms();
		board.GameUpdate();
		nonEnemies.GameUpdate();
	}

	void HandleAlternativeTouch () 
	{
		Tile tile = board.GetTile(TouchRay);
		if (tile != null) 
		{
			if (Input.GetKey(KeyCode.LeftShift)) 
			{
				board.ToggleDestination(tile);
			}
			else 
			{
				board.ToggleSpawnPoint(tile);
			}
		}
	}

	void HandleTouch () 
	{
		Tile tile = board.GetTile(TouchRay);
		if (tile != null) 
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				board.ToggleTower(tile, selectedTowerType);
			}
			else 
			{
				board.ToggleWall(tile);
			}
		}
	}

	void SpawnEnemy () 
	{
		Tile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
		Enemy enemy = enemyFactory.Get();
		enemy.SpawnOn(spawnPoint);
		enemies.Add(enemy);
	}
}
