using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TweenManager
{
    public static List<Tween> tweens = new List<Tween>();

    public static void OnReset()
    {
        foreach (var tween in tweens)
        {
            if (tween.IsActive() && !tween.IsComplete())
            {
                tween.Kill();
            }
        }
        tweens.Clear();
    }
}
