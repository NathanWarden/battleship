using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BoardController
{
	private float timer;
	private Dictionary<Vector2Int, Grid> untestedGrids = new();

	private Camera mainCam;
	private Grid prevGrid;


	protected override void Awake()
	{
		mainCam = Camera.main;
	}


	public override void Setup(BoardData thisBoard, BoardData opponentBoard)
	{
		base.Setup(thisBoard, opponentBoard);

		foreach (var grid in opponentBoard.GetBoardGrids())
		{
			untestedGrids.Add(grid.Coords, grid);
		}
	}


	private RaycastHit[] hits = new RaycastHit[10];

	void Update()
	{
		if (!MyMove) return;

		if (prevGrid)
		{
			prevGrid.SetPegActive(false);
		}

		var ray = mainCam.ScreenPointToRay(Input.mousePosition + Vector3.forward * 10);
		var hitCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity);
		var camPos = mainCam.transform.position;
		var closestHit = Mathf.Infinity;
		int hitIndex = -1;

		for (int i = 0; i < hitCount; i++)
		{
			if (hits[i].transform.tag.Equals("Grid"))
			{
				var dist = (camPos - hits[i].point).sqrMagnitude;

				if (dist < closestHit)
				{
					hitIndex = i;
					closestHit = dist;
				}
			}
		}

		if (hitIndex == -1) return;

		var grid = hits[hitIndex].transform.GetComponent<Grid>();
		if (!untestedGrids.ContainsKey(grid.Coords)) return;

		grid.SetPegActive(true);
		prevGrid = grid;

		if (!Input.GetMouseButtonDown(0)) return;

		untestedGrids.Remove(grid.Coords);
		grid.gameObject.SetActive(true);
		grid.SetPegActive(true);
		MakeMove(grid.Coords);
		MyMove = false;
		prevGrid = null;
	}
}