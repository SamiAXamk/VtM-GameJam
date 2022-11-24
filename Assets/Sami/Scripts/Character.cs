using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    // attributes
    public Dictionary<Attribute, int> attributes;

    // skills
    public Dictionary<Skill, int> skills;

    // other
    public int bloodBags, resources, experience, hunger, health;
    public Sect sect;
    public PredatorType predatorType;
    public Clan clan;
    public Sprite portrait;

    public int actionPoints;

    // TO DO
    // completed events

    public Character(int social, int physical, int mental, int firearms, int stealth, int larceny, int drive, int brawl, int athletics,
                     int investigation, int academics, int occult, int politics, int awareness,
                     int insight, int persuasion, int streetwise, int subterfuge, int science, int resource)
    {
        attributes = new Dictionary<Attribute, int>();
        attributes.Add(Attribute.Social, social);
        attributes.Add(Attribute.Physical, physical);
        attributes.Add(Attribute.Mental, mental);

        skills = new Dictionary<Skill, int>();
        skills.Add(Skill.Firearms, firearms);
        skills.Add(Skill.Stealth, stealth);
        skills.Add(Skill.Larceny, larceny);
        skills.Add(Skill.Brawl, brawl);
        skills.Add(Skill.Athletics, athletics);
        skills.Add(Skill.Investigation, investigation);
        skills.Add(Skill.Academics, academics);
        skills.Add(Skill.Occult, occult);
        skills.Add(Skill.Politics, politics);
        skills.Add(Skill.Awareness, awareness);
        skills.Add(Skill.Insight, insight);
        skills.Add(Skill.Persuasion, persuasion);
        skills.Add(Skill.Streetwise, streetwise);
        skills.Add(Skill.Subterfuge, subterfuge);
        skills.Add(Skill.Science, science);
        skills.Add(Skill.Resource, resource);
    }

    public Character() {
        bloodBags = 0;
        resources = 0;
        experience = 0;
        hunger = 0;
        health = 5;
        actionPoints = 3;

        attributes = new Dictionary<Attribute, int>();
        skills = new Dictionary<Skill, int>();
    }

    public void MendHealth()
    {
        if (health < 5 && hunger < 4)
        {
            hunger++;
            health += 2;
        }
    }
}
