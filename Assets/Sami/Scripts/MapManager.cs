using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject eventPointParent;
    private List<GameObject> eventPointsLocation = new List<GameObject>();  // eventPoints with location
    private List<GameObject> eventPointsNone = new List<GameObject>();      // eventPoints without location

    public GameObject eventIconParent;
    public List<GameObject> eventIcons = new List<GameObject>();

    private Object[] allEvents;                                             // array of all events in the game
    public List<Object> thisNightEvents = new List<Object>();

    public GameObject mainCamera;
    private Camera camera;
    private bool firstNight = true;

    // Start is called before the first frame update
    void Start()
    {
        camera = mainCamera.GetComponent<Camera>();

        // get event points in list
        foreach (Transform child in eventPointParent.transform)
        {
            if (child.gameObject.GetComponent<EventPointScript>().eventLocation == EventLocation.none)
                eventPointsNone.Add(child.gameObject);
            else
                eventPointsLocation.Add(child.gameObject);
        }

        // get event icons in list
        foreach (Transform child in eventIconParent.transform)
        {
            eventIcons.Add(child.gameObject);
        }

        // fill the array with all the events
        allEvents = Resources.LoadAll("EventData");
    }

    public void MapSetup()
    {
        GetEvents();// get event for this night
        SetEventIcons();// set icon positions on event points
    }

    // set icon positions on event points
    private void SetEventIcons()
    {
        Vector3 screenPos;

        int i = 0;
        int eloc = 0;// for location

        // go through all events for this night
        foreach (EventDataScriptableObject obj in thisNightEvents)
        {
            // event location is none and index is not out of range
            if (obj.eventLocation == EventLocation.none && i < eventPointsNone.Count && i < eventIcons.Count)
            {
                screenPos = camera.WorldToScreenPoint(eventPointsNone[i].transform.position);

                eventIcons[i].transform.position = screenPos;
                eventIcons[i].GetComponent<IconScript>().eventId = obj.eventId;
                eventIcons[i].GetComponent<IconScript>().eventDescription = obj.description;
                eventIcons[i].GetComponent<IconScript>().eventCost = obj.actionPoints;
                eventIcons[i].GetComponent<IconScript>().costField.GetComponent<Text>().text = obj.actionPoints.ToString();
                eventIcons[i].GetComponent<IconScript>().descriptionField.GetComponent<Text>().text = obj.description;
                eventIcons[i].SetActive(true);

                i++;
            }
            // event has location
            else
            {
                // go through all eventPoints that have location
                foreach (GameObject ePoint in eventPointsLocation)
                {
                    // event location matches point location, set event to point
                    //if (obj.eventLocation == ePoint.GetComponent<EventPointScript>().eventLocation && eloc < eventPointsLocation.Count && i < eventIcons.Count)
                    if (obj.eventLocation == ePoint.GetComponent<EventPointScript>().eventLocation && i < eventPointsLocation.Count && i < eventIcons.Count)
                    {
                        screenPos = camera.WorldToScreenPoint(ePoint.transform.position);

                        eventIcons[i].transform.position = screenPos;
                        eventIcons[i].GetComponent<IconScript>().eventId = obj.eventId;
                        eventIcons[i].GetComponent<IconScript>().eventDescription = obj.description;
                        eventIcons[i].GetComponent<IconScript>().eventCost = obj.actionPoints;
                        eventIcons[i].GetComponent<IconScript>().costField.GetComponent<Text>().text = obj.actionPoints.ToString();
                        eventIcons[i].GetComponent<IconScript>().descriptionField.GetComponent<Text>().text = obj.description;
                        eventIcons[i].SetActive(true);

                        //eloc++;
                        i++;
                    }
                }
            }
            //screenPos = camera.WorldToScreenPoint(eventPointsLocation[i].transform.position);

            //eventIcons[i].transform.position = screenPos;
            //eventIcons[i].GetComponent<IconScript>().eventId = obj.eventId;
            //eventIcons[i].GetComponent<IconScript>().eventDescription = obj.description;
            //eventIcons[i].GetComponent<IconScript>().eventCost = obj.actionPoints;
            //eventIcons[i].GetComponent<IconScript>().costField.GetComponent<Text>().text = obj.actionPoints.ToString();
            //eventIcons[i].GetComponent<IconScript>().descriptionField.GetComponent<Text>().text = obj.description;
            //eventIcons[i].SetActive(true);

            //i++;
        }
    }

    public void ClearEventIcons()
    {
        foreach (GameObject obj in eventIcons)
        {
            obj.SetActive(false);
        }
    }

    // get events for this night
    private void GetEvents()
    {
        thisNightEvents.Clear();

        if (firstNight)
        {
            foreach (EventDataScriptableObject obj in allEvents)
            {
                if (obj.eventId == 7)
                {
                    thisNightEvents.Add(obj);
                    Debug.Log("Added event " + obj.eventId);
                    firstNight = false;
                }
            }
        }
        else
        {
            foreach (EventDataScriptableObject obj in allEvents)
            {
                // if mandatory, boolean is true and not done -- these are mandatory and done only once
                if (obj.mandatory && EventManager.eventBooleans[obj.boolListIndex] && !obj.done)
                {
                    thisNightEvents.Add(obj);
                    Debug.Log("Added event " + obj.eventId);
                }
                // these are mandatory and repetable
                else if(obj.mandatory && EventManager.eventBooleans[obj.boolListIndex] && obj.repeatable)
                {
                    thisNightEvents.Add(obj);
                    Debug.Log("Added event " + obj.eventId);
                }
                // if not boolean condition
                else if (obj.boolListIndex == 0)
                {
                    // is repeatable OR not repeatable and not done
                    if (obj.repeatable || (!obj.repeatable && !obj.done))
                    {
                        int rnd = Random.Range(0,3);
                        // 1/3 change to add event
                        if (rnd == 0)
                        {
                            thisNightEvents.Add(obj);
                            Debug.Log("Added event " + obj.eventId);
                        }
                    }
                }
            }
        }
    }

    public void EndNight()
    {
        this.GetComponentInParent<EventManager>().tempTimeLimit--;

        if (this.GetComponentInParent<EventManager>().tempTimeLimit < 1)
            this.GetComponentInParent<EventManager>().GameOver();
        else
        {
            this.GetComponentInParent<EventManager>().playerCharacter.actionPoints = 3;
            this.GetComponentInParent<EventManager>().UpdateHUD();

            MapSetup();
        }
    }

    public void EndFirstNight()
    {
        this.GetComponentInParent<EventManager>().playerCharacter.actionPoints = 3;
        this.GetComponentInParent<EventManager>().UpdateHUD();

        MapSetup();
    }
}
