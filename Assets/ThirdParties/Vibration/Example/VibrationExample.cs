﻿////////////////////////////////////////////////////////////////////////////////
//  
// @author Benoît Freslon @benoitfreslon
// https://github.com/BenoitFreslon/Vibration
// https://benoitfreslon.com
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VibrationExample : MonoBehaviour
{
    public Text inputTime;
    public Text inputPattern;
    public Text inputRepeat;
    public InputField inputMinTime;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void TapVibrate ()
    {
        Vibration.Vibrate ();
    }

    public void TapVibrateCustom ()
    {
        Debug.Log ( inputTime.text );
        Vibration.VibrationDuration = long.Parse(inputTime.text);
        Vibration.Vibrate ( int.Parse ( inputTime.text ) );
    }

    public void TapVibratePattern ()
    {
        long[] longs = inputPattern.text.Select ( item => ( long )item ).ToArray ();
        Debug.Log ( longs + " " + int.Parse ( inputRepeat.text ) );
        Vibration.Vibrate ( longs, int.Parse ( inputRepeat.text ) );
    }

    public void TapCancelVibrate ()
    {
        Vibration.Cancel ();
    }

    public void TapPopVibrate ()
    {
        Vibration.VibratePop ();
    }

    public void TapPeekVibrate ()
    {
        Vibration.VibratePeek ();
    }

    public void TapNopeVibrate ()
    {
        if (HapticManager.instance != null)
        {
            HapticManager.instance.StartVibrateRepeat(3);
        }
    }

    public void TapSetMinTime()
    {
        if (HapticManager.instance != null)
        {
            HapticManager.instance.MinTime = float.Parse(inputMinTime.text, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
