using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class vecs : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public float radius = 0.5f;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector2 center = transform.position;
        Vector2 objPos = a.position;


        float distance = Vector2.Distance(center, objPos);
        bool isInside = distance >= Mathf.Abs(radius);
        
        Handles.color = isInside ? Color.green : Color.red;
        Handles.DrawWireDisc(center, new Vector3(0, 0, 1), radius);
    }
#endif
}
