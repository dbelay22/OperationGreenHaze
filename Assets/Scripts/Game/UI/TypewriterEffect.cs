using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
	[SerializeField] float _delayBeforeStart = 0f;
	[SerializeField] float _timeBtwChars = 0.1f;
	[SerializeField] string _leadingChar = "";
	[SerializeField] bool _leadingCharBeforeDelay = false;

	TMP_Text _tmpProText;
	string _writer;

	void Start()
	{
		_tmpProText = GetComponent<TMP_Text>();

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

			yield return new WaitForSeconds(_timeBtwChars);
		}

		if (_leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - _leadingChar.Length);
		}
	}

	public void Flush()
	{
		StopAllCoroutines();

		_tmpProText.text = _writer;
	}
}