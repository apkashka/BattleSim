using UnityEngine;

public class CircleView : MonoBehaviour
{
    public float speed;
    private Circle _circle;

    public void Init(Circle circle)
    {
        _circle = circle;
        GetComponent<MeshRenderer>().material.color = circle.Color == CircleColor.Red ? Color.red : Color.blue;
    }

    public void UpdateData()
    {
        transform.localPosition = _circle.Position;
        transform.localScale = Vector3.one * _circle.Radius*2;
        speed = _circle.Speed;
    }
}
