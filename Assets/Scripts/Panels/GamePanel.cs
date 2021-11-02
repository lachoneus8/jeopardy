using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : APanel
{
	public GameGrid gameGrid;

	private bool isPopulated = false;

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Game;
	}

	public override void Show()
	{
		base.Show();

		if (!isPopulated)
		{
			gameGrid.PopulateGrid(gameController.jeopardyData, true);
			isPopulated = true;
		}

		gameGrid.OnQuestionSelected.AddListener(HandleQuestionSelected);
	}

	public override void Hide()
	{
		base.Hide();

		gameGrid.OnQuestionSelected.RemoveListener(HandleQuestionSelected);
	}

	private void HandleQuestionSelected(int categoryIdx, int questionIdx)
	{
		gameController.SetActiveQuestion(categoryIdx, questionIdx);
	}
}
