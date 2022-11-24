using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterCreation : MonoBehaviour
{
    public GameObject characterCreationParent;

    public Dropdown dropdownSect, dropdownClan, dropwdownPredatorType;
    [Tooltip("0 is feminine")]
    public Sprite[] portraits;

    public GameObject attributeParent;
    List<Button> attributeButtons = new List<Button>();
    public Text attributePointsLeftField;
    public GameObject skillParent;
    List<Button> skillButtons = new List<Button>();
    public Text skillPointsLeftField;

    int attributePointsLeft = 3;
    int skillPointsLeft = 6;
    Sect sect;
    Clan clan;
    PredatorType predatorType;
    bool? isFeminine = null;

    public ClanDataScriptableObject clanDataObject;
    public PredatorTypeDataScriptableObject predatorTypeDataObject;

    public Button createCharacterButton;

    private void Awake()
    {
        if (characterCreationParent.activeSelf)
            CharacterCreationSetup();
    }

    private void CharacterCreationSetup()
    {
        // enum dropdown setup
        PopulateDropDownWithEnum(dropdownSect, sect);
        dropdownSect.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdownSect); });//Add listener to Event
        PopulateDropDownWithEnum(dropdownClan, clan);
        dropdownClan.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdownClan); });//Add listener to Event
        PopulateDropDownWithEnum(dropwdownPredatorType, predatorType);
        dropwdownPredatorType.onValueChanged.AddListener(delegate { DropdownValueChanged(dropwdownPredatorType); });//Add listener to Event

        createCharacterButton.interactable = false;

        AttributeButtonSetup();

        SkillButtonSetup();

        UpdateSkillFields(Clan.Brujah);
        UpdatePredatorTypeFields(PredatorType.Siren);
    }

    private void AttributeButtonSetup()
    {
        // get list of all attribute - and + buttons
        foreach (Transform child in attributeParent.transform)
        {
            foreach (Transform secondChild in child)
            {
                if (secondChild.gameObject.CompareTag("Minus"))
                {
                    //Debug.Log("Added " + secondChild.name + " to list");
                    attributeButtons.Add(secondChild.GetComponent<Button>());
                }
                else if (secondChild.gameObject.CompareTag("Plus"))
                {
                    //Debug.Log("Added " + secondChild.name + " to list");
                    attributeButtons.Add(secondChild.GetComponent<Button>());
                }
            }
        }

        // add listeners to attribute buttons
        foreach (Button button in attributeButtons)
        {
            button.onClick.AddListener(delegate { OnClickAttribute(button); });
        }
    }

    private void SkillButtonSetup()
    {
        int i = -1;
        // get list of all skill - and + buttons
        foreach (Transform child in skillParent.transform)
        {
            foreach (Transform secondChild in child)
            {
                if (secondChild.gameObject.CompareTag("FieldName"))
                {
                    secondChild.GetComponent<Text>().text = ((Skill)i).ToString();
                    secondChild.GetComponentInParent<FieldValueScript>().skill = ((Skill)i);
                }
                else if (secondChild.gameObject.CompareTag("Minus"))
                {
                    //Debug.Log("Added " + secondChild.name + " to list");
                    skillButtons.Add(secondChild.GetComponent<Button>());
                }
                else if (secondChild.gameObject.CompareTag("Plus"))
                {
                    //Debug.Log("Added " + secondChild.name + " to list");
                    skillButtons.Add(secondChild.GetComponent<Button>());
                }
            }
            i++;
        }

        // add listeners to skills buttons
        foreach (Button button in skillButtons)
        {
            button.onClick.AddListener(delegate { OnClickSkill(button); });
        }
    }

    private void PopulateDropDownWithEnum(Dropdown dropdown, Enum targetEnum)//You can populate any dropdown with any enum with this method
    {
        Type enumType = targetEnum.GetType();//Type of enum(FormatPresetType in my example)
        List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();

        //newOptions.Add(new Dropdown.OptionData("Select"));

        for (int i = 0; i < Enum.GetNames(enumType).Length; i++)//Populate new Options
        {
            newOptions.Add(new Dropdown.OptionData(Enum.GetName(enumType, i)));
        }

        dropdown.ClearOptions();//Clear old options
        dropdown.AddOptions(newOptions);//Add new options
    }

    private void DropdownValueChanged(Dropdown change)
    {
        if (change == dropdownSect)
            sect = (Sect)change.value; //Convert dropwdown value to enum
        else if (change == dropdownClan)
        {
            clan = (Clan)change.value; //Convert dropwdown value to enum

            UpdateSkillFields(clan);
        }
        else if (change == dropwdownPredatorType)
        {
            predatorType = (PredatorType)change.value;//Convert dropwdown value to enum

            UpdatePredatorTypeFields(predatorType);
        }
    }

    private void UpdatePredatorTypeFields(PredatorType type)
    {
        // reset previous predator temps
        foreach (Transform child in skillParent.transform)
        {
            if (child.gameObject.GetComponent<FieldValueScript>())
            {
                child.gameObject.GetComponent<FieldValueScript>().tempPredator = 0;
                child.gameObject.GetComponent<FieldValueScript>().UpdateField();
            }
        }

        // go though all predator types
        foreach (PredatorTypeData predatorTypeData in predatorTypeDataObject.predatorTypeData)
        {
            // found right predator type data
            if (predatorTypeData.predatorType == type)
            {
                // go through all skill value pairs in predatortype
                foreach (SkillStruct skillStruct in predatorTypeData.skills)
                {
                    // go through all child objects in skillParent
                    foreach (Transform child in skillParent.transform)
                    {
                        if (child.gameObject.GetComponent<FieldValueScript>())
                        {
                            if (child.gameObject.GetComponent<FieldValueScript>().skill == skillStruct.skill)
                            {
                                child.gameObject.GetComponent<FieldValueScript>().tempPredator = skillStruct.value;
                                child.gameObject.GetComponent<FieldValueScript>().UpdateField();
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    private void UpdateSkillFields(Clan clanUpdate)
    {
        // reset previous clan temps
        foreach (Transform child in skillParent.transform)
        {
            if (child.gameObject.GetComponent<FieldValueScript>())
            {
                child.gameObject.GetComponent<FieldValueScript>().tempClan = 0;
                child.gameObject.GetComponent<FieldValueScript>().UpdateField();
            }
        }

        // go though all clans
        foreach (ClanData clanData in clanDataObject.clanData)
        {
            // found right clan data
            if (clanData.clan == clanUpdate)
            {
                Debug.Log("Setting clan: " + clanData.clan.ToString());
                // go through all skill value pairs in clan
                foreach (SkillStruct skillStruct in clanData.skills)
                {
                    // check clan skill against all skill fields
                    foreach (Transform child in skillParent.transform)
                    {
                        if (child.gameObject.GetComponent<FieldValueScript>())
                        {
                            if (child.gameObject.GetComponent<FieldValueScript>().skill == skillStruct.skill)
                            {
                                child.gameObject.GetComponent<FieldValueScript>().tempClan = skillStruct.value;
                                child.gameObject.GetComponent<FieldValueScript>().UpdateField();
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    private void OnClickAttribute(Button button)
    {
        int value = button.GetComponentInParent<FieldValueScript>().value;

        if (button.CompareTag("Minus") && value > 1)
        {
            attributePointsLeft++;
            button.GetComponentInParent<FieldValueScript>().value -= 1;
        }
        else if (button.CompareTag("Plus") && attributePointsLeft > 0 && value < 3)
        {
            attributePointsLeft--;
            button.GetComponentInParent<FieldValueScript>().value += 1;
        }

        button.GetComponentInParent<FieldValueScript>().UpdateField();
        attributePointsLeftField.text = attributePointsLeft.ToString();

        CheckCreateButton();
    }

    private void OnClickSkill(Button button)
    {
        int value = button.GetComponentInParent<FieldValueScript>().value;

        if (button.CompareTag("Minus") && value > 0)
        {
            skillPointsLeft++;
            button.GetComponentInParent<FieldValueScript>().value -= 1;
        }
        else if (button.CompareTag("Plus") && skillPointsLeft > 0)
        {
            skillPointsLeft--;
            button.GetComponentInParent<FieldValueScript>().value += 1;
        }

        button.GetComponentInParent<FieldValueScript>().UpdateField();
        skillPointsLeftField.text = skillPointsLeft.ToString();

        CheckCreateButton();
    }

    private void CheckCreateButton()
    {
        if (attributePointsLeft == 0 && skillPointsLeft == 0 && isFeminine != null)
            createCharacterButton.interactable = true;
        else
            createCharacterButton.interactable = false;
    }

    public void CharacterIsFeminine()
    {
        Debug.Log("feminine");
        isFeminine = true;

        CheckCreateButton();
    }

    public void CharacterIsMasculine()
    {
        Debug.Log("masculine");
        isFeminine = false;

        CheckCreateButton();
    }

    public void CreateCharacter()
    {
        Character character = new Character();

        foreach (Transform child in skillParent.transform)
        {
            if (child.gameObject.GetComponent<FieldValueScript>())
            {
                Skill skill = child.gameObject.GetComponent<FieldValueScript>().skill;
                character.skills[skill] = child.gameObject.GetComponent<FieldValueScript>().value;
                character.skills[skill] += child.gameObject.GetComponent<FieldValueScript>().tempClan;
                character.skills[skill] += child.gameObject.GetComponent<FieldValueScript>().tempPredator;
            }
        }
        if (clan == Clan.Ventrue)
            character.skills[Skill.Resource] = 2;
        else
            character.skills[Skill.Resource] = 0;
        if (predatorType == PredatorType.Bagger)
            character.skills[Skill.Resource] += 2;

        // everyone gets extra 4 Investigation
        character.skills[Skill.Investigation] += 4;

        foreach (Transform child in attributeParent.transform)
        {
            if (child.gameObject.GetComponent<FieldValueScript>())
            {
                Attribute attribute = child.gameObject.GetComponent<FieldValueScript>().attribute;
                character.attributes[attribute] = child.gameObject.GetComponent<FieldValueScript>().value;
            }
        }

        if ((bool)isFeminine)
            character.portrait = portraits[0];
        else
            character.portrait = portraits[1];

        character.sect = sect;
        character.clan = clan;
        character.predatorType = predatorType;

        this.GetComponentInParent<EventManager>().Createcharacter(character);

        characterCreationParent.SetActive(false);
        this.GetComponentInParent<MapManager>().MapSetup();

        Debug.Log("Character created!");
    }
}
