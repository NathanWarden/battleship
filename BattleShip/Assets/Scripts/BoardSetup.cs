using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardSetup : MonoBehaviour
{
	[FormerlySerializedAs("gridWidth")] public int boardSize = 10;

	public Transform shipGraphics;
	public Transform trackerGraphics;

	public GameObject gridSource;
	public GameObject textSource;


	void Awake()
	{
		SetupGraphics();
	}


	void SetupGraphics()
	{
		int size = boardSize+1;
		var scale = new Vector3(size, 1, size);

		shipGraphics.localScale = scale;
		trackerGraphics.localScale = scale;

		SetupGrid(shipGraphics.parent);
		SetupGrid(trackerGraphics.parent);

		textSource.SetActive(false);
	}


	void SetupGrid(Transform parent)
	{
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
				var newGrid = Instantiate(gridSource, parent);
				newGrid.transform.localPosition = new Vector3(-boardSize/2f + 1 + x, 0, boardSize/2f - 1 - y);
				newGrid.name = $"{x},{y}";
			}
		}
	}
}
