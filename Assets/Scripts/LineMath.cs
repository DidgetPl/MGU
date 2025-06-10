using System;
using System.Collections.Generic;
using UnityEngine;

public enum LineType
{
    Regular,
    Vertical
}

public static class LineMath
{
    public static (float, float, LineType) GetPerpendicularLineThroughB(Vector2 A, Vector2 B)
    {
        Vector2 direction = B - A;
        float b_or_const;
        float a;
        Vector2 perp = new Vector2(-direction.y, direction.x);
        LineType lt = LineType.Regular;

        if (direction == Vector2.zero)
        {
            Debug.LogError("Punkty A i B s¹ identyczne – nie mo¿na wyznaczyæ prostej.");
            a = 0;
            b_or_const = 0;
        }
        else if (Mathf.Approximately(perp.x, 0f))
        {
            a = float.PositiveInfinity;
            b_or_const = B.x;
            lt = LineType.Vertical;
        }
        else
        {
            a = perp.y / perp.x;
            b_or_const = B.y - a * B.x;
        }
        return (a, b_or_const, lt);
    }

    public static List<Vector2> GetPointsAlongLine(Vector2 origin, float m, LineType type, float distance)
    {
        if (type == LineType.Vertical)
        {
            Vector2 dir = new Vector2(0, 1);
            Vector2 p1 = origin + dir * distance;
            Vector2 p2 = origin - dir * distance;
            return new List<Vector2>() { p1, p2 };
        }
        else
        {
            float dx = 1f / Mathf.Sqrt(1 + m * m);
            float dz = m * dx;
            Vector2 dir = new Vector2(dx, dz);

            Vector2 p1 = origin + dir * distance;
            Vector2 p2 = origin - dir * distance;
            return new List<Vector2>() { p1, p2 };
        }
    }
}