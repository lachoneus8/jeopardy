using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APanel : MonoBehaviour
{
	protected GameController gameController;

	public abstract GameController.EGameState GetGameState();

	public virtual void Initialize(GameController gameController)
	{
		this.gameController = gameController;
	}

	public virtual void Show()
	{
		gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);
	}
}
