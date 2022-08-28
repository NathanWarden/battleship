using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
	[SerializeField] private BoardData playerBoard;
	[SerializeField] private BoardController playerBoardController;
	[SerializeField] private BoardData aiBoard;
	[SerializeField] private BoardController aiBoardController;

	private bool gameStarted;
	private BoardController currentController;


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
		if (!currentController.MyMove)
		{
			if (currentController.CheckForWin())
			{
				print(currentController.name + " wins!!!");
				gameStarted = false;
			}

			if (currentController == playerBoardController)
				currentController = aiBoardController;
			else
				currentController = playerBoardController;
			currentController.MyMove = true;
		}
	}
}
