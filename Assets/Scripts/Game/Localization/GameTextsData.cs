using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameTextsData", menuName = "Custom/GameTextsData")]
public class GameTextsData : ScriptableObject
{
    [System.Serializable]
    public class GameTextEntry
    {
        public string _key;

        [TextArea(2,7)]
        public string _value;
    }

    public List<GameTextEntry> _gameTextEntries;

}
