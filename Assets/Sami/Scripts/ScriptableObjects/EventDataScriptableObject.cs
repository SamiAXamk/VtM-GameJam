using System.Collections.Generic;
using UnityEngine;

// this make it possibe to create new EventData objects from menu
[CreateAssetMenu(fileName = "EventData", menuName = "ScriptableObjects/Event Data", order = 1)]
public class EventDataScriptableObject : ScriptableObject
{
    public int eventId;
    public int actionPoints;// action point cost
    [Tooltip("0 for no condition")]
    public int boolListIndex;
    public EventLocation eventLocation;
    public bool repeatable;
    public bool mandatory;
    public bool done;
    [TextArea] public string description;
    [Space(10)]
    public Sprite eventIcon;
    public Sprite eventBackgroundImage;
    [Space(20)]
    public List<EventObject> eventObjects;
}
