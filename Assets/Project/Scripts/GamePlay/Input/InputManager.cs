using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private Camera _mainCamera;
    private DraggableObject _currentDraggedObject;
    private bool _isDragging = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Mobile Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryStartDrag(touchPos);
                    break;
                case TouchPhase.Moved:
                    if (_isDragging) UpdateDrag(touchPos);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDrag();
                    break;
            }
        }
        // Mouse (Editor)
        else if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (_isDragging) UpdateDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out DraggableObject draggable))
            {
                _currentDraggedObject = draggable;
                _isDragging = true;
                draggable.OnTouch(hit.point);
            }
        }
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        if (_currentDraggedObject == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 newPos = hit.point;
            _currentDraggedObject.OnDrag(newPos);
        }
    }

    private void EndDrag()
    {
        if (_currentDraggedObject != null)
        {
            _currentDraggedObject.OnDrop();
        }
        _currentDraggedObject = null;
        _isDragging = false;
    }
}