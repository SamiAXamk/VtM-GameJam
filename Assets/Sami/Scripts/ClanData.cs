[System.Serializable]
public class ClanData
{
    public Clan clan;
    public SkillStruct[] skills;
}

[System.Serializable]
public struct SkillStruct
{
    public Skill skill;
    public int value;
}