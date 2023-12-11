using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FMOD.Studio;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
	[SerializeField] float _delayBeforeStart = 0f;
	[SerializeField] float _timeBtwChars = 0.1f;
	[SerializeField] string _leadingChar = "";
	[SerializeField] bool _leadingCharBeforeDelay = false;

	TMP_Text _tmpProText;
	string _writer;

	EventInstance _typewriterSFX;

    void Awake()
    {
		_typewriterSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.Typewriter);
	}

    void Start()
	{
		_tmpProText = GetComponent<TMP_Text>();		

		_writer = Localization.Instance.GetTextByKey(_tmpProText.text);

		_tmpProText.text = "";

		StartCoroutine(TypeWriter());
	}

	int _charCount = 0;

	IEnumerator TypeWriter()
	{
		_charCount = 0;

		_tmpProText.text = _leadingCharBeforeDelay ? _leadingChar : "";

		yield return new WaitForSeconds(_delayBeforeStart);

		_writer.Replace("<br>", "\n");

		foreach (char c in _writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - _leadingChar.Length);
			}

			//Debug.Log($"[TypewritterEffect] TypeWriter) char: {c}, hashcode:{c.GetHashCode()}");

			// hides a glitch when it renders the new line in the text (¿?)
			bool isEnter = c.GetHashCode() == 851981;
			if (isEnter == false)
			{
				_tmpProText.text += c.ToString() + _leadingChar;
			}
			else
			{
				_tmpProText.text += _leadingChar;
			}

			_charCount++;

			if (_charCount % 7 == 0)
			{
				PlayTypeWordSFX();
			}			

			yield return new WaitForSeconds(_timeBtwChars);
		}

		if (_leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - _leadingChar.Length);
		}
	}

    void PlayTypeWordSFX()
    {
		StartCoroutine(PlayTypeAWordSFXDelayed());
    }

	IEnumerator PlayTypeAWordSFXDelayed()
	{
		int rndCharCount = UnityEngine.Random.Range(1, 5);

		for (int i = 0; i < rndCharCount; i++)
		{
			AudioController.Instance.PlayEvent(_typewriterSFX, true);

			yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.09f));
		}

		yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.3f));
	}


    public void Flush()
	{
		StopAllCoroutines();

		_tmpProText.text = _writer;

		AudioController.Instance.StopEventIfPlaying(_typewriterSFX);
	}
}