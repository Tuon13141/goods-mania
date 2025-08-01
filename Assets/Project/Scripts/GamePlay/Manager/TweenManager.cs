using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TweenManager
{
    public static List<Tween> tweens = new List<Tween>();
    public static List<Sequence> sequences = new List<Sequence>();

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

        foreach (var sequence in sequences)
        {
            if (sequence.IsActive() && !sequence.IsComplete())
            {
                sequence.Kill();
            }
        }
        sequences.Clear();
    }
}
