using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardData : MonoBehaviour
{
	public bool Setup { get; set; }

	[SerializeField] int boardSize = 10;
	public int BoardSize => boardSize;

	Grid[] boardGrids, trackerGrids;
	Ship[] ships;

	private Dictionary<Vector2Int, Grid> boardGridMap = new();
	private Dictionary<Vector2Int, Grid> trackerGridMap = new();


	public Grid GetTrackerGrid(Vector2Int coords)
	{
		return trackerGridMap[coords];
	}


	public Grid[] GetBoardGrids()
	{
		return boardGrids;
	}


	public void SetShips(Ship[] ships)
	{
		this.ships = ships;
	}

	public void SetGrids(Grid[] boardGrids, Grid[] trackerGrids)
	{
		this.boardGrids = boardGrids;
		this.trackerGrids = trackerGrids;

		foreach (var grid in boardGrids)
		{
			boardGridMap[grid.Coords] = grid;
		}

		foreach (var grid in trackerGrids)
		{
			trackerGridMap[grid.Coords] = grid;
		}
	}

	public void HideAllGrids()
	{
		foreach (var grid in boardGrids)
		{
			grid.gameObject.SetActive(false);
		}

		foreach (var grid in trackerGrids)
		{
			grid.gameObject.SetActive(false);
		}
	}


	public bool TestGrid(Vector2Int gridCoords)
	{
		var grid = boardGridMap[gridCoords];
		if (grid.Ship != null)
		{
			grid.Hit = true;
			return true;
		}
		return false;
	}


	public bool AllShipsSunk()
	{
		return ships.All(ship => ship.Sunk());
	}
}
