using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPanel : APanel
{
	public AudioClip introMusic;

	public Button setupButton;

	public override GameController.EGameState GetGameState()
	{
		return GameController.EGameState.Ready;
	}

	public override void Show()
	{
		base.Show();

		gameController.PlayClip(introMusic, true);

		setupButton.onClick.AddListener(HandleSetup);
	}

	private void HandleSetup()
	{
		gameController.ChangeState(GameController.EGameState.Setup);
	}

	private void Update()
	{
		if (Input.anyKey && !Input.GetMouseButton(0))
		{
			gameController.ChangeState(GameController.EGameState.Intro);
		}
	}
}
