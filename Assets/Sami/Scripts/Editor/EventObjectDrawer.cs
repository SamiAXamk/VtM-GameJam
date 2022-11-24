using UnityEngine;
using UnityEditor;


/*
This script defines how the EventObjects should be drawn in the inspector.
EventObjects for one event in the game are in a list in a scriptableobject.
One problem that arose during development was clutter that all the data made,
since all data was always visible even if it wasn't needed.
Custom property drawer draws things that are only necessary, like only draw
choice objects if current EventObject is a choice event.
 */

[CustomPropertyDrawer(typeof(EventObject))]
public class EventObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        property.isExpanded = EditorGUI.Foldout(new Rect(position.xMin, position.yMin, position.width, 16), property.isExpanded, label);

        // Make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        int padding = 3;
        int topClearance = 16;
        int height = 18;
        int fieldIndent = 223;
        int labelWidth = 160;

        if (property.isExpanded)
        {
            // next object index
            EditorGUI.LabelField(new Rect(position.x, position.y + padding + topClearance, labelWidth, height), "Next Object Index");
            EditorGUI.PropertyField(new Rect(position.x + fieldIndent, position.y + padding + topClearance, position.width - fieldIndent, height), property.FindPropertyRelative("nextObjectIndex"), GUIContent.none);

            // previous object index
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 2 + topClearance + height, labelWidth, height), "Previous Object Index");
            EditorGUI.PropertyField(new Rect(position.x + fieldIndent, position.y + padding * 2 + topClearance + height, position.width - fieldIndent, height), property.FindPropertyRelative("previousObjectIndex"), GUIContent.none);

            // type
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 3 + topClearance + height * 2, labelWidth, height), "Type");
            EditorGUI.PropertyField(new Rect(position.x + fieldIndent, position.y + padding * 3 + topClearance + height * 2, position.width - fieldIndent, height), property.FindPropertyRelative("type"), GUIContent.none);

            // text
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 4 + topClearance + height * 3, labelWidth, height), "Text");
            EditorGUI.PropertyField(new Rect(position.x, position.y + padding * 4 + topClearance + height * 3, position.width, height * 4), property.FindPropertyRelative("text"), GUIContent.none);

            // is italic
            EditorGUI.LabelField(new Rect(position.x + 60, position.y + padding * 4 + topClearance + height * 3, labelWidth, height), "Is Italic");
            EditorGUI.PropertyField(new Rect(position.x + 110, position.y + padding * 4 + topClearance + height * 3, 30, height), property.FindPropertyRelative("isItalic"), GUIContent.none);

            // left side character
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 5 + topClearance + height * 7, labelWidth, height), "Player Character");
            EditorGUI.PropertyField(new Rect(position.x + fieldIndent, position.y + padding * 5 + topClearance + height * 7, position.width - fieldIndent, height), property.FindPropertyRelative("playerCharacter"), GUIContent.none);

            // right side character
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 6 + topClearance + height * 8, labelWidth, height), "Right Side Character");
            EditorGUI.PropertyField(new Rect(position.x + fieldIndent, position.y + padding * 6 + topClearance + height * 8, position.width - fieldIndent, height), property.FindPropertyRelative("rightSideCharacter"), GUIContent.none);

            // has reward
            EditorGUI.LabelField(new Rect(position.x, position.y + padding * 7 + topClearance + height * 9, labelWidth, height), "Has Reward");
            EditorGUI.PropertyField(new Rect(position.x + 80, position.y + padding * 7 + topClearance + height * 9, position.width - fieldIndent, height), property.FindPropertyRelative("hasReward"), GUIContent.none);

            if (property.FindPropertyRelative("type").enumValueIndex == ((int)ObjectType.choice))
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y + padding * 8 + topClearance + height * 10, position.width, height), property.FindPropertyRelative("choices"), true);

            }
            if (property.FindPropertyRelative("hasReward").boolValue)
            {
                // reward type
                EditorGUI.LabelField(new Rect(position.x + 110, position.y + padding * 7 + topClearance + height * 9, 100, height), "Reward Type");
                EditorGUI.PropertyField(new Rect(position.x + 195, position.y + padding * 7 + topClearance + height * 9, 100, height), property.FindPropertyRelative("rewardType"), GUIContent.none);

                // reward value
                EditorGUI.LabelField(new Rect(position.x + 300, position.y + padding * 7 + topClearance + height * 9, 100, height), "Reward Value");
                EditorGUI.PropertyField(new Rect(position.x + 385, position.y + padding * 7 + topClearance + height * 9, 40, height), property.FindPropertyRelative("rewardValue"), GUIContent.none);

                if(property.FindPropertyRelative("rewardType").enumValueIndex == ((int)RewardType.skill))
                {
                    // reward value
                    EditorGUI.LabelField(new Rect(position.x + 430, position.y + padding * 7 + topClearance + height * 9, 100, height), "Reward Skill");
                    EditorGUI.PropertyField(new Rect(position.x + 510, position.y + padding * 7 + topClearance + height * 9, 100, height), property.FindPropertyRelative("rewardSkill"), GUIContent.none);
                }
            }
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.FindPropertyRelative("type").enumValueIndex == ((int)ObjectType.choice) && property.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(property) + 20;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == ((int)ObjectType.text) && property.isExpanded)
        {
            return 3 * 8 + 16 + 18 * 10;
        }
        return EditorGUI.GetPropertyHeight(property);
    }
}
