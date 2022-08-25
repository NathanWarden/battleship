using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardSetup : MonoBehaviour
{
	[SerializeField] int boardSize = 10;

	[SerializeField] Transform shipGraphics;
	private Transform shipBoard;
	[SerializeField] Transform trackerGraphics;
	private Transform trackerBoard;

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

		RandomlyPlaceShips();
	}


	private RaycastHit[] hits = new RaycastHit[10];

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(0);
		}

		if (currentShip == null) return;

		var ray = mainCam.ScreenPointToRay(Input.mousePosition);
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
			MoveCurrentShip(hits[hitIndex].transform.position);
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
		}
	}


	private void TryPlaceShip()
	{
		var results = currentShip.CheckPlacement();
		if (!results.validPlacement) return;

		foreach (var grid in results.placementGrids)
		{
			grid.Used = true;
		}

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

		SetupGrid(shipBoard);
		SetupGrid(trackerBoard);
		textSource.SetActive(false);

		ships = new GameObject[shipPrefabs.Length];
		for (int i = 0; i < shipPrefabs.Length; i++)
		{
			ships[i] = Instantiate(shipPrefabs[i].gameObject, transform);
			ships[i].SetActive(false);
		}
	}


	void SetupGrid(Transform parent)
	{
		var gridSquaresParent = new GameObject("GridSquares");
		var gridSquaresTfm = gridSquaresParent.transform;
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
			}
		}
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
