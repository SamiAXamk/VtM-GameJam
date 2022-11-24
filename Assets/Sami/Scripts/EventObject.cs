using System.Collections.Generic;
using UnityEngine;

/*
This code is for a visual novel style game.
The EventObject is the main component of the game containing the text and choice data for each game view.
Each invidual view (meaning a text or a choice) is an EventObject.
All the text that is in a text box is in one EventObject.
When player click(or something) to progress the story text the new text is in another EventObject.
 */

[System.Serializable]
public class EventObject
{
    public int nextObjectIndex;         // reference to the next EventObject
    public int previousObjectIndex;     // in case there ever is need to go back to preious EventObject
    public ObjectType type;
    [TextArea] public string text;      // contains the text for text EventObject
    public bool isItalic;

    public bool hasReward;
    public RewardType rewardType;
    public Skill rewardSkill;
    public int rewardValue;

    public List<ChoiceObject> choices;  // list of all the choices

    // characters for left and right side if they are needed
    //public Sprite leftSideCharacter;
    public bool playerCharacter;
    public Sprite rightSideCharacter;
}