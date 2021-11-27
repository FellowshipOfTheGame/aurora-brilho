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
    
    [SerializeField] [Range(-1f, 1f)] float offset;

    private void OnValidate()
    {
        UpdateCollider();
        updateCollider = false;
    }

    private void UpdateCollider()
    {
        Spline spline = GetComponent<SpriteShapeController>().spline;

        List<Vector2> colliderPoints = new List<Vector2>();

        int pointCount = spline.GetPointCount();
        for (int i = 0; i < pointCount; i++)
        {
            Vector2 previousPosition = spline.GetPosition((i - 1 + pointCount) % pointCount);
            Vector2 currentPosition = spline.GetPosition(i);
            Vector2 nextPosition = spline.GetPosition((i + 1) % pointCount);

            Vector2 a = (previousPosition - currentPosition).normalized;
            Vector2 b = (nextPosition - currentPosition).normalized;

            Vector2 bisector = (a + b).normalized;
            if (bisector == Vector2.zero)
                continue;

            float midAngle = -Vector2.SignedAngle(a, b);
            if (midAngle < 0) midAngle += 360;
            midAngle = (midAngle / 2) * Mathf.Deg2Rad;

            if (Vector2.SignedAngle(a, bisector) < 0)
            {
                colliderPoints.Add(currentPosition + (bisector * (offset / Mathf.Sin(midAngle))));
            }
            else
            {
                colliderPoints.Add(currentPosition - (bisector * (offset / Mathf.Sin(midAngle))));
            }
        }

        GetComponent<PolygonCollider2D>().SetPath(0, colliderPoints);
    }

    /*private void UpdateCollider2()
    {
        Spline spline = GetComponent<SpriteShapeController>().spline;

        List<Vector2> colliderPoints = new List<Vector2>();

        int pointCount = spline.GetPointCount();
        for (int i = 0; i < pointCount; i++)
        {
            Vector2 previousPosition = spline.GetPosition((i - 1 + pointCount) % pointCount);
            Vector2 currentPosition = spline.GetPosition(i);
            Vector2 nextPosition = spline.GetPosition((i + 1) % pointCount);

            Vector2 a = previousPosition - currentPosition;
            Vector2 b = nextPosition - currentPosition;


            // varying offsets
            float d1 = (a.magnitude > 30) ? offset1 : offset2;
            float d2 = (b.magnitude > 30) ? offset1 : offset2;
            Debug.Log(d1 + " " + d2);
            float theta = Vector2.SignedAngle(a, b);
            if (theta < 0) theta += 360;
            theta *= Mathf.Deg2Rad;
            float theta_2 = Mathf.Atan((d2 * Mathf.Sin(theta)) / (d1 + d2 * Mathf.Cos(theta)));

            Vector2 new_vector = new Vector2(Mathf.Cos(theta_2) * a.x - Mathf.Sin(theta_2) * a.y,
                                             Mathf.Sin(theta_2) * a.x + Mathf.Cos(theta_2) * a.y);

            colliderPoints.Add(currentPosition - (new_vector.normalized * (d2 / Mathf.Sin(theta_2))));
        }

        GetComponent<PolygonCollider2D>().SetPath(0, colliderPoints);
    }*/

    float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        // The following determinant formula gives twice the (signed) area of the triangle a->b->c
        // If the area is positive, then a->b->c is counterclockwise
        // if the area is negative, then a->b->c is clockwise
        // if the area is zero then a->b->c are collinear
        return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
    }
}
