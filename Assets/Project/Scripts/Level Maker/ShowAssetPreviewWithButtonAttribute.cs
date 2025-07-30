using NaughtyAttributes;
using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ShowAssetPreviewWithButtonAttribute : DrawerAttribute
{
    public const int DefaultWidth = 64;
    public const int DefaultHeight = 64;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public string[] ButtonLabels { get; private set; }

    public ShowAssetPreviewWithButtonAttribute(int width = DefaultWidth, int height = DefaultHeight, params string[] buttonLabels)
    {
        Width = width;
        Height = height;
        ButtonLabels = buttonLabels;
    }
}


[CustomPropertyDrawer(typeof(ShowAssetPreviewWithButtonAttribute))]
public class ShowAssetPreviewWithButtonPropertyDrawer : PropertyDrawer
{
    private const float ButtonWidth = 80f;
    private const float ButtonHeight = 20f;
    private const float Spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.HelpBox(position, "ShowAssetPreviewWithButton chỉ hoạt động với ObjectReference", MessageType.Error);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // Lấy attribute
        var previewAttr = (ShowAssetPreviewWithButtonAttribute)attribute;

        // Tính toán layout
        float previewWidth = previewAttr.Width;
        float previewHeight = previewAttr.Height;
        float buttonsAreaHeight = (ButtonHeight + Spacing) * previewAttr.ButtonLabels.Length - Spacing;

        // Vẽ field object (dòng đầu tiên)
        Rect propertyRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(propertyRect, property, label);

        // Tính vị trí bắt đầu cho preview và buttons
        float contentY = position.y + EditorGUIUtility.singleLineHeight + Spacing;
        float contentHeight = Mathf.Max(previewHeight, buttonsAreaHeight);

        // Vẽ preview (bên trái)
        if (property.objectReferenceValue != null)
        {
            Texture2D previewTexture = GetAssetPreview(property);
            if (previewTexture != null)
            {
                Rect previewRect = new Rect(
                    position.x,
                    contentY,
                    previewWidth,
                    previewHeight);

                GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit);
            }
        }

        // Vẽ các nút (bên phải, theo hàng dọc)
        float buttonsX = position.x + previewWidth + Spacing * 3;
        for (int i = 0; i < previewAttr.ButtonLabels.Length; i++)
        {
            Rect buttonRect = new Rect(
                buttonsX,
                contentY + (ButtonHeight + Spacing) * i,
                ButtonWidth,
                ButtonHeight);

            if (GUI.Button(buttonRect, previewAttr.ButtonLabels[i]))
            {
                HandleButtonClick(property.serializedObject.targetObject, property.objectReferenceValue as GameObject, i + 1);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var previewAttr = (ShowAssetPreviewWithButtonAttribute)attribute;
        float height = EditorGUIUtility.singleLineHeight + Spacing;

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
        {
            float previewHeight = previewAttr.Height;
            float buttonsHeight = (ButtonHeight + Spacing) * previewAttr.ButtonLabels.Length - Spacing;
            height += Mathf.Max(previewHeight, buttonsHeight);
        }

        return height;
    }

    private Texture2D GetAssetPreview(SerializedProperty property)
    {
        if (property.objectReferenceValue != null)
        {
            return AssetPreview.GetAssetPreview(property.objectReferenceValue);
        }
        return null;
    }

    private void HandleButtonClick(UnityEngine.Object target, GameObject selectedObject, int slotIndex)
    {
        var method = target.GetType().GetMethod("OnAddToSlot");
        if (method != null)
        {
            method.Invoke(target, new object[] { selectedObject, slotIndex });
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy method OnAddToSlot trên {target.GetType().Name}");
        }
    }
}
