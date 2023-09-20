using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
	[SerializeField] float _delayBeforeStart = 0f;
	[SerializeField] float _timeBtwChars = 0.1f;
	[SerializeField] string _leadingChar = "";
	[SerializeField] bool _leadingCharBeforeDelay = false;
	[SerializeField] AudioClip[] _typewritterClips;

	TMP_Text _tmpProText;
	string _writer;

	AudioSource _audioSource;

	void Start()
	{
		_tmpProText = GetComponent<TMP_Text>();

		_audioSource = GetComponent<AudioSource>();

		_writer = _tmpProText.text;

		_tmpProText.text = "";

		StartCoroutine(TypeWriter());
	}

	IEnumerator TypeWriter()
	{
		_tmpProText.text = _leadingCharBeforeDelay ? _leadingChar : "";

		yield return new WaitForSeconds(_delayBeforeStart);

		foreach (char c in _writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - _leadingChar.Length);
			}

			_tmpProText.text += c.ToString() + _leadingChar;

			PlayTypeSFX();

			// hides a glitch when it renders the new line in the text (¿?)
			bool isEnter = c.GetHashCode() == 851981;
			if (isEnter == false)
			{
				yield return new WaitForSeconds(_timeBtwChars);
			}			
		}

		if (_leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - _leadingChar.Length);
		}
	}

    void PlayTypeSFX()
    {
		if (_audioSource.isPlaying)
		{
			return;
		}

		StartCoroutine(PlayTypeAWordSFXDelayed());
    }

	IEnumerator PlayTypeAWordSFXDelayed()
	{
		int rndCharCount = UnityEngine.Random.Range(1, 10);

		for (int i = 0; i < rndCharCount; i++)
		{
			// play random press sfx			
			int rndIdx = UnityEngine.Random.Range(0, _typewritterClips.Length);

			_audioSource.PlayOneShot(_typewritterClips[rndIdx]);

			yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.09f));
		}

		yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.3f));
	}


    public void Flush()
	{
		StopAllCoroutines();

		_tmpProText.text = _writer;

		_audioSource.Stop();
	}
}