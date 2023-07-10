using System.Collections;
using UnityEngine;

namespace Yxp.Audio
{
	public static class AudioFader
	{
		public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
		{
			float startVolume = audioSource.volume;
			while (audioSource.volume > 0)
			{
				audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
				yield return null;
			}
			audioSource.Stop();
			audioSource.volume = 1;
			yield break;
		}

		public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
		{
			audioSource.Play();
			audioSource.volume = 0f;
			while (audioSource.volume < 1)
			{
				audioSource.volume += Time.deltaTime / FadeTime;
				yield return null;
			}
			yield break;
		}

	}

}