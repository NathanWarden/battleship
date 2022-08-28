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


	protected bool MakeMove(Vector2Int coords)
	{
		var hit = opponentBoard.TestGrid(coords);
		var trackerGrid = thisBoard.GetTrackerGrid(coords);
		trackerGrid.SetToHitColor(hit);
		var grid = opponentBoard.GetBoardGrid(coords);
		grid.SetPegActive(true);
		grid.gameObject.SetActive(hit);
		return hit;
	}


	public bool CheckForWin()
	{
		return opponentBoard.AllShipsSunk();
	}
}