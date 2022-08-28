using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	[SerializeField] private BoardData playerBoard;
	[SerializeField] private BoardController playerBoardController;
	[SerializeField] private BoardData aiBoard;
	[SerializeField] private BoardController aiBoardController;

	private bool gameStarted;
	private BoardController currentController;
	public bool waitBetweenMoves;
	private bool wait;


	public void SetupComplete(BoardData board)
	{
		board.Setup = true;

		if (playerBoard.Setup && aiBoard.Setup)
		{
			playerBoardController.Setup(playerBoard, aiBoard);
			aiBoardController.Setup(aiBoard, playerBoard);
			currentController = playerBoardController;
			currentController.MyMove = true;
			gameStarted = true;
		}
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(0);
		}

		if (!gameStarted) return;
		if (Wait()) return;
		if (!currentController.MyMove)
		{
			if (currentController.CheckForWin())
			{
				print(currentController.name + " wins!!!");
				gameStarted = false;
				wait = false;
			}

			if (currentController == playerBoardController)
				currentController = aiBoardController;
			else
				currentController = playerBoardController;
			currentController.MyMove = true;
			wait = true;
		}
	}


	void OnGUI()
	{
		if (Wait())
		{
			if (GUILayout.Button("Next"))
			{
				wait = false;
			}
		}
	}

	private bool Wait()
	{
		return waitBetweenMoves && wait;
	}
}
