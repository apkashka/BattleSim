using UnityEngine;
using System.Collections.Generic;

public class BattleController 
{
    BattleModel Model { get; set; }
    IComparer<Circle> _xComparer;
    private List<Circle> _circlesFiringList;
    private float _simulationSpeed;
    private float _simulationTime;

    public event System.Action<float, string> SimulationEnded;
    public BattleController(BattleModel model, float simulationSpeed = 1)
    {
        Model = model;
        _circlesFiringList = new List<Circle>();
        _xComparer = new CircleXComparer();
        _simulationSpeed = simulationSpeed;
    }

    public void Init()
    {
        var circlesList = new List<Circle>();
        var color = CircleColor.Blue;

        Debug.Log("initiation started");
        while (circlesList.Count < 3)
        {
            var newCircle = GetNewCircle(color);
            if (OverlapsBorder(newCircle))
            {
                continue;
            }
            bool overlaps = false;
            foreach (var circle in circlesList)
            {
                if (!newCircle.Overlaps(circle))
                {
                    continue;
                }
                overlaps = true;
                break;
            }

            if (!overlaps)
            {
                color = color == CircleColor.Red ? CircleColor.Blue : CircleColor.Red;
                circlesList.Add(newCircle);
            }
        }

        Debug.Log("3 elements created");
        int falseChecks = 0;

        int count = Model.NumUnitsForColor * 2;
        while (circlesList.Count < count && falseChecks < 1000)
        {
            var newCircle = GetNewCircle(color);
            falseChecks++;
            bool overlaps = !CheckArea(newCircle, circlesList);
            if (!overlaps)
            {
                circlesList.Add(newCircle);
                color = color == CircleColor.Red ? CircleColor.Blue : CircleColor.Red;
                falseChecks = 0;
            }
        }
        Debug.Log($" circle list count:{circlesList.Count}");
        Debug.Log("Circles created hmm");

        Model.SetCircles(circlesList);
    }

    private Circle GetNewCircle(CircleColor color)
    {
        return new Circle(Model.GetRandomRadius(), Model.DestroyR, 
                          Model.GetRandomSpeed(), Model.GetRandomPosition(),
                          Model.GetRandomDirection(), color);
    }

    private void SwapDirections(Circle one, Circle another)
    {
        Vector2 direction = one.Direction;
        one.Direction = another.Direction;
        another.Direction = direction;
    }

    private bool CheckArea(Circle newCircle,List<Circle> circlesList)
    {
        if (OverlapsBorder(newCircle))
            return false;

        circlesList.Sort(_xComparer);
        int result = BynaryIdSearch(circlesList, newCircle);

        if (result == -1)
            return true;

        for (int i = result; i < circlesList.Count; i++)
        {
            if (circlesList[i].Overlaps(newCircle))
            {
                return false;
            }
        }
        return true;
    }

    private int BynaryIdSearch(List<Circle> array, Circle circle)
    {
        int left = 0;
        int right = array.Count;
        int middle;

        float hypotheticalMaxR = circle.Radius + Model.MaxR;
        float searchedValue = circle.Position.x;

        while (left <= right)
        {
            middle = (left + right) / 2;

            bool weAreClose = Mathf.Abs(array[middle].Position.x) - searchedValue < hypotheticalMaxR;
            if (weAreClose)
            {
                for (int i = middle; i >= 0; i--)
                {
                    if (i == 0 || searchedValue - array[i].Position.x > hypotheticalMaxR)
                    {
                        return i;
                    }
                }
            }

            if (array[middle].Position.x > searchedValue)
            {
                if (middle == 0)
                {
                    return 0;
                }
                right = middle - 1;
            }
            else
            {
                if (middle == array.Count - 1)
                {
                    return middle;
                }
                left = middle;
            }
        }
        return -1;
    }
    private bool OverlapsBorder(Circle circle)
    {
        return circle.Position.x + circle.Radius > Model.Width ||
               circle.Position.x - circle.Radius < 0 ||
               circle.Position.y + circle.Radius > Model.Height ||
               circle.Position.y - circle.Radius < 0;
    }

    public void UpdateSimulation(float timeStep)
    {
        _simulationTime += timeStep;
        timeStep *= _simulationSpeed;

        for (int i = 0; i < Model.CirclesList.Count; i++)
        {
            Model.CirclesList[i].Move(timeStep);
        }

        Model.CirclesList.Sort(_xComparer);

        for (int i = 0; i < Model.CirclesList.Count; i++)
        {
            var current = Model.CirclesList[i];
            int maxId = i;
            while (maxId < Model.CirclesList.Count)
            {
                if (Model.CirclesList[maxId].Position.x - current.Position.x > current.Radius + Model.MaxR)
                {
                    break;
                }
                maxId++;
            }

            for (int j = i + 1; j < maxId; j++)
            {
                var next = Model.CirclesList[j];

                if (current.Overlaps(next))
                {
                    if (current.Color != next.Color)
                    {
                        var delta = (current.Radius + next.Radius - Vector2.Distance(current.Position, next.Position)) / 2;
                        current.CutRadius(delta);
                        next.CutRadius(delta);
                        if (current.Radius < 0.2f)
                        {
                            if (!_circlesFiringList.Contains(current))
                                _circlesFiringList.Add(current);
                        }
                        if (next.Radius < 0.2f)
                        {
                           if (!_circlesFiringList.Contains(next))
                                _circlesFiringList.Add(next);
                        }
                        continue;
                    }


                    current.Move(-timeStep);
                    next.Move(-timeStep);
                    SwapDirections(current, next);

                    if ((current.Direction.x > 0 && next.Direction.x <= 0 ||
                         current.Direction.x < 0 && next.Direction.x >= 0) &&
                        (current.Direction.y > 0 && next.Direction.y <= 0 ||
                         current.Direction.y < 0 && next.Direction.y >= 0))
                    {
                        continue;
                    }
                    bool currentFasterX = current.Direction.x * current.Speed > next.Direction.x * next.Speed;
                    bool currentFasterY = current.Direction.y * current.Speed > next.Direction.y * next.Speed;

                    if (currentFasterX)
                    {
                        if (current.Position.x < next.Position.x)
                        {
                            current.ReflectX();
                        }
                        else
                        {
                            next.ReflectX();
                        }
                    }

                    if (currentFasterY)
                    {
                        if (current.Position.y < next.Position.y)
                        {
                            current.ReflectY();
                        }
                        else
                        {
                            next.ReflectY();
                        }
                    }
                }
            }

            if (OverlapsBorder(current) && !_circlesFiringList.Contains(current))
            {
                bool overlapsX = current.Position.x + current.Radius > Model.Width ||
                                 current.Position.x < current.Radius;
                bool overlapsY = current.Position.y + current.Radius > Model.Height ||
                                 current.Position.y < current.Radius;

                if (overlapsX)
                {
                    current.ReflectX();
                }
                if (overlapsY)
                {
                    current.ReflectY();
                }
                current.Move(timeStep);
            }
        }

        foreach (var circle in _circlesFiringList)
        {
            Model.RemoveCircleEveryWhere(circle);
        }
        _circlesFiringList.Clear();

        if (!Model.RedAvailable||!Model.BlueAvailable)
        {
            SimulationEnded.Invoke(_simulationTime, Model.CirclesList[0].GetColorName());
        }
    }

    public void ChangeSimulationSpeed(float speed)
    {
        _simulationSpeed = speed;
    }
}
