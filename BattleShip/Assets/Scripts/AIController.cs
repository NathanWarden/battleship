using System.Collections.Generic;
using UnityEngine;

public class AIController : BoardController
{
	private float timer;
	private List<Grid> untestedGrids = new();


	public override void Setup(BoardData thisBoard, BoardData opponentBoard)
	{
		base.Setup(thisBoard, opponentBoard);
		untestedGrids.AddRange(opponentBoard.GetBoardGrids());
	}


	void Update()
	{
		if (!MyMove) return;

		if (timer < 0)
		{
			timer += .1f;

			if (untestedGrids.Count == 0) return;

			var grid = untestedGrids[0];
			untestedGrids.RemoveAt(0);

			MakeMove(grid);

			MyMove = false;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
}