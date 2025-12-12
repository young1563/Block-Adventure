using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapesList;

    private void Start()
    {
        RequestNewShapes();
    }
    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;

    }
    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach(Shape shape in shapesList)
        {
            if(!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }
        
        return null; //선택된 도형이 없음
    }

    private void RequestNewShapes()
    {
        //1. 완전 랜덤 (중복 허용)
        foreach(Shape shape in shapesList)
        {
            int shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }

        //2. 중복 최소화 - TO DO
    }
}
