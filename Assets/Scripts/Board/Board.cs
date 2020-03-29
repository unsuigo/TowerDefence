using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
  [SerializeField]
	Transform ground = default;

	[SerializeField]
	Tile tilePrefab = default;

	Vector2Int size;

	Tile[] tiles;

	List<Tile> spawnPoints = new List<Tile>();

	List<TileContent> updatingContent = new List<TileContent>();

	Queue<Tile> searchFrontier = new Queue<Tile>();

	TileContentFactory contentFactory;

	
	public int SpawnPointCount => spawnPoints.Count;

	public void Initialize (Vector2Int size, TileContentFactory contentFactory) 
	{
		this.size = size;
		this.contentFactory = contentFactory;
		ground.localScale = new Vector3(size.x, size.y, 1f);

		Vector2 offset = new Vector2(
			(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		);
		tiles = new Tile[size.x * size.y];
		for (int i = 0, y = 0; y < size.y; y++) {
			for (int x = 0; x < size.x; x++, i++) {
				Tile tile = tiles[i] = Instantiate(tilePrefab);
				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(
					x - offset.x, 0f, y - offset.y
				);

				if (x > 0) {
					Tile.MakeEastWestNeighbors(tile, tiles[i - 1]);
				}
				if (y > 0) {
					Tile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
				}

				tile.IsAlternative = (x & 1) == 0;
				if ((y & 1) == 0) {
					tile.IsAlternative = !tile.IsAlternative;
				}

				tile.Content = contentFactory.Get(TileContentType.Empty);
			}
		}

		ToggleDestination(tiles[tiles.Length / 2]);
		ToggleSpawnPoint(tiles[0]);
	}

	public void GameUpdate () {
		for (int i = 0; i < updatingContent.Count; i++) {
			updatingContent[i].GameUpdate();
		}
	}

	public void ToggleDestination (Tile tile) {
		if (tile.Content.TileType == TileContentType.Destination) {
			tile.Content = contentFactory.Get(TileContentType.Empty);
			if (!FindPaths()) {
				tile.Content =
					contentFactory.Get(TileContentType.Destination);
				FindPaths();
			}
		}
		else if (tile.Content.TileType == TileContentType.Empty) {
			tile.Content = contentFactory.Get(TileContentType.Destination);
			FindPaths();
		}
	}
	
	public void ToggleWall (Tile tile) {
		if (tile.Content.TileType == TileContentType.Wall) {
			tile.Content = contentFactory.Get(TileContentType.Empty);
			FindPaths();
		}
		else if (tile.Content.TileType == TileContentType.Empty) {
			tile.Content = contentFactory.Get(TileContentType.Wall);
			if (!FindPaths()) {
				tile.Content = contentFactory.Get(TileContentType.Empty);
				FindPaths();
			}
		}
	}
	
	public void ToggleSpawnPoint (Tile tile) {
		if (tile.Content.TileType == TileContentType.SpawnPoint) {
			if (spawnPoints.Count > 1) {
				spawnPoints.Remove(tile);
				tile.Content = contentFactory.Get(TileContentType.Empty);
			}
		}
		else if (tile.Content.TileType == TileContentType.Empty) {
			tile.Content = contentFactory.Get(TileContentType.SpawnPoint);
			spawnPoints.Add(tile);
		}
	}
	
	
	public void ToggleTower (Tile tile, TowerType towerType) {
		if (tile.Content.TileType == TileContentType.Tower) {
			updatingContent.Remove(tile.Content);
			if (((Tower)tile.Content).TowerType == towerType) {
				tile.Content = contentFactory.Get(TileContentType.Empty);
				FindPaths();
			}
			else {
				tile.Content = contentFactory.Get(towerType);
				updatingContent.Add(tile.Content);
			}
		}
		else if (tile.Content.TileType == TileContentType.Empty) {
			tile.Content = contentFactory.Get(towerType);
			if (FindPaths()) {
				updatingContent.Add(tile.Content);
			}
			else {
				tile.Content = contentFactory.Get(TileContentType.Empty);
				FindPaths();
			}
		}
		else if (tile.Content.TileType == TileContentType.Wall) {
			tile.Content = contentFactory.Get(towerType);
			updatingContent.Add(tile.Content);
		}
	}

	public Tile GetSpawnPoint (int index) {
		return spawnPoints[index];
	}

	public Tile GetTile (Ray ray) {
		if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1)) {
			int x = (int)(hit.point.x + size.x * 0.5f);
			int y = (int)(hit.point.z + size.y * 0.5f);
			if (x >= 0 && x < size.x && y >= 0 && y < size.y) {
				return tiles[x + y * size.x];
			}
		}
		return null;
	}

	bool FindPaths () {
		foreach (Tile tile in tiles) {
			if (tile.Content.TileType == TileContentType.Destination) {
				tile.BecomeDestination();
				searchFrontier.Enqueue(tile);
			}
			else {
				tile.ClearPath();
			}
		}
		if (searchFrontier.Count == 0) {
			return false;
		}

		while (searchFrontier.Count > 0) {
			Tile tile = searchFrontier.Dequeue();
			if (tile != null) {
				if (tile.IsAlternative) {
					searchFrontier.Enqueue(tile.GrowPathNorth());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathWest());
				}
				else {
					searchFrontier.Enqueue(tile.GrowPathWest());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathNorth());
				}
			}
		}

		foreach (Tile tile in tiles) {
			if (!tile.HasPath) {
				return false;
			}
		}

		return true;
	}
}
