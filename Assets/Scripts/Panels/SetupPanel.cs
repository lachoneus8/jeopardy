using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanel : APanel
{
	public Dropdown numCategoriesDropdown;
	public Dropdown numQuestionsDropdown;
	public Dropdown currentCategoryDropdown;
	public InputField categoryNameInput;

	public Button setupCompleteButton;
	public Button setupCancelButton;

	public Button exportButton;

	public Transform questionParent;
	public GameObject questionPrefab;

	public InputField pathText;

	public Button clearSetupButton;
	public Button saveSetupButton;
	public Button loadSetupButton;

	private List<QuestionInstance> questionInstances = new List<QuestionInstance>();

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Setup;
	}

	public override void Show()
	{
		base.Show();

		numCategoriesDropdown.onValueChanged.AddListener(HandleNumCategoriesChanged);
		numQuestionsDropdown.onValueChanged.AddListener(HandleNumQuestionsChanged);
		currentCategoryDropdown.onValueChanged.AddListener(HandleCurrentCategoryChanged);

		categoryNameInput.onEndEdit.AddListener(HandleCategoryNameChanged);

		setupCompleteButton.onClick.AddListener(HandleSetupComplete);
		setupCancelButton.onClick.AddListener(HandleSetupCancel);

		exportButton.onClick.AddListener(HandleExport);

		clearSetupButton.onClick.AddListener(HandleClearSetup);
		saveSetupButton.onClick.AddListener(HandleSaveSetup);
		loadSetupButton.onClick.AddListener(HandleLoadSetup);

		RefreshData();
	}

	public override void Hide()
	{
		base.Hide();

		numCategoriesDropdown.onValueChanged.RemoveListener(HandleNumCategoriesChanged);
		numQuestionsDropdown.onValueChanged.RemoveListener(HandleNumQuestionsChanged);
		currentCategoryDropdown.onValueChanged.RemoveListener(HandleCurrentCategoryChanged);

		categoryNameInput.onEndEdit.RemoveListener(HandleCategoryNameChanged);

		setupCompleteButton.onClick.RemoveListener(HandleSetupComplete);
		setupCancelButton.onClick.RemoveListener(HandleSetupCancel);

		exportButton.onClick.RemoveListener(HandleExport);

		clearSetupButton.onClick.RemoveListener(HandleClearSetup);
		saveSetupButton.onClick.RemoveListener(HandleSaveSetup);
		loadSetupButton.onClick.RemoveListener(HandleLoadSetup);
	}

	private void HandleNumCategoriesChanged(int numCategoriesIdx)
	{
		int numCategories = numCategoriesIdx + 3;
		while (numCategories < gameController.jeopardyData.categoryList.Count)
		{
			gameController.jeopardyData.categoryList.RemoveAt(gameController.jeopardyData.categoryList.Count - 1);
		}

		while (numCategories > gameController.jeopardyData.categoryList.Count)
		{
			int categoryNum = gameController.jeopardyData.categoryList.Count + 1;
			var newCategory = new JeopardyData.CategoryData();
			newCategory.categoryName = "Category " + categoryNum;

			newCategory.questionList = new List<JeopardyData.QuestionData>();
			int numQuestions = numQuestionsDropdown.value + 3;
			UpdateQuestions(newCategory, numQuestions);

			gameController.jeopardyData.categoryList.Add(newCategory);
		}

		int curIdx = currentCategoryDropdown.value;

		ResetCategoryList();

		currentCategoryDropdown.value = curIdx;
	}

	private void HandleNumQuestionsChanged(int numQuestionsIdx)
	{
		int numQuestions = numQuestionsIdx + 3;

		gameController.jeopardyData.numQuestionsPerCategory = numQuestions;

		foreach (var category in gameController.jeopardyData.categoryList)
		{
			UpdateQuestions(category, numQuestions);
		}

		HandleCurrentCategoryChanged(currentCategoryDropdown.value);
	}

	private void HandleCurrentCategoryChanged(int currentCategoryIdx)
	{
		if (currentCategoryIdx < 0)
		{
			return;
		}

		var currentCategory = gameController.jeopardyData.categoryList[currentCategoryIdx];

		categoryNameInput.text = currentCategory.categoryName;

		foreach (var questionInstance in questionInstances)
		{
			Destroy(questionInstance.gameObject);
		}

		questionInstances.Clear();

		foreach (var question in currentCategory.questionList)
		{
			var questionInstanceObj = Instantiate(questionPrefab, questionParent);
			var questionInstance = questionInstanceObj.GetComponent<QuestionInstance>();
			questionInstance.Setup(question);

			questionInstances.Add(questionInstance);
		}
	}

	private void HandleCategoryNameChanged(string categoryName)
	{
		int curCategory = currentCategoryDropdown.value;
		gameController.jeopardyData.categoryList[curCategory].categoryName = categoryName;

		int curIdx = currentCategoryDropdown.value;

		ResetCategoryList();

		currentCategoryDropdown.value = curIdx;
	}

	private void HandleSetupComplete()
	{
		var json = JsonUtility.ToJson(gameController.jeopardyData);
		PlayerPrefs.SetString(GameController.cJeopardyDataKey, json);
		PlayerPrefs.Save();

		gameController.ChangeState(GameController.EGameState.Ready);
	}

	private void HandleSetupCancel()
	{
		gameController.ReloadData();

		gameController.ChangeState(GameController.EGameState.Ready);
	}

	private void HandleExport()
	{
		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.AppendLine("Jeopardy Answer Key");
		foreach (var category in gameController.jeopardyData.categoryList)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Category: " + category.categoryName);

			foreach (var question in category.questionList)
			{
				stringBuilder.AppendLine(question.score + " - Q: " + question.question);
				stringBuilder.AppendLine("    - A: " + question.answer);
			}
		}

		var fileName = Path.Combine(Application.persistentDataPath, "answerkey.txt");

		File.WriteAllText(fileName, stringBuilder.ToString());

		pathText.text = fileName;
	}

	private void HandleClearSetup()
	{
		gameController.jeopardyData = gameController.startJeopardyData.DeepCopy();
		gameController.jeopardyData.SetScores();
		RefreshData();
	}

	private void HandleSaveSetup()
	{
		StartCoroutine(DoSave()); 
	}

	private void HandleLoadSetup()
	{
		StartCoroutine(DoLoad());
	}

	private IEnumerator DoSave()
	{
		FileBrowser.SetDefaultFilter(".json");
		FileBrowser.SetFilters(true, new FileBrowser.Filter("JSON Files", ".json"));
		yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, initialFilename:"jeopardy.json");

		if (FileBrowser.Success && FileBrowser.Result.Length > 0)
		{
			var filename = FileBrowser.Result[0];
			var json = JsonUtility.ToJson(gameController.jeopardyData);

			File.WriteAllText(filename, json);
		}
	}

	private IEnumerator DoLoad()
	{
		FileBrowser.SetDefaultFilter(".json");
		FileBrowser.SetFilters(true, new FileBrowser.Filter("JSON Files", ".json"));
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

		if (FileBrowser.Success && FileBrowser.Result.Length > 0)
		{
			var filename = FileBrowser.Result[0];
			var json = File.ReadAllText(filename);

			var loadedJeopardyData = JsonUtility.FromJson<JeopardyData.BaseData>(json);
			gameController.jeopardyData = loadedJeopardyData;

			RefreshData();
		}
	}

	private void RefreshData()
	{
		numCategoriesDropdown.value = gameController.jeopardyData.categoryList.Count - 3;
		numQuestionsDropdown.value = gameController.jeopardyData.numQuestionsPerCategory - 3;

		ResetCategoryList();

		currentCategoryDropdown.value = -1;
		currentCategoryDropdown.value = 0;
	}

	private void ResetCategoryList()
	{
		currentCategoryDropdown.options.Clear();
		foreach (var category in gameController.jeopardyData.categoryList)
		{
			currentCategoryDropdown.options.Add(new Dropdown.OptionData(category.categoryName));
		}

		currentCategoryDropdown.captionText.text = currentCategoryDropdown.options[currentCategoryDropdown.value].text;
	}

	private void UpdateQuestions(JeopardyData.CategoryData category, int numQuestions)
	{
		while (category.questionList.Count > numQuestions)
		{
			category.questionList.RemoveAt(category.questionList.Count - 1);
		}

		while (category.questionList.Count < numQuestions)
		{
			int questionNum = category.questionList.Count + 1;
			var newQuestion = new JeopardyData.QuestionData();
			newQuestion.score = questionNum * gameController.jeopardyData.scorePerIncrement;
			newQuestion.question = "Question " + questionNum;
			newQuestion.answer = "Answer " + questionNum;

			category.questionList.Add(newQuestion);
		}
	}
}
