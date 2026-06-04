using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(SOBrickModifier))]
public class BrickModifierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Build mapping:
        // controller bool field -> child fields
        var groupMap = new Dictionary<string, List<string>>();

        FieldInfo[] allFields =
            target.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        foreach (var field in allFields)
        {
            var groupAttr = field.GetCustomAttribute<GroupUnderAttribute>();

            if (groupAttr != null)
            {
                if (!groupMap.TryGetValue(groupAttr.BoolField, out var list))
                {
                    list = new List<string>();
                    groupMap[groupAttr.BoolField] = list;
                }

                list.Add(field.Name);
            }
        }

        HashSet<string> drawnChildren = new HashSet<string>();

        SerializedProperty prop = serializedObject.GetIterator();

        if (prop.NextVisible(true))
        {
            do
            {
                // Skip already drawn child properties
                if (drawnChildren.Contains(prop.name))
                    continue;

                // Draw script reference
                if (prop.name == "m_Script")
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(prop, true);
                    GUI.enabled = true;
                    continue;
                }

                // Is this a controller field?
                if (groupMap.TryGetValue(prop.name, out var children))
                {
                    EditorGUILayout.PropertyField(prop, true);

                    // Only draw children if bool enabled
                    if (prop.propertyType == SerializedPropertyType.Boolean &&
                        prop.boolValue)
                    {
                        EditorGUI.indentLevel++;

                        foreach (var childName in children)
                        {
                            SerializedProperty childProp =
                                serializedObject.FindProperty(childName);

                            if (childProp != null)
                            {
                                EditorGUILayout.PropertyField(childProp, true);
                                drawnChildren.Add(childName);
                            }
                        }

                        EditorGUI.indentLevel--;
                    }

                    continue;
                }

                // Skip grouped child fields
                FieldInfo thisField =
                    target.GetType().GetField(
                        prop.name,
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic
                    );

                if (thisField != null &&
                    thisField.GetCustomAttribute<GroupUnderAttribute>() != null)
                {
                    continue;
                }

                // Default draw
                EditorGUILayout.PropertyField(prop, true);

            } while (prop.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();
    }
}