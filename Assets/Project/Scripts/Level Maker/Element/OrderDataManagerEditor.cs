using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrderDataManager))]
public class OrderDataManagerEditor : Editor
{
    private OrderDataManager manager;
    private bool[] showOrderElements;

    private void OnEnable()
    {
        manager = (OrderDataManager)target;
        showOrderElements = new bool[manager.orderDataElements.Count];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Vẽ phần items
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Order Data", EditorStyles.boldLabel);

        // Hiển thị nút Add
        if (GUILayout.Button("Add New Order"))
        {
            manager.orderDataElements.Add(new OrderDataElement());
            UpdateShowArray();
        }


        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete All Orders"))
        {
            if (EditorUtility.DisplayDialog("Delete All Orders",
                "Are you sure you want to delete all orders?", "Yes", "Cancel"))
            {
                manager.orderDataElements.Clear();
                UpdateShowArray();
            }
        }

        // Vẽ từng OrderDataElement
        for (int i = 0; i < manager.orderDataElements.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Header với nút Remove
            EditorGUILayout.BeginHorizontal();
            showOrderElements[i] = EditorGUILayout.Foldout(showOrderElements[i], $"Order {i}", true);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                manager.orderDataElements.RemoveAt(i);
                UpdateShowArray();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (showOrderElements[i])
            {
                var element = manager.orderDataElements[i];

                // Nút Add Item
                if (GUILayout.Button("Add Item Selection"))
                {
                    element.itemSelections.Add(new OrderDataElement.ItemSelection());
                }

                // Vẽ từng ItemSelection
                for (int j = 0; j < element.itemSelections.Count; j++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);

                    var selection = element.itemSelections[j];

                    // Hiển thị slider và preview
                    if (manager.items.Count > 0)
                    {
                        // Slider chọn item
                        selection.itemIndex = EditorGUILayout.IntSlider("Item Index",
                            selection.itemIndex, 0, manager.items.Count - 1);

                        // Cập nhật itemId
                        var selectedItem = manager.items[selection.itemIndex];
                        if (selectedItem != null)
                        {
                            var itemData = selectedItem.GetComponent<ItemDataElement>();
                            selection.itemId = itemData != null ? itemData.ItemId : "NO_ID";

                            // Hiển thị preview
                            Texture2D preview = AssetPreview.GetAssetPreview(selectedItem);
                            if (preview != null)
                            {
                                EditorGUILayout.LabelField("Preview:");
                                Rect rect = GUILayoutUtility.GetRect(64, 64);
                                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                            }

                            EditorGUILayout.LabelField($"Item ID: {selection.itemId}");
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No items available", MessageType.Warning);
                    }

                    // Nút Remove
                    if (GUILayout.Button("Remove This Item"))
                    {
                        element.itemSelections.RemoveAt(j);
                        EditorGUILayout.EndVertical();
                        break;
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateShowArray()
    {
        bool[] newShowElements = new bool[manager.orderDataElements.Count];
        for (int i = 0; i < Mathf.Min(showOrderElements.Length, newShowElements.Length); i++)
        {
            newShowElements[i] = showOrderElements[i];
        }
        showOrderElements = newShowElements;
    }
}