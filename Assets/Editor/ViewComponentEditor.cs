using Base;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(ViewComponent))]
public class ViewComponentEditor : Editor
{
    private SerializedProperty m_Property;

    private void OnEnable()
    {
        m_Property = serializedObject.FindProperty("Pairs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.skin.textField.alignment = TextAnchor.MiddleCenter;

        int index = -1;
        int length = m_Property.arraySize;
        for (int i = 0; i < length; ++i)
        {
            SerializedProperty pairProperty = m_Property.GetArrayElementAtIndex(i);
            GUILayout.BeginHorizontal();
            {
                SerializedProperty typeProperty = pairProperty.FindPropertyRelative("Type");
                typeProperty.intValue = (int)(ViewComponent.Cop)EditorGUILayout.EnumPopup((ViewComponent.Cop)typeProperty.intValue, GUILayout.Width(100));

                SerializedProperty keyProperty = pairProperty.FindPropertyRelative("Key");
                keyProperty.stringValue = GUILayout.TextField(keyProperty.stringValue, GUILayout.Width(80));

                GUILayout.Label("->", GUILayout.Width(30));

                SerializedProperty valueProperty = pairProperty.FindPropertyRelative("Value");
                valueProperty.objectReferenceValue = EditorGUILayout.ObjectField(valueProperty.objectReferenceValue, GetPairType(typeProperty.intValue), true);

                if (GUILayout.Button("DEL", GUILayout.Width(50)))
                {
                    index = i;
                }

                if (valueProperty.objectReferenceValue != null && valueProperty.objectReferenceValue.GetType() != GetPairType(typeProperty.intValue))
                {
                    valueProperty.objectReferenceValue = null;
                }

                GUILayout.EndHorizontal();
            }
            if (i != length - 1)
            {
                GUILayout.Space(2);
            }
        }

        if (index >= 0)
        {
            m_Property.DeleteArrayElementAtIndex(index);
        }

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("ADD", GUILayout.Height(30)))
            {
                m_Property.InsertArrayElementAtIndex(length);
                SerializedProperty pairProperty = m_Property.GetArrayElementAtIndex(length);

                SerializedProperty typeProperty = pairProperty.FindPropertyRelative("Type");
                typeProperty.intValue = (int)ViewComponent.Cop.GameObject;

                SerializedProperty keyProperty = pairProperty.FindPropertyRelative("Key");
                keyProperty.stringValue = length.ToString();

                SerializedProperty valueProperty = pairProperty.FindPropertyRelative("Value");
                valueProperty.objectReferenceValue = null;
            }

            GUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.skin.textField.alignment = TextAnchor.MiddleLeft;

        serializedObject.ApplyModifiedProperties();
    }

    private System.Type GetPairType(int pair)
    {
        System.Type type = typeof(GameObject);
        if (pair == (int)ViewComponent.Cop.RectTransform)
        {
            type = typeof(RectTransform);
        }
        else if (pair == (int)ViewComponent.Cop.Sprite)
        {
            type = typeof(Sprite);
        }
        else if (pair == (int)ViewComponent.Cop.Text)
        {
            type = typeof(Text);
        }
        else if (pair == (int)ViewComponent.Cop.Image)
        {
            type = typeof(Image);
        }
        else if (pair == (int)ViewComponent.Cop.RawImage)
        {
            type = typeof(RawImage);
        }
        else if (pair == (int)ViewComponent.Cop.Button)
        {
            type = typeof(Button);
        }
        else if (pair == (int)ViewComponent.Cop.Toggle)
        {
            type = typeof(Toggle);
        }
        else if (pair == (int)ViewComponent.Cop.Slider)
        {
            type = typeof(Slider);
        }
        else if (pair == (int)ViewComponent.Cop.Scrollbar)
        {
            type = typeof(Scrollbar);
        }
        else if (pair == (int)ViewComponent.Cop.Dropdown)
        {
            type = typeof(Dropdown);
        }
        else if (pair == (int)ViewComponent.Cop.InputField)
        {
            type = typeof(InputField);
        }
        else if (pair == (int)ViewComponent.Cop.ScrollRect)
        {
            type = typeof(ScrollRect);
        }
        else if (pair == (int)ViewComponent.Cop.Canvas)
        {
            type = typeof(Canvas);
        }
        else if (pair == (int)ViewComponent.Cop.CanvasGroup)
        {
            type = typeof(CanvasGroup);
        }
        return type;
    }
}
