using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera _mainCamera;
    private DraggableObject _currentDraggedObject;
    private DropOnableObject _currentHoveredObject;
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
                    if (_isDragging)
                    {
                        UpdateDrag(touchPos);
                        CheckShelfHover(touchPos);
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    TryDropOnShelf();
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
            if (_isDragging)
            {
                UpdateDrag(Input.mousePosition);
                CheckShelfHover(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            TryDropOnShelf();
            EndDrag();
        }
    }

    private void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out DraggableObject draggable))
            {
                _currentDraggedObject = draggable;
                _isDragging = true;
                draggable.OnTouch(hit.point);
                break; 
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

    private void CheckShelfHover(Vector2 screenPos)
    {
        if (_currentDraggedObject == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(screenPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        DropOnableObject newHovered = null;
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out DropOnableObject dropOn))
            {
                newHovered = dropOn;
                break;
            }
        }

        if (_currentHoveredObject != newHovered)
        {
            if (_currentHoveredObject != null)
            {
                _currentHoveredObject.OnDragExit(_currentDraggedObject);
            }

            _currentHoveredObject = newHovered;

            if (_currentHoveredObject != null)
            {
                _currentHoveredObject.OnDragEnter(_currentDraggedObject);
            }
        }
    }

    private void TryDropOnShelf()
    {
        if (_currentDraggedObject != null && _currentHoveredObject != null)
        {
            _currentHoveredObject.OnDrop(_currentDraggedObject);
        }
    }

    private void EndDrag()
    {
        if (_currentDraggedObject != null)
        {
            _currentDraggedObject.OnDrop();
        }

        if (_currentHoveredObject != null)
        {
            _currentHoveredObject.OnDragExit(_currentDraggedObject);
            _currentHoveredObject = null;
        }
        else if (_currentDraggedObject != null)
        {
            _currentDraggedObject.OnDropedFail();
        }


        _currentDraggedObject = null;
        _isDragging = false;
    }

    public void OnReset()
    {

    }
}