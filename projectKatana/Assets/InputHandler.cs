using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    public bool detectSwipeAfterRelease = false;

    public float minValueForSwipe = 20f;

    public static event Action<SwipeData> OnSwipe = delegate { };

    // Update is called once per frame
    void Update()
    {

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            //Detects Swipe while finger is still moving on screen
            if (!detectSwipeAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }

            //Detects swipe after finger is released from screen
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }
        }
    }

    void DetectSwipe()
    {

        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPos.y - fingerUpPos.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                Debug.Log("Vertical Swipe Detected!");
                SendSwipe(direction);

            }
            else
            {
                var direction = fingerDownPos.x - fingerUpPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            fingerUpPos = fingerDownPos;

        }
        else
        {
            //Debug.Log("No Swipe Detected!");
        }
    }

    private void SendSwipe(SwipeDirection swipeDirection)
    {
        SwipeData swipeData = new SwipeData()
        {
            direction = swipeDirection,
            startPosition = fingerDownPos,
            endPosition = fingerUpPos,
        };
        OnSwipe(swipeData);
    }

    float VerticalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    float HorizontalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    bool SwipeDistanceCheckMet()
    {
        return (VerticalMoveValue() > minValueForSwipe || HorizontalMoveValue() > minValueForSwipe);
    }

    bool IsVerticalSwipe()
    {
        return VerticalMoveValue() > HorizontalMoveValue();
    }
}

public struct SwipeData
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public SwipeDirection direction;
}

public enum SwipeDirection
{
    Up,
    Down,   
    Left,
    Right,
}

