using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryQuestion : MonoBehaviour
{
	public GameGrid.QuestionSelectedEvent OnQuestionSelected = new GameGrid.QuestionSelectedEvent();

	public Button categorySelectButton;
	public Text valueText;
	public Image backgroundImage;

	private int categoryIdx;

	private int questionIdx;

	private void Start()
	{
		categorySelectButton.onClick.AddListener(HandleQuestionClick);
	}

	public void Setup(JeopardyData.QuestionData questionData, int categoryIdx, int questionIdx, bool isVisible)
	{
		valueText.text = "$ " + questionData.score;

		this.categoryIdx = categoryIdx;
		this.questionIdx = questionIdx;

		backgroundImage.gameObject.SetActive(isVisible);
		valueText.gameObject.SetActive(isVisible);
	}

	public void Show()
	{
		backgroundImage.gameObject.SetActive(true);
		valueText.gameObject.SetActive(true);
	}

	private void HandleQuestionClick()
	{
		OnQuestionSelected.Invoke(categoryIdx, questionIdx);

		backgroundImage.gameObject.SetActive(false);
		valueText.gameObject.SetActive(false);

		categorySelectButton.interactable = false;
	}
}
