﻿using UnityEngine;
using System;

public class DraggableObject : MonoBehaviour
{
    public Action OnTouchEvent;
    public Action<Vector3> OnDragEvent;
    public Action OnDropEvent;
    public Action<DropOnableObject> OnDropedOnEvent;
    public Action OnDropedFailEvent;

    private Vector3 _offset;
    private bool _isBeingDragged = false;

    public void OnTouch(Vector3 touchWorldPos)
    {
        _isBeingDragged = true;

        _offset = transform.position - touchWorldPos;
        _offset.z = 0; 

        OnTouchEvent?.Invoke();
    }

    public void OnDrag(Vector3 newWorldPos)
    {
        if (!_isBeingDragged) return;

        Vector3 targetPos = new Vector3(
            newWorldPos.x + _offset.x,
            newWorldPos.y + _offset.y,
            transform.position.z 
        );

        OnDragEvent?.Invoke(targetPos);
    }

    public void OnDrop()
    {
        _isBeingDragged = false;
        OnDropEvent?.Invoke();
    }

    public void OnDropedOn(DropOnableObject dropOnableObject)
    {
        OnDropedOnEvent?.Invoke(dropOnableObject);
    }

    public void OnDropedFail()
    {
        OnDropedFailEvent?.Invoke();
    }
}