using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _spriteSurface;
    [SerializeField] Transform[] _borders;

    private Camera _cam;
    private float _rotateSpeed = 5;
    private float _zoomSpeed = 10;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsOutSideBounds())
        {
            _cam.fieldOfView +=  _zoomSpeed * Time.deltaTime;
        }
        if (IsLowerSideBounds())
        {
            _cam.fieldOfView -= _zoomSpeed * Time.deltaTime;
        }
    }
    public void Move(float deltaX)
    {
        _cam.transform.RotateAround(_spriteSurface.position, Vector3.forward, deltaX * _rotateSpeed * Time.deltaTime);
    }


    private  float GetMinY()
    {
        float minY = 1;
        for (int i = 0; i<_borders.Length; i++)
        {
            Vector2 viewPos = _cam.WorldToViewportPoint(_borders[i].position);

            if (viewPos.y < minY)
            {
                minY = viewPos.y;
            }
        }
        return minY;
    }
    bool IsOutSideBounds()
    {
        var minY  = GetMinY();
        if (minY < 0.05f)
        {
            Debug.Log(minY);
            return true;
        }
        return false;
    }

    bool IsLowerSideBounds()
    {
        var minY = GetMinY();
        if ( minY > 0.1f)
            return true;
        return false;
    }

}
