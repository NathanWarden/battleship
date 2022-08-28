using UnityEngine;

public class Grid : MonoBehaviour
{
	[SerializeField] GameObject peg;
	[SerializeField] MeshRenderer pegRenderer;
	[SerializeField] Material hitMaterial;

	public Vector2Int Coords { get; set; }
	public int X => Coords.x;
	public int Y => Coords.y;

	public bool Used { get; set; }
	public bool Hit { get; set; }
	public Ship Ship { get; set; }


	public void SetPegActive(bool active)
	{
		peg.SetActive(active);
	}


	public void SetToHitColor(bool hit, bool setActive = true)
	{
		if (hit) pegRenderer.sharedMaterial = hitMaterial;
		gameObject.SetActive(setActive);
	}
}
