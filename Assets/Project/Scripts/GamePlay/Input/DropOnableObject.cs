using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnableObject : MonoBehaviour
{
    public Action<DraggableObject> OnDragEnterEvent;
    public Action<DraggableObject> OnDragExitEvent;
    public Action<DraggableObject> OnDropEvent;

    public void OnDragEnter(DraggableObject draggable)
    {
        Debug.Log($"Object {draggable.name} entered shelf {name}");
        OnDragEnterEvent?.Invoke(draggable);
    }

    public void OnDragExit(DraggableObject draggable)
    {
        Debug.Log($"Object {draggable.name} exited shelf {name}");
        OnDragExitEvent?.Invoke(draggable);
    }

    public void OnDrop(DraggableObject draggable)
    {
        Debug.Log($"Object {draggable.name} dropped on shelf {name}");
        OnDropEvent?.Invoke(draggable); 
        //draggable.transform.SetParent(transform);
    }
}
