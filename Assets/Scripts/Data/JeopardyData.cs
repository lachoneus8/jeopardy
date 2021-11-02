using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JeopardyData", menuName = "Jeopardy/JeopardyData")]
public class JeopardyData : ScriptableObject
{
	[Serializable]
	public class BaseData
	{
		public int numQuestionsPerCategory = 5;
		public int scorePerIncrement = 200;

		public List<CategoryData> categoryList;

		public void SetScores()
		{
			foreach (var category in categoryList)
			{
				int curScore = scorePerIncrement;

				foreach (var question in category.questionList)
				{
					question.score = curScore;

					curScore += scorePerIncrement;
				}
			}
		}

		public string GetCategoryName(int curCategoryIdx)
		{
			return categoryList[curCategoryIdx].categoryName;
		}

		public int GetScore(int curCategoryIdx, int curQuestionIdx)
		{
			return categoryList[curCategoryIdx].questionList[curQuestionIdx].score;
		}
	}

	[Serializable]
	public class CategoryData
	{
		public string categoryName;
		public List<QuestionData> questionList;
	}

	[Serializable]
	public class QuestionData
	{
		public string question;
		public string answer;

		[HideInInspector]
		public int score;
	}

	public BaseData baseData;

	public BaseData DeepCopy()
	{
		var copy = new BaseData();
		copy.numQuestionsPerCategory = baseData.numQuestionsPerCategory;
		copy.scorePerIncrement = baseData.scorePerIncrement;

		copy.categoryList = new List<CategoryData>();
		foreach (var category in baseData.categoryList)
		{
			var categoryCopy = new CategoryData();
			categoryCopy.categoryName = category.categoryName;

			categoryCopy.questionList = new List<QuestionData>();
			foreach (var question in category.questionList)
			{
				var questionCopy = new QuestionData();
				questionCopy.question = question.question;
				questionCopy.answer = question.answer;

				categoryCopy.questionList.Add(questionCopy);
			}

			copy.categoryList.Add(categoryCopy);
		}

		return copy;
	}
}
