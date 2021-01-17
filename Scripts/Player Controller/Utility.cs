using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static float AngleBetweenVectors_rad(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }

    public static float AngleBetweenVectors_deg(Vector3 vec1, Vector3 vec2)
    {
        return AngleBetweenVectors_rad(vec1, vec2) * 180 / Mathf.PI;
    }

    public Vector3 GetLocalDirection(Transform transform, Vector3 destination)
    {
        return transform.InverseTransformDirection((destination - transform.position).normalized);
    }

    public float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        //float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }

    public float signedAngleBetween(Vector3 a, Vector3 b, bool clockwise)
    {
        float angle = Vector3.Angle(a, b);

        //clockwise
        if (Mathf.Sign(angle) == -1 && clockwise)
            angle = 360 + angle;

        //counter clockwise
        else if (Mathf.Sign(angle) == 1 && !clockwise)
            angle = -angle;
        return angle;
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }
}
