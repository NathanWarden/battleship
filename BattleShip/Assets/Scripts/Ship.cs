using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
	[SerializeField] int shipSize;
	public int Size => shipSize;

	private Grid[] placementGrids;

	static RaycastHit[] hits = new RaycastHit[10];


	void Awake()
	{
		placementGrids = new Grid[shipSize];
	}


	public (bool validPlacement, Grid[] placementGrids) CheckPlacement()
	{
		for (int i = 0; i < shipSize; i++)
		{
			Debug.DrawRay(transform.position + transform.right * i, Vector3.down, Color.red, 5);
			var hitCount = Physics.RaycastNonAlloc(transform.position + transform.right * i, Vector3.down, hits, Mathf.Infinity);
			bool validHit = false;

			for (int j = 0; j < hitCount; j++)
			{
				if (!hits[j].transform.tag.Equals("Grid")) continue;

				var grid = hits[j].transform.GetComponent<Grid>();
				if (grid == null) continue;

				placementGrids[i] = grid;
				validHit = !grid.Used;
				break;
			}

			if (!validHit) return (false, null);
		}

		return (true, placementGrids);
	}

	public bool Sunk()
	{
		return placementGrids.All(grid => grid.Hit);
	}
}
