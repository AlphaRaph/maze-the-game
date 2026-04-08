using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Touch detection
    int lookFingerId;

    // Camera control
    Vector2 lookInput;
    public Vector2 LookInput { get { return lookInput; } }

    // Player movement
    public float Horizontal { get { return joystick.Horizontal; } }
    public float Vertical { get { return joystick.Vertical; } }
    public Joystick joystick;
    [SerializeField]
    private RectTransform m_joystickRect;
    [SerializeField]
    private Canvas m_canvas;
    // Start is called before the first frame update
    void Start()
    {
        lookFingerId = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // Handles input
        GetTouchInput();
    }

    private void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            // Check each touch's phase 
            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (lookFingerId == -1 && !IsJoystickCoordinates(t.position))
                    {
                        // Start tracking the right finger if it was not previously being tracked
                        lookFingerId = t.fingerId;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == lookFingerId)
                    {
                        // Stop tracking the right finger 
                        lookFingerId = -1;
                        lookInput = Vector2.zero;
                    }
                    break;
                case TouchPhase.Moved:
                    // Get input for looking around
                    if (t.fingerId == lookFingerId)
                    {
                        lookInput = t.deltaPosition;
                    }
                    break;
                case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                    if (t.fingerId == lookFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }
    private bool IsJoystickCoordinates(Vector2 coordinates)
    {
        float margin = 100;
        float halfWidth = m_joystickRect.rect.width / 2f;
        float halfHeight = m_joystickRect.rect.height / 2f;
        Vector2 myV2 = new Vector2(0, 24);
        Vector2 joystickPosition = new Vector2(300, 300);

        Debug.Log("joystickPosition : " + joystickPosition);
        Debug.Log("fingerPosition : " + coordinates);

        bool result = coordinates.x > joystickPosition.x - halfWidth - margin &&
            coordinates.x < joystickPosition.x + halfWidth + margin &&
            coordinates.y > joystickPosition.y - halfHeight - margin &&
            coordinates.y < joystickPosition.y + halfHeight + margin;

        if (result) Debug.Log("Je suis sur le joystick.");
        return result;
    }
}
