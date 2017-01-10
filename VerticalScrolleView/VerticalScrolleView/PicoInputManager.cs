using UnityEngine;
using System.Collections;

public class PicoInputManager : MonoBehaviour
{
    public static event System.Action OnUp = delegate { };
    public static event System.Action OnDown = delegate { };
    public static event System.Action OnLeft = delegate { };
    public static event System.Action OnRight = delegate { };

    bool isLeftButtonDown = false;
    bool isRightButtonDown = false;
    bool isUpButtonDown = false;
    bool isDownButtonDown = false;
    float StartTime = 0.0f;
    float Horizontal = 0.0f;
    float Vertical = 0.0f;

    private static PicoInputManager _instance = null;

    public static void Init()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("PicoInputManager");
            _instance = go.AddComponent<PicoInputManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        Horizontal = Input.GetAxis(InputUtil.Horizontal);
        Vertical = Input.GetAxis(InputUtil.Vertical);
#endif
        EmulateButton();
    }

    void FixedUpdate()
    {
        //right button
        CheckButtonPositive(Horizontal, ref isRightButtonDown, 0.9f, 0.1f, OnRight);
        //
        CheckButtonPositive(Vertical, ref isUpButtonDown, 0.9f, 0.1f, OnUp);
        //right button
        CheckButtonNegative(Horizontal, ref isLeftButtonDown, -0.9f, -0.1f, OnLeft);
        //
        CheckButtonNegative(Vertical, ref isDownButtonDown, -0.9f, -0.1f, OnDown);
    }

    public void CheckButtonPositive(float horv, ref bool isDown, float max, float min, System.Action downEvent)
    {
        if (horv > max)
        {
            if (isDown == false)
            {
                StartTime = Time.time;
                downEvent();
            }
            isDown = true;
            if (isDown)
            {
                if (Time.time - StartTime > 0.4f)
                {
                    isDown = false;
                }
            }
        }
        if (horv < min)
        {
            isDown = false;
        }
    }
    public void CheckButtonNegative(float horv, ref bool isDown, float max, float min, System.Action downEvent)
    {
        if (horv < max)
        {
            if (isDown == false)
            {
                StartTime = Time.time;
                downEvent();
            }
            isDown = true;
            if (isDown)
            {
                if (Time.time - StartTime > 0.4f)
                {
                    isDown = false;
                }
            }
        }
        if (horv > min)
        {
            isDown = false;
        }
    }
    public void EmulateButton()
    {
        //emulate rihgt button
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Horizontal = 1.0f;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Horizontal = 0.0f;
        }

        //emulate left button
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Horizontal = -1.0f;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Horizontal = 0.0f;
        }

        //emulate up button
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vertical = 1.0f;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Vertical = 0.0f;
        }

        //emulate down button
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vertical = -1.0f;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Vertical = 0.0f;
        }
    }
}