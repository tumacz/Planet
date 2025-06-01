using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceTransform : MonoBehaviour
{
    //public Vector2 localSpacePoint;
    public Vector2 worldSpacePoint;
    public Transform obj;

    private void OnDrawGizmos()
    {
        Vector2 pos = transform.position;
        Vector2 right = transform.right;
        Vector2 up = transform.up;

        //Vector2 LocalToWorld(Vector2 localPt)
        //{
        //    Vector2 worldOffset = right * localPt.x + up * localPt.y;
        //    return (Vector2)transform.position + worldOffset;
        //}

        Vector2 WorldToLocal(Vector2 worldPt)
        {
            Vector2 relPoint = worldPt - pos;
            float x = Vector2.Dot(relPoint, right);
            float y = Vector2.Dot(relPoint, up);
            return new Vector2(x, y);
        } 


        DrawBasisVectors(pos, right, up);
        DrawBasisVectors(Vector2.zero, Vector2.right, Vector2.up);

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(LocalToWorld(localSpacePoint), 0.05f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(worldSpacePoint, 0.05f);

        obj.localPosition = WorldToLocal(worldSpacePoint);
        Console.WriteLine(obj.localPosition);
    }

    void DrawBasisVectors(Vector2 pos, Vector2 right, Vector2 up)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pos, right);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, up);
        Gizmos.color = Color.white;
    }
}
