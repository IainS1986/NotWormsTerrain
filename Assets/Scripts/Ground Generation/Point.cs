using System;
using UnityEngine;

public struct Point
{
    public float X { get; set; }
    public float Y { get; set; }

    public static Point operator +(Point a, Point b)
    {
        return new Point() { X = a.X + b.X, Y = a.Y + b.Y };
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point() { X = a.X - b.X, Y = a.Y - b.Y };
    }

    public static Point operator -(Point a)
    {
        return new Point() { X = -a.X, Y = -a.Y };
    }

    public static Point operator *(Point a, float s)
    {
        return new Point() { X = a.X * s, Y = a.Y * s };
    }

    public static Point Bisect(Point a, Point b)
    {
        Point ab = a + b;
        return Point.Normalize(ab);
    }

    public static Point Normalize(Point a)
    {
        float size = Mathf.Sqrt((a.X * a.X) + (a.Y * a.Y));
        return new Point() { X = a.X / size, Y = a.Y / size };
    }

    public static float Cross(Point a, Point b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
}

