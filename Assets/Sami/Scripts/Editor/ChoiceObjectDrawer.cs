using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ChoiceObject))]
public class ChoiceObjectDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var padding = 5;
        var height = 18;
        var textHeight = 40;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        // choice text
        EditorGUI.LabelField(new Rect(position.x, position.y + padding, 100, textHeight), "Choice Text");
        EditorGUI.PropertyField(new Rect(position.x + 100, position.y + padding, position.width - 100, textHeight), property.FindPropertyRelative("choiceText"), GUIContent.none);

        // next object index
        EditorGUI.LabelField(new Rect(position.x, position.y + textHeight + padding * 2, 110, height), "Next Object Index");
        EditorGUI.PropertyField(new Rect(position.x + 110, position.y + textHeight + padding * 2, 20, height), property.FindPropertyRelative("nextObjectIndex"), GUIContent.none);

        // is check bool
        EditorGUI.LabelField(new Rect(position.x + 140, position.y + textHeight + padding * 2, 100, height), "Is check");
        EditorGUI.PropertyField(new Rect(position.x + 200, position.y + textHeight + padding * 2, height, height), property.FindPropertyRelative("check"), GUIContent.none);

        // show attributes for check if this is a check choice
        if (property.FindPropertyRelative("check").boolValue)
        {
            // check type
            EditorGUI.LabelField(new Rect(position.x + 230, position.y + textHeight + padding * 2, 100, height), "Check Type");
            EditorGUI.PropertyField(new Rect(position.x + 310, position.y + textHeight + padding * 2, 80, height), property.FindPropertyRelative("checkType"), GUIContent.none);

            // skill or humanity check
            if (property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.skill) || property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.humanity))
            {
                // check difficulty
                EditorGUI.LabelField(new Rect(position.x, position.y + textHeight + height + padding * 3, 60, height), "Difficulty");
                EditorGUI.PropertyField(new Rect(position.x + 60, position.y + textHeight + height + padding * 3, 20, height), property.FindPropertyRelative("difficulty"), GUIContent.none);
            }

            // skill check
            if (property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.skill))
            {
                // attribute used for check
                EditorGUI.LabelField(new Rect(position.x + 90, position.y + textHeight + height + padding * 3, 50, height), "Attribute");
                EditorGUI.PropertyField(new Rect(position.x + 140, position.y + textHeight + height + padding * 3, 70, height), property.FindPropertyRelative("attribute"), GUIContent.none);

                // skill used for check
                EditorGUI.LabelField(new Rect(position.x + 220, position.y + textHeight + height + padding * 3, 30, height), "Skill");
                EditorGUI.PropertyField(new Rect(position.x + 250, position.y + textHeight + height + padding * 3, 90, height), property.FindPropertyRelative("skill"), GUIContent.none);
            }

            // sect check
            if (property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.sect))
            {
                // sect
                EditorGUI.LabelField(new Rect(position.x, position.y + textHeight + height + padding * 3, 60, height), "Sect");
                EditorGUI.PropertyField(new Rect(position.x + 30, position.y + textHeight + height + padding * 3, 80, height), property.FindPropertyRelative("sect"), GUIContent.none);
            }

            // bool check
            if (property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.boolean))
            {
                // bool
                EditorGUI.LabelField(new Rect(position.x, position.y + textHeight + height + padding * 3, 60, height), "Bool Index");
                EditorGUI.PropertyField(new Rect(position.x + 65, position.y + textHeight + height + padding * 3, 30, height), property.FindPropertyRelative("boolIndex"), GUIContent.none);
            }

            // predator type check
            if (property.FindPropertyRelative("checkType").enumValueIndex == ((int)CheckType.predatorType))
            {
                // predator type
                EditorGUI.LabelField(new Rect(position.x, position.y + textHeight + height + padding * 3, 120, height), "Predator Type");
                EditorGUI.PropertyField(new Rect(position.x + 90, position.y + textHeight + height + padding * 3, 100, height), property.FindPropertyRelative("predatorType"), GUIContent.none);
            }

            // hidden
            EditorGUI.LabelField(new Rect(position.x + 400, position.y + textHeight + padding * 2, 100, height), "Hidden");
            EditorGUI.PropertyField(new Rect(position.x + 450, position.y + textHeight + padding * 2, 80, height), property.FindPropertyRelative("hidden"), GUIContent.none);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //return base.GetPropertyHeight(property, label) + 30;
        if (property.FindPropertyRelative("check").boolValue)
            return 100;
        return 70;
    }
}
