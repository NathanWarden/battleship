using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
	[SerializeField] private bool isAI;
	[SerializeField] GameController gameController;
	[SerializeField] BoardData boardData;
	int boardSize => boardData.BoardSize;

	[SerializeField] Transform shipGraphics;
	private Transform shipBoard;
	private Grid[] boardGrids;
	[SerializeField] Transform trackerGraphics;
	private Transform trackerBoard;
	private Grid[] trackerGrids;

	[SerializeField] GameObject gridSource;
	[SerializeField] GameObject textSource;

	[SerializeField] Ship[] shipPrefabs;
	private GameObject[] ships;
	private int currentShipIndex;
	private Ship currentShip;

	private Camera mainCam;
	private int rotation;


	void Awake()
	{
		mainCam = Camera.main;
		shipBoard = shipGraphics.parent;
		trackerBoard = trackerGraphics.parent;
		SetupGraphics();

		currentShip = ships[0].GetComponent<Ship>();
		shipBoard.GetChild(0).gameObject.SetActive(true);

		if (isAI)
		{
			canPlaceShip = true;
			RandomlyPlaceShips();
		}
	}


	void StartGame()
	{
		boardData.HideAllGrids();
		trackerBoard.GetChild(0).gameObject.SetActive(true);

		enabled = false;

		if (!isAI)
		{
			foreach (var grid in trackerGrids)
			{
				grid.gameObject.SetActive(true);
			}
		}

		BoardController controller = isAI ?
				gameObject.GetComponent<AIController>()
				: gameObject.GetComponent<PlayerController>();
		gameController.SetupComplete(boardData, controller);
	}


	private RaycastHit[] hits = new RaycastHit[10];
	private bool canPlaceShip;

	void Update()
	{
		if (currentShip == null) return;

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

		if (hitIndex >= 0)
		{
			MoveCurrentShip(hits[hitIndex].transform.localPosition);
			canPlaceShip = true;
		}

		if (Input.GetMouseButtonDown(1) && currentShip.isActiveAndEnabled)
		{
			rotation = (rotation + 1) % 4;
			RotateShip();
		}

		if (Input.GetMouseButtonDown(0))
		{
			TryPlaceShip();
		}
	}


	private void MoveCurrentShip(Vector3 position)
	{
		position.y = 1;
		currentShip.transform.localPosition = position;
		currentShip.gameObject.SetActive(true);
	}


	void GetNextShip()
	{
		currentShipIndex++;
		if (currentShipIndex < ships.Length)
		{
			currentShip = ships[currentShipIndex].GetComponent<Ship>();
			RotateShip();
		}
		else
		{
			currentShip = null;
			StartGame();
		}
	}


	private void TryPlaceShip()
	{
		if (!canPlaceShip) return;
		var results = currentShip.CheckPlacement();
		if (!results.validPlacement) return;

		foreach (var grid in results.placementGrids)
		{
			grid.SetToHitColor(true, false);
			grid.Used = true;
			grid.Ship = currentShip;
		}

		canPlaceShip = isAI;
		GetNextShip();
	}


	void RotateShip()
	{
		currentShip.transform.localEulerAngles = Vector3.up * (rotation * 90);
	}


	void SetupGraphics()
	{
		int size = boardSize+1;
		var scale = new Vector3(size, 1, size);

		shipGraphics.localScale = scale;
		trackerGraphics.localScale = scale;

		boardGrids = SetupGrid(shipBoard);
		trackerGrids = SetupGrid(trackerBoard);
		gridSource.SetActive(false);
		boardData.SetGrids(boardGrids, trackerGrids);
		textSource.SetActive(false);

		List<Ship> shipList = new List<Ship>();
		ships = new GameObject[shipPrefabs.Length];
		for (int i = 0; i < shipPrefabs.Length; i++)
		{
			ships[i] = Instantiate(shipPrefabs[i].gameObject, transform);
			shipList.Add(ships[i].GetComponent<Ship>());
			ships[i].SetActive(false);
		}
		boardData.SetShips(shipList.ToArray());
	}


	Grid[] SetupGrid(Transform parent)
	{
		var gridSquaresParent = new GameObject("GridSquares");
		var gridSquaresTfm = gridSquaresParent.transform;
		List<Grid> grids = new List<Grid>();

		gridSquaresTfm.SetParent(parent, false);
		gridSquaresTfm.SetSiblingIndex(0);
		gridSquaresParent.SetActive(false);

		for (int y = 0; y < boardSize; y++)
		{
			var newLetter = Instantiate(textSource, parent);
			newLetter.transform.localPosition = new Vector3(-boardSize/2f, 0, boardSize/2f - 1 - y);
			var tmp = newLetter.GetComponentInChildren<TextMeshPro>();
			tmp.text = ((char)('A'+y)).ToString();

			var newNumber = Instantiate(textSource, parent);
			newNumber.transform.localPosition = new Vector3(-boardSize/2f + 1 + y, 0, boardSize/2f);
			tmp = newNumber.GetComponentInChildren<TextMeshPro>();
			tmp.text = (y+1).ToString();

			for (int x = 0; x < boardSize; x++)
			{
				var newGrid = Instantiate(gridSource, gridSquaresTfm);
				var grid = newGrid.GetComponent<Grid>();
				newGrid.transform.localPosition = new Vector3(-boardSize/2f + 1 + x, 0, boardSize/2f - 1 - y);
				newGrid.name = $"{x},{y}";
				grid.Coords = new Vector2Int(x, y);
				grid.SetPegActive(isAI);
				grids.Add(grid);
			}
		}

		return grids.ToArray();
	}


	void RandomlyPlaceShips()
	{
		void AttemptPlacement(int x, int y)
		{
			rotation = Random.Range(0, 4);
			MoveCurrentShip(new Vector3(x, 1, y));
			TryPlaceShip();
		}

		int hBoardSize = boardSize / 2;

		// Back left
		AttemptPlacement(Random.Range(-hBoardSize+1, 0), Random.Range(-hBoardSize+1, 0));
		// Back right
		AttemptPlacement(Random.Range(0, hBoardSize), Random.Range(-hBoardSize+1, 0));
		// Front left
		AttemptPlacement(Random.Range(-hBoardSize+1, 0), Random.Range(0, hBoardSize));
		// Front right
		AttemptPlacement(Random.Range(0, hBoardSize), Random.Range(0, hBoardSize));

		int maxTries = 1000;
		while (currentShip != null)
		{
			AttemptPlacement(Random.Range(-hBoardSize+1, hBoardSize), Random.Range(-hBoardSize+1, hBoardSize));
			maxTries--;
			if (maxTries < 0)
			{
				Debug.LogError("Max tries reached!!!");
				break;
			}
		}
	}
}
