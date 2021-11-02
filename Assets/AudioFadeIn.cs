using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour
{
	public float fadeInTime;

	private float timeRemaining;


	private void Start()
	{
		timeRemaining = fadeInTime;
	}

	private void Update()
    {
		timeRemaining -= Time.deltaTime;

		var t = (fadeInTime - timeRemaining) / fadeInTime;
		var source = GetComponent<AudioSource>();

		source.volume = t;
		
		if (timeRemaining <= 0)
		{
			Destroy(this);
		}
    }
}
