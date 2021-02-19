using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public event System.Action Hided;

    public float Radius { get; private set; }
    public float DestoryRadius { get; private set; }
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; set; }
    public float Speed { get; private set; }
    public CircleColor Color { get; private set; }
    public float Square => Radius >= DestoryRadius? Mathf.PI * Radius * Radius:0;

    public Circle(float radius, float destroyRadius, float speed, Vector2 position,Vector2 direction, CircleColor color)
    {
        Radius = radius;
        Position = position;
        Speed = speed;
        Direction = direction;
        Color = color;
        DestoryRadius = destroyRadius;
    }
    public void CutRadius(float r)
    {
        Radius -= r;
        if (Radius < DestoryRadius)
        {
            Hided?.Invoke();
        }
    }
    public void ReflectX()
    {
       Direction = new Vector2(-Direction.x, Direction.y);
    }
    public void ReflectY()
    {
        Direction = new Vector2(Direction.x, -Direction.y);
    }
    public void Move(float timeStep)
    {
        Position += Direction * Speed * timeStep;
    }
}
public static class CircleExtenstion
{
    public static bool Overlaps(this Circle current, Circle another)
    {
        return Vector2.Distance(current.Position, another.Position) < (current.Radius + another.Radius);
    }
    public static string GetColorName(this Circle circle)
    {
        string name = "Имя цвета не определено";
        switch (circle.Color)
        {
            case CircleColor.Red:
                name = "Красный";
                break;
            case CircleColor.Blue:
                name = "Синий";
                break;
        }
        return name;
    }
}

public class CircleXComparer : IComparer<Circle>
{
    public int Compare(Circle first, Circle second)
    {
        return first.Position.x < second.Position.x ? -1 : (first.Position.x == second.Position.x ? 0 : 1);
    }
}
public class CircleYComparer : IComparer<Circle>
{
    public int Compare(Circle first, Circle second)
    {
        return first.Position.y < second.Position.y ? -1 : (first.Position.y == second.Position.y ? 0 : 1);
    }
}

public enum CircleColor
{
    Red = 0,
    Blue = 1
}