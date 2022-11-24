[System.Serializable]
public class ChoiceObject
{
    public int nextObjectIndex;
    public string choiceText;
    public bool check;
    public CheckType checkType;
    public bool hidden;             // if true show the check choice only if player fulfills the requirements

    // things that are checked
    public int difficulty;
    public int boolIndex;
    public Attribute attribute;
    public Skill skill;
    public Sect sect;
    public PredatorType predatorType;
}