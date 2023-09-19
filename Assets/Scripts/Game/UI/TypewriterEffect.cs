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

			_tmpProText.text += c;

			_tmpProText.text += _leadingChar;

			PlayTypeSFX();

			yield return new WaitForSeconds(_timeBtwChars);
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

		int rndIdx = UnityEngine.Random.Range(0, _typewritterClips.Length);

		_audioSource.PlayOneShot(_typewritterClips[rndIdx]);
    }

    public void Flush()
	{
		StopAllCoroutines();

		_tmpProText.text = _writer;

		_audioSource.Stop();
	}
}