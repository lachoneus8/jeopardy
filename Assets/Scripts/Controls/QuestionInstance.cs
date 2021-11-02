using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionInstance : MonoBehaviour
{
    public Text valueText;
    public InputField questionInput;
    public InputField answerInput;

    private JeopardyData.QuestionData questionData;

    public void Setup(JeopardyData.QuestionData questionData)
	{
        this.questionData = questionData;
        valueText.text = "$" + questionData.score;
        questionInput.text = questionData.question;
        answerInput.text = questionData.answer;

        questionInput.onEndEdit.AddListener(HandleQuestionEdit);
        answerInput.onEndEdit.AddListener(HandleAnswerEdit);
    }

	private void HandleQuestionEdit(string questionValue)
	{
        questionData.question = questionValue;
	}

    private void HandleAnswerEdit(string answerValue)
    {
        questionData.answer = answerValue;
    }
}
