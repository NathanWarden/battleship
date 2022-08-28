using UnityEngine;

public abstract class BoardController : MonoBehaviour
{
	protected BoardData thisBoard;
	protected BoardData opponentBoard;

	public bool MyMove { get; set; }

	public virtual void Setup(BoardData thisBoard, BoardData opponentBoard)
	{
		this.thisBoard = thisBoard;
		this.opponentBoard = opponentBoard;
	}


	protected bool MakeMove(Grid grid)
	{
		var hit = opponentBoard.TestGrid(grid.Coords);
		var trackerGrid = thisBoard.GetTrackerGrid(grid.Coords);
		trackerGrid.SetHitStatus(hit);
		grid.gameObject.SetActive(hit);
		return hit;
	}


	public bool CheckForWin()
	{
		return opponentBoard.AllShipsSunk();
	}
}