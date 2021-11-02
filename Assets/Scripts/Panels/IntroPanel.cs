using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanel : APanel
{
	public GameGrid gameGrid;

	public AudioClip showCategoriesClip;
	public AudioClip showTitleClip;

	public float startWaitTime;
	public float categoryShowTime;

	public float showTitleInitialTime;
	public float showTitleInteremTime;

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Intro;
	}

	public override void Show()
	{
		base.Show();

		gameGrid.PopulateGrid(gameController.jeopardyData, false);

		StartCoroutine(ShowQuestionsRandomly());

		gameController.PlayClip(showCategoriesClip, false);
	}

	private IEnumerator ShowQuestionsRandomly()
	{
		yield return new WaitForSeconds(startWaitTime);

		var questions = new List<CategoryQuestion>(gameGrid.gameObject.GetComponentsInChildren<CategoryQuestion>(true));

		float timeBetweenQuestions = categoryShowTime / questions.Count;

		while (questions.Count > 0)
		{
			int randIdx = UnityEngine.Random.Range(0, questions.Count);

			questions[randIdx].Show();

			questions.RemoveAt(randIdx);

			yield return new WaitForSeconds(timeBetweenQuestions);
		}

		var titles = new List<CategoryHeader>(gameGrid.gameObject.GetComponentsInChildren<CategoryHeader>(true));

		yield return new WaitForSeconds(showTitleInitialTime);

		foreach (var title in titles)
		{
			gameController.PlayClip(showTitleClip, false);
			title.ShowCategory();

			yield return new WaitForSeconds(showTitleInteremTime);
		}

		gameController.ChangeState(GameController.EGameState.Game);
	}
}
