using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ColliderCreator : MonoBehaviour
{
#pragma warning disable CS0414
    [SerializeField] bool updateCollider;
#pragma warning restore CS0414
    [SerializeField] [Range(-1f, 1f)] float topOffset;
    [SerializeField] [Range(-1f, 1f)] float sideOffset;
    [SerializeField] [Range(-1f, 1f)] float bottomOffset;

    private void OnValidate()
    {
        UpdateCollider();
        updateCollider = false;
    }

    private void UpdateCollider()
    {
        Spline spline = GetComponent<SpriteShapeController>().spline;

        int pointCount = spline.GetPointCount();
        Vector2[] colliderPoints = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Vector2 previousPosition = spline.GetPosition((i - 1 + pointCount) % pointCount);
            Vector2 nextPosition = spline.GetPosition((i + 1) % pointCount);
            Vector2 currentPosition = spline.GetPosition(i);

            Vector2 offset1 = GetOffset(previousPosition, currentPosition);
            Vector2 offset2 = GetOffset(currentPosition, nextPosition);

            if (offset1 == Vector2.zero || offset2 == Vector2.zero)
                return;

            colliderPoints[i] = currentPosition + offset1;
            if (offset2 != offset1)
                colliderPoints[i] += offset2;
        }

        GetComponent<PolygonCollider2D>().SetPath(0, colliderPoints);
    }

    Vector2 GetOffset(Vector2 thisPosition, Vector2 nextPosition)
    {
        if (thisPosition.x != nextPosition.x && thisPosition.y != nextPosition.y)
        {
            Debug.Log("Pontos não ortogonais :" + thisPosition.x + " " + nextPosition.x
                + " " + thisPosition.y + " " + nextPosition.y);
            return Vector2.zero;
        }

        if (thisPosition.x > nextPosition.x) // bottom
        {
            return new Vector2(0, -bottomOffset);
        }
        if (thisPosition.x < nextPosition.x) // top
        {
            return new Vector2(0, topOffset);
        }
        if (thisPosition.y < nextPosition.y) // left side
        {
            return new Vector2(-sideOffset, 0);
        }
        if (thisPosition.y > nextPosition.y) // right side
        {
            return new Vector2(sideOffset, 0);
        }

        return Vector2.zero;
    }
}
