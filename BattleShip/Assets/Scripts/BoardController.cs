using TMPro;
using UnityEngine;

public abstract class BoardController : MonoBehaviour
{
	protected BoardData thisBoard;
	protected BoardData opponentBoard;
	[SerializeField] private TextMeshProUGUI shipSunkMessage;

	public bool MyMove { get; set; }


	protected virtual void Awake()
	{
		shipSunkMessage.text = "";
	}


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

		if (hit && grid.Ship.Sunk())
		{
			shipSunkMessage.text = name + " sunk a ship!";
		}
		else
		{
			shipSunkMessage.text = "";
		}

		Debug.Log(name + " made move " + coords);

		return hit;
	}


	public bool CheckForWin()
	{
		return opponentBoard.AllShipsSunk();
	}
}