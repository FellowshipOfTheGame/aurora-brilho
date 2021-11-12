using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
    [Serializable]
    public class ParallaxObject
    {
        public Transform transform;
        [Range(-1,1)] public float speed;
        [DraggablePoint] public Vector3 relativePosition;
    }

    [SerializeField] private List<ParallaxObject> childs = new List<ParallaxObject>();

    private Transform cameraTransform; 

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        foreach (var child in childs)
        {
            PositionObject(child);
        }
    }

    private void PositionObject(ParallaxObject child)
    {
        child.transform.position = new Vector2(
            child.relativePosition.x + (cameraTransform.position.x - child.relativePosition.x) * child.speed,
            child.relativePosition.y + (cameraTransform.position.y - child.relativePosition.y) * child.speed);
    }
}
