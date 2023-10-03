using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    TMP_Text _textComponent;
    
    string _localizedTextKey;
        
    void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
        
        _localizedTextKey = _textComponent.text;

        SetLocalizedText(_localizedTextKey);
    }

    void SetLocalizedText(string text)
    {
        _textComponent.text = Localization.Instance.GetTextByKey(text);
    }

    void Update()
    {
        
    }
}
