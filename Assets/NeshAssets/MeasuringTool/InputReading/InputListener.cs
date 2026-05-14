using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputListener : MonoBehaviour
{
    public event Action<Vector2> OnSelectPoint;
    public event Action OnDeselectLastPoint;
    public event Action OnResetAllPoints;
    public event Action OnNextTool;
    public event Action OnPrevTool;

    private InputAction _selectPoint;
    private InputAction _deselectLastPoint;
    private InputAction _resetAllPoints;
    private InputAction _nextTool;
    private InputAction _prevTool;

    private void OnEnable()
    {
        EnableAllActions();
        if (_isMobile)
        {
            EnhancedTouchSupport.Enable();
        }
    }

    private void OnDisable()
    {
        DisableAllActions();
        if (_isMobile) EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        SetupActions();
        if (_isMobile) MeasDebug.Log("MOBILE controls activated for measuring tools", "InputListener");
        else MeasDebug.Log("DESKTOP controls activated for measuring tools", "InputListener");
    }

    private void Update()
    {
        if (_isMobile)
        {
            HandleMobileInput();  
        }
        else
        {
            HandleDesktopInput();
        }
    }
    private void EnableAllActions()
    {
        _selectPoint?.Enable();
        _deselectLastPoint?.Enable();
        _resetAllPoints?.Enable();
        _nextTool?.Enable();
        _prevTool?.Enable();
    }
    private void DisableAllActions()
    {
        _selectPoint?.Disable();
        _deselectLastPoint?.Disable();
        _resetAllPoints?.Disable();
        _nextTool?.Disable();
        _prevTool?.Disable();
    }
    private void SetupActions()
    {
        _selectPoint = new InputAction("SelectPoint");
        _deselectLastPoint = new InputAction("DeselectLastPoint");
        _resetAllPoints = new InputAction("ResetAllPoints");
        _nextTool = new InputAction("NextTool");
        _prevTool = new InputAction("PrevTool");

        if (!_isMobile)
            BindDesktopKeys();

        EnableAllActions();
    }

    #region Desktop
    private void HandleDesktopInput()
    {
        if (_selectPoint.WasPressedThisFrame()) OnSelectPoint?.Invoke(GetClickPostion());
        else if (_deselectLastPoint.WasPressedThisFrame()) OnDeselectLastPoint?.Invoke();
        else if (_resetAllPoints.WasPressedThisFrame()) OnResetAllPoints?.Invoke();
        else if (_nextTool.WasPressedThisFrame()) OnNextTool?.Invoke();
        else if (_prevTool.WasPressedThisFrame()) OnPrevTool?.Invoke();
    }
    private Vector2 GetClickPostion() => Mouse.current.position.ReadValue();
    private void AddBinding(InputAction action, string binding)
    {
        if (action.bindings.Count == 0) action.AddBinding(binding);
    }
    private void BindDesktopKeys()
    {
        AddBinding(_selectPoint, "<Mouse>/leftButton");
        AddBinding(_deselectLastPoint, "<Mouse>/rightButton");
        AddBinding(_resetAllPoints, "<Keyboard>/r");
        AddBinding(_nextTool, "<Mouse>/scroll/up");
        AddBinding(_prevTool, "<Mouse>/scroll/down");
    }
    #endregion

    #region Mobile
    /*
     * [MOBILE CONTROL SCHEME]
     * Single tap: select point
     * Hold: deselect last point
     * Double tap: reset all points
     * Two finger swipe up: switch to next tool
     * Two finger swip donw: switch to prev tool
     */
    private bool _isMobile = Application.isMobilePlatform || UnityEngine.Device.Application.isMobilePlatform;
    private Vector2 _touchStartPos;
    private float _touchStartTime;
    [SerializeField] private float _holdThreshold = 0.4f;
    [SerializeField] private float _swipeThreshold = 50f;
    private bool _holdFired;

    private void HandleSingleTouchGestures(Touch touch)
    {
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            _touchStartPos = touch.screenPosition;
            _touchStartTime = Time.time;
            _holdFired = false;
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary 
            || touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            bool heldLongEnough = Time.time - _touchStartTime >= _holdThreshold;
            bool notMovedMuch = Vector2.Distance(touch.screenPosition, _touchStartPos) < _swipeThreshold;

            if (heldLongEnough && notMovedMuch && !_holdFired)
            {
                _holdFired = true;
                OnDeselectLastPoint?.Invoke();
                MeasDebug.Log("MOBILE: Point deselected", "InputListener");
            }
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            bool wasQuickTap = Time.time - _touchStartTime < _holdThreshold;
            bool notMovedMuch = Vector2.Distance(touch.screenPosition, _touchStartPos) < _swipeThreshold;

            if (wasQuickTap && notMovedMuch && !_holdFired)
            {
                if (touch.tapCount == 2)
                {
                    OnResetAllPoints?.Invoke();
                    MeasDebug.Log("MOBILE: Points reset", "InputListener");
                }
                else
                {
                    OnSelectPoint?.Invoke(touch.screenPosition);
                    MeasDebug.Log("MOBILE: Point selected", "InputListener");
                }
            }
        }
    }
    private void HandleTwoFingerGestures(Touch touchA, Touch touchB)
    {
        if (touchA.phase == UnityEngine.InputSystem.TouchPhase.Ended
            || touchB.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            float deltaA = touchA.screenPosition.y - touchA.startScreenPosition.y;
            float deltaB = touchB.screenPosition.y - touchB.startScreenPosition.y;
            float averageDelta = (deltaA + deltaB) / 2f;

            if (Mathf.Abs(averageDelta) > _swipeThreshold)
            {
                if (averageDelta > 0)
                {
                    OnNextTool?.Invoke();
                    MeasDebug.Log("MOBILE: next tool", "InputListener");
                }
                else
                {
                    OnPrevTool?.Invoke();
                    MeasDebug.Log("MOBILE: prev tool", "InputListener");
                }
            }
        }
    }
    private void HandleMobileInput()
    {
        //var touches = Touch.activeTouches;
        //if (touches.Count == 1) HandleSingleTouchGestures(touches[0]);
        //else if (touches.Count == 2) HandleTwoFingerGestures(touches[0], touches[1]);
    }

    public void SelectPoint() 
    {
        OnSelectPoint?.Invoke(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }
    public void DeselectPoint() { OnDeselectLastPoint?.Invoke(); }
    public void ResetPoints() { OnResetAllPoints?.Invoke(); }
    public void SwitchNextTool() { OnNextTool?.Invoke(); }
    public void SwitchPrevTool() { OnPrevTool?.Invoke(); }
    #endregion
}
