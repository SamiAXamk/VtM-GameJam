using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour
{
    public int timeLimit = 10;
    [HideInInspector] public int tempTimeLimit;
    [Tooltip("This doesn't have to actually exist")]
    public int winIndex = 200;
    [Space(20)]
    public GameObject eventParent;
    public GameObject textParent;

    public Text textField;
    public Image eventBackgroundImage;
    public Image leftSideCharacterPortrait;
    private Vector3 leftCharacterPortraitOriginalPosition;
    public Image rightSideCharacterPortrait;
    private Vector3 rightCharacterPortraitOriginalPosition;
    public Button[] choiceButtons;

    [Tooltip("These are the booleans that can define if some events are available")]
    public static bool[] eventBooleans;

    public float portraitMovementTime = 0.75f;
    [Space(20)]
    public int evenDataTestId = 1;

    private Object[] events;                            // array of all events in the game
    private EventDataScriptableObject currentEvent;     // the event that is running
    private bool eventRunning = false;
    private int eventObjectIndex = 0;
    public Character playerCharacter;

    public bool[] hackEventBooleans;

    private List<ChoiceObject> validChoices;

    public Button bloodSurgeButton;
    public bool bloodSurgeUsed = false;
    public Text healthValueField;
    public Text hungerValueField;
    public Text apValueField;
    public Text timeLimitValueField;

    public GameObject statsParent;
    public GameObject statsAttributeParent;
    public GameObject statsSkillParent;
    private bool statsVisible = false;

    public GameObject gameOverObject;
    private bool gameIsOver = false;

    public GameObject mainMenuParent;
    public GameObject credits;
    public GameObject betweenImage;
    public GameObject characterCreationParent;
    public GameObject quitMenuParent;
    public GameObject youWinObject;

    private bool paused = false;

    // for no delay thing
    [SerializeField] GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField] EventSystem m_EventSystem;
    [SerializeField] RectTransform canvasRect;


    private void Start()
    {
        eventBooleans = hackEventBooleans;

        // fill the array with all the events
        events = Resources.LoadAll("EventData");

        // set listeners to all choice buttons - this is done this way for a reason that includes how memory and stack works, ask your coder for details if interested
        int count = 0;
        foreach (Button btn in choiceButtons)
        {
            var current = count;
            choiceButtons[current].onClick.AddListener(() => ButtonClick(current));
            count++;

            btn.gameObject.SetActive(false);
        }

        leftCharacterPortraitOriginalPosition = leftSideCharacterPortrait.rectTransform.position;
        rightCharacterPortraitOriginalPosition = rightSideCharacterPortrait.rectTransform.position;

        // testing
        //StartEvent(evenDataTestId);

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = canvasRect.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse pointer
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                Debug.Log("Hit " + results[0].gameObject.name);
                if (results[0].gameObject.CompareTag("ClickForNext") && eventRunning && currentEvent.eventObjects[eventObjectIndex].type != ObjectType.choice && !paused)
                {
                    NextEvenObject();
                }
            }

        }



        if (!paused)
        {
            if (gameIsOver)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameReset();
                }
            }
            //else if (eventRunning)
            //{
            //    if (Input.GetMouseButtonDown(0) && !statsVisible && currentEvent.eventObjects[eventObjectIndex].type != ObjectType.choice)
            //        StartCoroutine(NextEventClickDelayer());
            //}
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuitMenu();
        }
    }

    private void CheckFontItalic()
    {
        if (currentEvent.eventObjects[eventObjectIndex].isItalic)
            textField.fontStyle = FontStyle.Italic;
        else
            textField.fontStyle = FontStyle.Normal;
    }

    public void Createcharacter(Character character)
    {
        gameIsOver = false;// player can go to new game from game over so reset here
        playerCharacter = character;
        UpdateHUD();
    }

    public void StartEvent(int eventId)
    {
        eventObjectIndex = 0;
        eventParent.SetActive(true);

        currentEvent = FindRightEvent(eventId);
        currentEvent.done = true;
        eventBackgroundImage.sprite = currentEvent.eventBackgroundImage;
        textField.text = currentEvent.eventObjects[eventObjectIndex].text;
        CheckFontItalic();

        CharacterPortraitSetup(0);

        // reset portrait positions
        ResetCharacterPortraits();

        eventRunning = true;
    }

    // find the right event from among all the events
    private EventDataScriptableObject FindRightEvent(int eventId)
    {
        foreach (EventDataScriptableObject obj in events)
        {
            if (obj.eventId == eventId)
                return obj;
        }

        /****************************** FIGURE OUT WHAT TO DO IN THIS CASE, BECAUSE THIS CAN BE GAME BREAKING ******************************/
        // event was not found
        Debug.LogError("Event not found!");
        return null;
    }

    private void NextEvenObject()
    {
        int oldIndex = eventObjectIndex;

        // event end
        if (currentEvent.eventObjects[eventObjectIndex].nextObjectIndex == 0)
        {
            // if event has rewards
            if (currentEvent.eventObjects[eventObjectIndex].hasReward)
            {
                GiveRewards();
            }

            // visit haven first time event
            if (currentEvent.eventId == 7)
            {
                this.gameObject.GetComponent<MapManager>().EndFirstNight();
                UpdateHUD();
            }
            eventParent.SetActive(false);
            eventRunning = false;
        }
        // player has won
        else if (currentEvent.eventObjects[eventObjectIndex].nextObjectIndex == winIndex)
        {
            YouWin();
        }
        // event continue
        else
        {
            // update the index of eventObject
            eventObjectIndex = currentEvent.eventObjects[eventObjectIndex].nextObjectIndex;

            // if event has rewards
            if (currentEvent.eventObjects[eventObjectIndex].hasReward)
            {
                GiveRewards();
            }

            // if the new eventObject is text, update the textField
            if (currentEvent.eventObjects[eventObjectIndex].type == ObjectType.text)
            {
                textField.text = currentEvent.eventObjects[eventObjectIndex].text;
                CheckFontItalic();
            }

            // if the new eventObject is choice object, show choice buttons
            else if (currentEvent.eventObjects[eventObjectIndex].type == ObjectType.choice)
            {
                ShowChoiceButtons();
            }
            CharacterPortraitSetup(oldIndex);
        }
    }

    private void GiveRewards()
    {
        int value = currentEvent.eventObjects[eventObjectIndex].rewardValue;

        if (currentEvent.eventObjects[eventObjectIndex].hasReward)
        {
            switch (currentEvent.eventObjects[eventObjectIndex].rewardType)
            {
                case RewardType.bloodBag:
                    playerCharacter.bloodBags += value;
                    break;
                case RewardType.boolean:
                    eventBooleans[value] = true;
                    break;
                case RewardType.experience:
                    playerCharacter.experience += value;
                    break;
                case RewardType.feeding:
                    if (playerCharacter.hunger > 0)
                        playerCharacter.hunger -= value;
                    if (playerCharacter.hunger < 0)
                        playerCharacter.hunger = 0;
                    UpdateHUD();
                    break;
                case RewardType.skill:
                    foreach (KeyValuePair<Skill, int> obj in playerCharacter.skills)
                    {
                        if (obj.Key == currentEvent.eventObjects[eventObjectIndex].rewardSkill)
                        {
                            playerCharacter.skills[obj.Key] += value;
                            break;
                        }
                    }
                    break;
                case RewardType.healthLoss:
                    playerCharacter.health -= value;
                    UpdateHUD();
                    if (playerCharacter.health <= 0)
                        GameOver();
                    break;
                case RewardType.negativeBool:
                    eventBooleans[value] = false;
                    break;
            }
        }
    }

    private void ShowChoiceButtons()
    {
        bool needBloodSurge = false;
        bloodSurgeButton.gameObject.SetActive(false);

        // gets list of only valid choices (excludes the ones that are hidden and can't be fulfilled by player
        validChoices = GetValidChoices();

        int i = 0;
        // go through all valid choices
        foreach (ChoiceObject obj in validChoices)
        {
            // calculates the Y-position of button
            float tmp = (validChoices.Count / 2 * 100) + 100 - 100 * i;
            // set button position
            choiceButtons[i].GetComponent<RectTransform>().localPosition = new Vector2(0, tmp);
            // set button content
            choiceButtons[i].GetComponentInChildren<Text>().text = validChoices[i].choiceText;
            // set to interactable, in case it was previously set to false
            choiceButtons[i].GetComponent<Button>().interactable = true;
            // set button active
            choiceButtons[i].gameObject.SetActive(true);

            // if this button is a check
            if (validChoices[i].check)
            {
                // skill check
                if (obj.checkType == CheckType.skill)
                {
                    int checkScore = CalculateCheckScore(i);
                    // playerCharacters checkScore is smaller than check difficulty, so can't click button
                    if (checkScore < validChoices[i].difficulty)
                    {
                        choiceButtons[i].GetComponent<Button>().interactable = false;
                        needBloodSurge = true;
                    }
                }
                //// boolean check
                //else if (obj.checkType != CheckType.boolean && eventBooleans[obj.boolIndex])
                //    choiceButtons[i].GetComponent<Button>().interactable = false;
                //// humanity check
                //else if (obj.checkType != CheckType.humanity && obj.difficulty <= playerCharacter.humanity)
                //    choiceButtons[i].GetComponent<Button>().interactable = false;
                //// sect check
                //else if (obj.checkType != CheckType.sect && obj.sect == playerCharacter.sect)
                //    choiceButtons[i].GetComponent<Button>().interactable = false;
                //// predator type check
                //else if (obj.checkType != CheckType.predatorType && obj.predatorType == playerCharacter.predatorType)
                //    choiceButtons[i].GetComponent<Button>().interactable = false;
            }
            i++;
        }

        // bloodsurge can be used on skill
        if (needBloodSurge)
        {
            bloodSurgeButton.interactable = true;

            bloodSurgeButton.gameObject.SetActive(true);

            // too much hunger to use bloodsurge
            if (playerCharacter.hunger >= 4)
                bloodSurgeButton.interactable = false;
        }
    }

    // returns list of all valid choices (ones that should be visible)
    private List<ChoiceObject> GetValidChoices()
    {
        List<ChoiceObject> choiceObjects = new List<ChoiceObject>();
        int index = 0;

        // go through all choices and see if player can complete hidden checks
        // if player can complete hidden check, add choice to list
        foreach (ChoiceObject obj in currentEvent.eventObjects[eventObjectIndex].choices)
        {
            if (obj.hidden)
            {
                // skill check
                if (obj.checkType == CheckType.skill)
                {
                    int checkScore = CalculateCheckScore(index);
                    // player can clear the check, so add choice to list
                    if (checkScore >= currentEvent.eventObjects[eventObjectIndex].choices[index].difficulty)
                        choiceObjects.Add(obj);
                }
                // boolean check
                else if (obj.checkType == CheckType.boolean && eventBooleans[obj.boolIndex])
                    choiceObjects.Add(obj);
                // sect check
                else if (obj.checkType == CheckType.sect && obj.sect == playerCharacter.sect)
                    choiceObjects.Add(obj);
                // predator type check
                else if (obj.checkType == CheckType.predatorType && obj.predatorType == playerCharacter.predatorType)
                    choiceObjects.Add(obj);
            }
            else
                choiceObjects.Add(obj);
            index++;
        }

        return choiceObjects;
    }

    private int CalculateCheckScore(int index)
    {
        // total score for the check
        int checkScore = 0;

        // go through all attributes in playerCharacter
        foreach (KeyValuePair<Attribute, int> attribute in playerCharacter.attributes)
        {
            // if playerCharacter attribute matches the attribute in choice check
            if (attribute.Key == currentEvent.eventObjects[eventObjectIndex].choices[index].attribute)
            {
                checkScore += attribute.Value;
                break;
            }
        }
        // go through all skills in playerCharacter
        foreach (KeyValuePair<Skill, int> skill in playerCharacter.skills)
        {
            // if playerCharacter skill matches the attribute in choice check
            if (skill.Key == currentEvent.eventObjects[eventObjectIndex].choices[index].skill)
            {
                checkScore += skill.Value;
                break;
            }
        }

        return checkScore;
    }

    // Function for buttons
    private void ButtonClick(int index)
    {
        bloodSurgeButton.gameObject.SetActive(false);
        int oldIndex = eventObjectIndex;
        //if (currentEvent.eventObjects[eventObjectIndex].choices[index].check && currentEvent.eventObjects[eventObjectIndex].choices[index].checkType == CheckType.skill)
        //{
        //    Skill checkSkill = currentEvent.eventObjects[eventObjectIndex].choices[index].skill;
        //    Attribute checkAttribute = currentEvent.eventObjects[eventObjectIndex].choices[index].attribute;
        //    int checktDifficulty = currentEvent.eventObjects[eventObjectIndex].choices[index].difficulty;

        //    playerCharacter.skills[checkSkill] = checktDifficulty - playerCharacter.attributes[checkAttribute];
        //}

        // if this button is check and check type is skill -- substract skills from player
        if (validChoices[index].check && validChoices[index].checkType == CheckType.skill)
        {
            Skill checkSkill = validChoices[index].skill;
            Attribute checkAttribute = validChoices[index].attribute;
            int checkDifficulty = validChoices[index].difficulty;

            // bloodsurge was used, so don't reduce skill so much
            if (bloodSurgeUsed)
            {
                int totalScore = checkDifficulty - playerCharacter.attributes[checkAttribute] - 2;
                // reduce skill by remaining score, if this is 0 or negative no change is made
                if (totalScore > 0)
                    playerCharacter.skills[checkSkill] -= totalScore;
                bloodSurgeUsed = false;
            }
            // bloodsurge not used
            else
                playerCharacter.skills[checkSkill] -= checkDifficulty - playerCharacter.attributes[checkAttribute];
        }

        /******************************************************** Make this use NextEventObject() or not? ********************************************************/
        //eventObjectIndex = currentEvent.eventObjects[eventObjectIndex].choices[index].nextObjectIndex;
        eventObjectIndex = validChoices[index].nextObjectIndex;
        textField.text = currentEvent.eventObjects[eventObjectIndex].text;

        GiveRewards();
        CheckFontItalic();

        foreach (Button btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // next object is choice so show buttons
        if (currentEvent.eventObjects[eventObjectIndex].type == ObjectType.choice)
            ShowChoiceButtons();

        //textParent.SetActive(true);

        CharacterPortraitSetup(oldIndex);
    }

    // Handles showing the character portraits on the sides in events
    private void CharacterPortraitSetup(int previousIndex)
    {
        // get the previous evenObject index
        //int previousIndex = currentEvent.eventObjects[eventObjectIndex].previousObjectIndex;

        // if this eventObject has character on the left side
        if (currentEvent.eventObjects[eventObjectIndex].playerCharacter)
        {
            // if the character was not there in the previous eventObject, bring it to screen
            if (!currentEvent.eventObjects[previousIndex].playerCharacter)
            {
                //leftSideCharacterPortrait.sprite = currentEvent.eventObjects[eventObjectIndex].leftSideCharacter;
                leftSideCharacterPortrait.sprite = playerCharacter.portrait;
                leftSideCharacterPortrait.gameObject.SetActive(true);
                StartCoroutine(PortraitMovement(leftSideCharacterPortrait, Direction.right));
            }
        }
        // no character on left side
        else
        {
            // if previous eventObject had a character animate character leaving
            if (currentEvent.eventObjects[previousIndex].playerCharacter)
            {
                StartCoroutine(PortraitMovement(leftSideCharacterPortrait, Direction.left));
            }
            else
                leftSideCharacterPortrait.gameObject.SetActive(false);
        }

        // if this eventObject has character on the right side
        if (currentEvent.eventObjects[eventObjectIndex].rightSideCharacter)
        {
            // if the character was not there in the previous eventObject, bring it to screen
            if (!currentEvent.eventObjects[previousIndex].rightSideCharacter)
            {
                rightSideCharacterPortrait.sprite = currentEvent.eventObjects[eventObjectIndex].rightSideCharacter;
                rightSideCharacterPortrait.gameObject.SetActive(true);
                StartCoroutine(PortraitMovement(rightSideCharacterPortrait, Direction.left));
            }
        }
        // no character on right side
        else
        {
            // if previous eventObject had a character animate character leaving
            if (currentEvent.eventObjects[previousIndex].rightSideCharacter)
            {
                StartCoroutine(PortraitMovement(rightSideCharacterPortrait, Direction.right));
            }
            else
                rightSideCharacterPortrait.gameObject.SetActive(false);
        }
    }

    // takes in the image to move and direction of movement
    private IEnumerator PortraitMovement(Image target, Direction direction)
    {
        float timer = 0;
        float t;
        float targetX = 0;
        Vector3 startPos = target.GetComponent<RectTransform>().position;

        if (direction == Direction.left)
            targetX = startPos.x - target.GetComponent<RectTransform>().rect.width;
        else if (direction == Direction.right)
            targetX = startPos.x + target.GetComponent<RectTransform>().rect.width;

        Vector3 targetPos = new Vector3(targetX, startPos.y, startPos.z);
        Vector3 pos;

        // actual movement
        while (target.GetComponent<RectTransform>().position.x != targetPos.x)
        {
            t = timer / portraitMovementTime;
            pos = Vector3.Lerp(startPos, targetPos, t);
            target.GetComponent<RectTransform>().position = pos;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void MendHealth()
    {
        playerCharacter.MendHealth();
        UpdateHUD();
    }

    public void BloodSurge()
    {
        bloodSurgeUsed = true;
        playerCharacter.hunger++;
        int i = 0;
        // go through all valid choices
        foreach (ChoiceObject obj in validChoices)
        {
            // set to interactable, in case it was previously set to false
            choiceButtons[i].GetComponent<Button>().interactable = true;

            // if this button is a check
            if (validChoices[i].check)
            {
                // skill check
                if (obj.checkType == CheckType.skill)
                {
                    int checkScore = CalculateCheckScore(i) + 2;
                    // playerCharacters checkScore is smaller than check difficulty, so can't click button
                    if (checkScore < validChoices[i].difficulty)
                    {
                        choiceButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
            }
            i++;
        }
        bloodSurgeButton.interactable = false;

        UpdateHUD();
    }

    // updates health, hunger, AP on screen
    public void UpdateHUD()
    {
        healthValueField.text = playerCharacter.health.ToString();
        hungerValueField.text = playerCharacter.hunger.ToString();
        apValueField.text = playerCharacter.actionPoints.ToString();
        timeLimitValueField.text = tempTimeLimit.ToString();
    }

    public void ToggleStats()
    {
        if (statsVisible)
        {
            statsParent.gameObject.SetActive(false);
            statsVisible = !statsVisible;
        }
        else
        {
            int i = 0;
            foreach (Transform child in statsAttributeParent.transform)
            {
                child.gameObject.GetComponent<Text>().text = ((Attribute)i).ToString();
                child.GetChild(0).GetComponent<Text>().text = playerCharacter.attributes[((Attribute)i)].ToString();
                i++;
            }
            i = 0;
            foreach (Transform child in statsSkillParent.transform)
            {
                child.gameObject.GetComponent<Text>().text = ((Skill)i).ToString();
                child.GetChild(0).GetComponent<Text>().text = playerCharacter.skills[((Skill)i)].ToString();
                i++;
            }

            statsParent.gameObject.SetActive(true);
            statsVisible = !statsVisible;
        }
    }

    public void GameOver()
    {
        gameIsOver = true;
        gameOverObject.SetActive(true);
    }

    public void StartGame()
    {
        foreach (EventDataScriptableObject obj in events)
        {
            obj.done = false;
        }
        betweenImage.SetActive(true);
        mainMenuParent.SetActive(false);
        characterCreationParent.SetActive(true);

        tempTimeLimit = timeLimit;
        eventBooleans = hackEventBooleans;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditsToggle()
    {
        if (credits.activeSelf)
        {
            credits.SetActive(false);
        }
        else
        {
            credits.SetActive(true);
        }
    }

    public void BetweenImage()
    {
        betweenImage.SetActive(false);
        mainMenuParent.SetActive(false);
    }

    public void ToggleQuitMenu()
    {
        if (quitMenuParent.activeSelf)
        {
            paused = false;
            quitMenuParent.SetActive(false);
        }
        else
        {
            paused = true;
            quitMenuParent.SetActive(true);
        }
    }

    public void GameReset()
    {
        mainMenuParent.SetActive(true);
        betweenImage.SetActive(false);
        gameOverObject.SetActive(false);
        youWinObject.SetActive(false);
        gameIsOver = false;

        eventParent.SetActive(false);
    }

    public void YouWin()
    {
        youWinObject.SetActive(true);
    }

    private void ResetCharacterPortraits()
    {
        // reset portrait positions
        leftSideCharacterPortrait.rectTransform.position = leftCharacterPortraitOriginalPosition;
        rightSideCharacterPortrait.rectTransform.position = rightCharacterPortraitOriginalPosition;
    }
}
