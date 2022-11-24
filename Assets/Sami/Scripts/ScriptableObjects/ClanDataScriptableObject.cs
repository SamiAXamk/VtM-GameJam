using System.Collections.Generic;
using UnityEngine;

// this make it possibe to create new this scriptableobject from menu
[CreateAssetMenu(fileName = "ClanData", menuName = "ScriptableObjects/Clan Data", order = 2)]
public class ClanDataScriptableObject : ScriptableObject
{
    public List<ClanData> clanData;
}
