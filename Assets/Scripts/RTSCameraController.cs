using UnityEngine;
using UnityEngine.EventSystems;

public class RTSCameraController : MonoBehaviour
{
    [Header("General")]
    //[SerializeField] Transform _cameraTransform;
    public Transform _followTransform;
    Vector3 _newPosition;
    Vector3 _dragStartPosition;
    Vector3 _dragCurrentPosition;

    [Header("Optional Functionality")]
    [SerializeField] bool _moveWithKeyboad;
    [SerializeField] bool _moveWithEdgeScrolling;
    [SerializeField] bool _moveWithMouseDrag;
    [SerializeField] bool _blockOutsideMouseMoving;

    [Header("Keyboard Movement")]
    [SerializeField] float _fastSpeed = 0.05f;
    [SerializeField] float _normalSpeed = 0.01f;
    [SerializeField] float _movementSensitivity = 1f; // Hardcoded Sensitivity
    float movementSpeed;

    [Header("Edge Scrolling Movement")]
    [SerializeField] float _edgeSize = 50f;
    bool _isCursorSet = false;
    public Texture2D _cursorArrowUp;
    public Texture2D _cursorArrowDown;
    public Texture2D _cursorArrowLeft;
    public Texture2D _cursorArrowRight;

    CursorArrow _currentCursor = CursorArrow.DEFAULT;
    enum CursorArrow
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        DEFAULT
    }

    private void Start()
    {
        _newPosition = transform.position;

        movementSpeed = _normalSpeed;
    }

    private void Update()
    {
        // Allow Camera to follow Target
        if (_followTransform != null)
        {
            transform.position = _followTransform.position;
        }
        // Let us control Camera
        else
        {
            HandleCameraMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _followTransform = null;
        }
    }

    void HandleCameraMovement()
    {
        // Mouse Drag
        if (_moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }

        // Keyboard Control
        if (_moveWithKeyboad)
        {
            if (Input.GetKey(KeyCode.LeftCommand))
            {
                movementSpeed = _fastSpeed;
            }
            else
            {
                movementSpeed = _normalSpeed;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _newPosition += (transform.forward * movementSpeed);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _newPosition += (transform.forward * -movementSpeed);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _newPosition += (transform.right * movementSpeed);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _newPosition += (transform.right * -movementSpeed);
            }
        }

        // Edge Scrolling
        if (_moveWithEdgeScrolling)
        {

            // Move Right
            if (Input.mousePosition.x > Screen.width - _edgeSize)
            {
                _newPosition += (transform.right * movementSpeed);
                ChangeCursor(CursorArrow.RIGHT);
                _isCursorSet = true;
            }

            // Move Left
            else if (Input.mousePosition.x < _edgeSize)
            {
                _newPosition += (transform.right * -movementSpeed);
                ChangeCursor(CursorArrow.LEFT);
                _isCursorSet = true;
            }

            // Move Up
            else if (Input.mousePosition.y > Screen.height - _edgeSize)
            {
                _newPosition += (transform.forward * movementSpeed);
                ChangeCursor(CursorArrow.UP);
                _isCursorSet = true;
            }

            // Move Down
            else if (Input.mousePosition.y < _edgeSize)
            {
                _newPosition += (transform.forward * -movementSpeed);
                ChangeCursor(CursorArrow.DOWN);
                _isCursorSet = true;
            }
            else
            {
                if (_isCursorSet)
                {
                    ChangeCursor(CursorArrow.DEFAULT);
                    _isCursorSet = false;
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementSensitivity);
        if (_blockOutsideMouseMoving)
            Cursor.lockState = CursorLockMode.Confined; // If we have an extra monitor we don't want to exit screen bounds
    }

    private void ChangeCursor(CursorArrow newCursor)
    {
        // Only change cursor if its not the same cursor
        if (_currentCursor != newCursor)
        {
            switch (newCursor)
            {
                case CursorArrow.UP:
                    Cursor.SetCursor(_cursorArrowUp, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.DOWN:
                    Cursor.SetCursor(_cursorArrowDown, new Vector2(_cursorArrowDown.width, _cursorArrowDown.height), CursorMode.Auto); // So the Cursor will stay inside view
                    break;
                case CursorArrow.LEFT:
                    Cursor.SetCursor(_cursorArrowLeft, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.RIGHT:
                    Cursor.SetCursor(_cursorArrowRight, new Vector2(_cursorArrowRight.width, _cursorArrowRight.height), CursorMode.Auto); // So the Cursor will stay inside view
                    break;
                case CursorArrow.DEFAULT:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }

            _currentCursor = newCursor;
        }
    }



    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }
}