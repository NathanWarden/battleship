using UnityEngine;

public class Grid : MonoBehaviour
{
	public Vector2Int Coords { get; set; }
	public int X => Coords.x;
	public int Y => Coords.y;

	public bool Used { get; set; }
}
