using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : BoardController
{
	private float timer;
	private Dictionary<Vector2Int, Grid> untestedGrids = new();

	private Camera mainCam;


	public override void Setup(BoardData thisBoard, BoardData opponentBoard)
	{
		base.Setup(thisBoard, opponentBoard);

		foreach (var grid in opponentBoard.GetBoardGrids())
		{
			untestedGrids.Add(grid.Coords, grid);
		}
	}


	void Awake()
	{
		mainCam = Camera.main;
	}


	private RaycastHit[] hits = new RaycastHit[10];

	void Update()
	{
		if (!MyMove) return;
		if (!Input.GetMouseButtonDown(0)) return;

		var ray = mainCam.ScreenPointToRay(Input.mousePosition + Vector3.forward * 10);
		var hitCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity);
		int hitIndex = -1;

		for (int i = 0; i < hitCount; i++)
		{
			if (hits[i].transform.tag.Equals("Grid"))
			{
				hitIndex = i;
				break;
			}
		}

		if (hitIndex == -1) return;

		var grid = hits[hitIndex].transform.GetComponent<Grid>();
		if (!untestedGrids.ContainsKey(grid.Coords)) return;

		untestedGrids.Remove(grid.Coords);
		grid.gameObject.SetActive(true);
		grid.SetPegActive(true);
		MakeMove(grid.Coords);
		MyMove = false;
	}
}