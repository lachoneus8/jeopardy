using System;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPanel : APanel
{
	public Text headerText;
	public Text answerText;

	public Button returnToBoardButton;

	public AudioClip rewardClip;

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Answer;
	}

	public override void Show()
	{
		base.Show();

		headerText.text = gameController.GetHeaderText();
		answerText.text = gameController.GetAnswerText();

		returnToBoardButton.onClick.AddListener(HandleReturn);

		gameController.PlayClip(rewardClip, false);
	}

	public override void Hide()
	{
		base.Hide();

		returnToBoardButton.onClick.RemoveListener(HandleReturn);
	}

	private void HandleReturn()
	{
		gameController.ChangeState(GameController.EGameState.Game);
	}
}
