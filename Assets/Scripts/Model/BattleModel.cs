using UnityEngine;
using System.Collections.Generic;
public class BattleModel
{
    public List<Circle> CirclesList { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float MaxR { get; private set; }
    public float MinR { get; private set; }
    public float DestroyR { get; private set; }
    public int NumUnitsForColor { get; private set; }
    public float MinSpeed { get; private set; }
    public float MaxSpeed { get; private set; }
    public float SpawnDelay { get; private set; }

    private List<Circle> _redList;
    private List<Circle> _blueList;

    public BattleModel (GameConfigData data) 
    {
        Width = data.GameAreaWidth;
        Height = data.GameAreaHeight;
        NumUnitsForColor = data.NumUnitsToSpawn;
        MinR = data.UnitSpawnMinRadius;
        MaxR = data.UnitSpawnMaxRadius;
        DestroyR = data.UnitDestroyRadius;
        MinSpeed = data.UnitSpawnMinSpeed;
        MaxSpeed = data.UnitSpawnMaxSpeed;
        SpawnDelay = data.UnitSpawnDelay;

        CirclesList = new List<Circle>
        {
            Capacity = NumUnitsForColor * 2
        };
    }
 
    public Circle[] GetCircles => CirclesList.ToArray();
    public void SetCircles(List<Circle> circlesList)
    {
        CirclesList = circlesList;
        _redList = GetColoredCircles(CircleColor.Red);
        _blueList = GetColoredCircles(CircleColor.Blue);
    }


    private List<Circle> GetColoredCircles(CircleColor circleColor)
    {
        var list = new List<Circle>()
        { 
            Capacity = NumUnitsForColor 
        };

        foreach (var circle in CirclesList)
        {
            if(circle.Color == circleColor)
            {
                list.Add(circle);
            }
        }

        return list;
    }
    public float GetSquare(CircleColor color)
    {
        float square = 0;
        switch (color)
        {
            case CircleColor.Red:
                foreach (var circle in _redList)
                {
                   square+= circle.Square;
                }
                break;
            case CircleColor.Blue:
                foreach (var circle in _blueList)
                {
                    square += circle.Square;
                }
                break;
        }
        return square;
    }

    public bool RedAvailable => _redList.Count > 0;
    public bool BlueAvailable => _blueList.Count > 0;
    public void RemoveCircleEveryWhere(Circle circle)
    {
        if (CirclesList.Contains(circle))
        {
            CirclesList.Remove(circle);   
        }
        if (_blueList.Contains(circle))
        {
            _blueList.Remove(circle);
        }
        if (_redList.Contains(circle))
        {
            _redList.Remove(circle);
        }
    }
}
public static class BattleModelExtension
{
    public static float GetRandomRadius(this BattleModel model) => Random.Range(model.MinR, model.MaxR);
    public static float GetRandomSpeed(this BattleModel model) => Random.Range(model.MinSpeed, model.MaxSpeed);
    public static Vector2 GetRandomPosition(this BattleModel model) => new Vector2(Random.Range(0, model.Width), Random.Range(0, model.Height));
    public static Vector2 GetRandomDirection (this BattleModel model) => new Vector2(Random.Range(-1, 1f), Random.Range(-1, 1f));

}