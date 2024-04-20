using UnityEngine;

public static class Extensions
{
    public static float GetLastKeyTime(this AnimationCurve curve)
        => curve[curve.length - 1].time;
}
