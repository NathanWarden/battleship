using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : BoardController
{
	private float timer;
	private Dictionary<Vector2Int, Grid> untestedGrids = new();
	private List<Grid> hitGrids = new();
	private Dictionary<Vector2Int, Grid> testedGrids = new ();


	public override void Setup(BoardData thisBoard, BoardData opponentBoard)
	{
		base.Setup(thisBoard, opponentBoard);

		foreach (var grid in opponentBoard.GetBoardGrids())
		{
			untestedGrids.Add(grid.Coords, grid);
		}
	}


	void Update()
	{
		if (!MyMove) return;

		if (timer < 0)
		{
			timer += .1f;

			if (untestedGrids.Count == 0) return;

			var grid = TestFromLastHits();

			if (grid != null && testedGrids.ContainsKey(grid.Coords)) Debug.LogError("*******************");

			if (grid == null)
			{
				var rand = Random.Range(0, untestedGrids.Count);
				grid = untestedGrids.ElementAt(rand).Value;
			}

			untestedGrids.Remove(grid.Coords);

			if (MakeMove(grid.Coords))
			{
				// In the game the player would announce the ship was sunk, so simulate this in the AI
				//		by removing testing from existing hit grids that belong to the sunken ship
				if (grid.Ship.Sunk())
				{
					var ship = grid.Ship;
					for (int i = 0; i < hitGrids.Count; i++)
					{
						if (hitGrids[i].Ship != ship) continue;
						hitGrids.RemoveAt(i);
						i--;
					}
				}
				else
				{
					hitGrids.Add(grid);
				}
			}
			testedGrids.Add(grid.Coords, grid);

			MyMove = false;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}


	readonly Vector2Int[] _directions =
	{
		Vector2Int.left,
		Vector2Int.up,
		Vector2Int.right,
		Vector2Int.down
	};

	Grid TestFromLastHits()
	{
		while (hitGrids.Count > 0)
		{
			Grid GetGridFromCoord(Vector2Int coord)
			{
				if (testedGrids.ContainsKey(coord)) return null;
				return untestedGrids.TryGetValue(coord, out var grid) ? grid : null;
			}

			int lastHit = hitGrids.Count - 1;

			// Ships are lines, so check for a line first
			if (hitGrids.Count >= 2)
			{
				int secondTolastHit = hitGrids.Count - 2;
				var forwardDirection = hitGrids[lastHit].Coords - hitGrids[secondTolastHit].Coords;
				forwardDirection.Clamp(-Vector2Int.one, Vector2Int.one * 1);
				var reverseDirection = -forwardDirection;

				for (int i = 1; i < 5; i++)
				{
					var forwardCoord = hitGrids[lastHit].Coords + forwardDirection * i;
					var reverseCoord = hitGrids[lastHit].Coords + reverseDirection * i;
					Grid grid;

					if (testedGrids.ContainsKey(forwardCoord) && !testedGrids[forwardCoord].Hit)
					{
						forwardDirection = Vector2Int.zero;
					}
					else
					{
						grid = GetGridFromCoord(forwardCoord);
						if (grid != null) return grid;
					}

					if (testedGrids.ContainsKey(reverseCoord) && !testedGrids[reverseCoord].Hit)
					{
						reverseDirection = Vector2Int.zero;
					}
					else
					{
						grid = GetGridFromCoord(reverseCoord);
						if (grid != null) return grid;
					}
				}
			}

			// Check in any direction next
			for (int i = 0; i < _directions.Length; i++)
			{
				var grid = GetGridFromCoord(hitGrids[lastHit].Coords + _directions[i]);
				if (grid != null) return grid;
			}
			hitGrids.RemoveAt(lastHit);
		}

		return null;
	}
}