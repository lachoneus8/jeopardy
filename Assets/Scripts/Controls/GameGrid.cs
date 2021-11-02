using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameGrid : MonoBehaviour
{
	public GameObject categoryHeaderPrefab;
	public GameObject categoryQuestionPrefab;

	public class QuestionSelectedEvent : UnityEvent<int, int>
	{

	}

	public QuestionSelectedEvent OnQuestionSelected = new QuestionSelectedEvent();

	private List<GameObject> gridItems = new List<GameObject>();

	public void PopulateGrid(JeopardyData.BaseData jeopardyData, bool questionsVisible)
	{
		foreach (var item in gridItems)
		{
			Destroy(item);
		}
		gridItems.Clear();

		var layoutGroup = GetComponent<GridLayoutGroup>();
		layoutGroup.constraintCount = jeopardyData.categoryList.Count;

		for (int row = 0; row < jeopardyData.numQuestionsPerCategory + 1; ++row)
		{
			int categoryIdx = 0;

			foreach (var category in jeopardyData.categoryList)
			{
				if (row == 0)
				{
					CreateCategoryHeader(category, !questionsVisible);
				}
				else
				{
					CreateCategoryQuestion(category, categoryIdx, row - 1, questionsVisible);
				}

				++categoryIdx;
			}
		}
	}

	private void CreateCategoryHeader(JeopardyData.CategoryData category, bool isHidden)
	{
		var categoryHeader = Instantiate(categoryHeaderPrefab, transform).GetComponent<CategoryHeader>();
		categoryHeader.Setup(category, isHidden);

		gridItems.Add(categoryHeader.gameObject);
	}

	private void CreateCategoryQuestion(JeopardyData.CategoryData category, int categoryIdx, int questionIdx, bool startVisible)
	{
		var categoryQuestion = Instantiate(categoryQuestionPrefab, transform).GetComponent<CategoryQuestion>();
		categoryQuestion.Setup(category.questionList[questionIdx], categoryIdx, questionIdx, startVisible);

		gridItems.Add(categoryQuestion.gameObject);

		categoryQuestion.OnQuestionSelected.AddListener(HandleCategoryQuestionClick);
	}

	private void HandleCategoryQuestionClick(int categoryIdx, int questionIdx)
	{
		OnQuestionSelected.Invoke(categoryIdx, questionIdx);
	}
}
