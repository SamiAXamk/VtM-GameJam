using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldValueScript : MonoBehaviour
{
    public Attribute attribute;
    public Skill skill;
    public int value = 1;
    public Text field;

    public int tempClan = 0;
    public int tempPredator = 0;

    private void Start()
    {
        UpdateField();
    }

    public void UpdateField()
    {
        int num;
        if (skill == Skill.Investigation)
            num = value + tempClan + tempPredator + 4;
        else
            num = value + tempClan + tempPredator;

        field.text = num.ToString();
    }
}
