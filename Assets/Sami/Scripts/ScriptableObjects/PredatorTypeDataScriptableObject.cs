using UnityEngine;

// this make it possibe to create new this scriptableobject from menu
[CreateAssetMenu(fileName = "PredatorTypeData", menuName = "ScriptableObjects/Predator Type Data", order = 3)]
public class PredatorTypeDataScriptableObject : ScriptableObject
{
    public PredatorTypeData[] predatorTypeData;
}
