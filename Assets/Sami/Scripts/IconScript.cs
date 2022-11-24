using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject gameManager;
    private Character character;

    public int eventId, eventCost;
    public string eventDescription;

    public GameObject descriptionBox;
    public GameObject descriptionField;
    public GameObject costField;

    private bool isOver;

    private void Start()
    {
        character = gameManager.GetComponent<EventManager>().playerCharacter;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionBox.transform.position = this.transform.position;
        descriptionField.GetComponentInChildren<Text>().text = eventDescription;
        costField.GetComponent<Text>().text = eventCost.ToString();
        descriptionBox.SetActive(true);

        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionBox.SetActive(false);

        isOver = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // start event
        if (isOver && character.actionPoints >= eventCost)
        {
            character.actionPoints -= eventCost;
            gameManager.GetComponent<EventManager>().UpdateHUD();

            descriptionBox.SetActive(false);
            this.gameObject.SetActive(false);
            Debug.Log("icon disabled");
            gameManager.GetComponent<EventManager>().StartEvent(eventId);
        }
    }
}
