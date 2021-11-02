using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public const string cJeopardyDataKey = "JeopardyData";

	public enum EGameState
	{
		Ready,
		Setup,
		Intro,
		Game,
		Question,
		Answer
	}

	public List<APanel> panels;

	public JeopardyData startJeopardyData;

	[NonSerialized]
	public JeopardyData.BaseData jeopardyData;

	public AudioSource audioSource;

	private Dictionary<EGameState, APanel> panelLookup = new Dictionary<EGameState, APanel>();

	private APanel curPanel = null;

	private int curCategoryIdx = 0;
	private int curQuestionIdx = 0;

	public void ChangeState(EGameState state)
	{
		StopClip();

		if (curPanel != null)
		{
			curPanel.Hide();
		}

		var panel = panelLookup[state];
		panel.Show();

		curPanel = panel;
	}

	public void ReloadData()
	{
		if (PlayerPrefs.HasKey(cJeopardyDataKey))
		{
			try
			{
				var loadedJeopardyData = JsonUtility.FromJson<JeopardyData.BaseData>(PlayerPrefs.GetString(cJeopardyDataKey));
				jeopardyData = loadedJeopardyData;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to parse JeopardyData: " + ex.ToString());
			}
		}

		if (jeopardyData == null)
		{
			jeopardyData = startJeopardyData.DeepCopy();
		}

		jeopardyData.SetScores();
	}

	public void PlayClip(AudioClip audioClip, bool loop)
	{
		audioSource.clip = audioClip;
		audioSource.loop = loop;
		audioSource.Play();
	}

	public void StopClip()
	{
		audioSource.Stop();
	}

	public void SetActiveQuestion(int categoryIdx, int questionIdx)
	{
		curCategoryIdx = categoryIdx;
		curQuestionIdx = questionIdx;

		ChangeState(EGameState.Question);
	}

	public string GetHeaderText()
	{
		var curQuestionCategory = jeopardyData.GetCategoryName(curCategoryIdx);
		var scoreValue = jeopardyData.GetScore(curCategoryIdx, curQuestionIdx);
		return curQuestionCategory + " for $ " + scoreValue;
	}

	public string GetQuestionText()
	{
		return jeopardyData.categoryList[curCategoryIdx].questionList[curQuestionIdx].question;
	}

	public string GetAnswerText()
	{
		return jeopardyData.categoryList[curCategoryIdx].questionList[curQuestionIdx].answer;
	}

	private void Start()
    {
		ReloadData();

        foreach (var panel in panels)
		{
			panel.Initialize(this);
			panel.Hide();

			panelLookup[panel.GetGameState()] = panel;
		}

		ChangeState(EGameState.Ready);
	}
}
