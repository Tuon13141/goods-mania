using UnityEngine;
using System;

public class DraggableObject : MonoBehaviour
{
    public Action OnTouchEvent;
    public Action OnDragEvent;
    public Action OnDropEvent;

    private Vector3 _initialPosition;
    private Vector3 _offset;
    private bool _isBeingDragged = false;

    private void Start() => _initialPosition = transform.position;

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

        transform.position = targetPos;
        OnDragEvent?.Invoke();
    }

    public void OnDrop()
    {
        _isBeingDragged = false;
        OnDropEvent?.Invoke();
    }

    public void ResetPosition() => transform.position = _initialPosition;
}