using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : APanel
{
	public Text headerText;
	public Text questionText;

	public AudioClip thinkingClip;

	public Button showAnswerButton;

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Question;
	}

	public override void Show()
	{
		base.Show();

		headerText.text = gameController.GetHeaderText();
		questionText.text = gameController.GetQuestionText();

		showAnswerButton.onClick.AddListener(HandleShowAnswer);

		gameController.PlayClip(thinkingClip, true);
	}

	public override void Hide()
	{
		base.Hide();

		showAnswerButton.onClick.RemoveListener(HandleShowAnswer);
	}

	private void HandleShowAnswer()
	{
		gameController.ChangeState(GameController.EGameState.Answer);
	}
}
