using UnityEngine;

public class LookTrigger : MonoBehaviour
{
    [Range(0f,1f)]
    public float precission = 0.5f;
    public Transform ObjTf;

    private void OnDrawGizmos()
    {
        Vector2 center = transform.position;
        Vector2 playerPos = ObjTf.position;
        Vector2 playerLookDir = ObjTf.right *-1; //x

        Vector2 playerToTriggerDir = (center - playerPos).normalized;
        float lookness = Vector2.Dot(playerToTriggerDir, playerLookDir);
        bool isLooking = lookness >= precission;

        Gizmos.color = isLooking ? Color.green : Color.red;
        Gizmos.DrawLine(playerPos, playerPos + playerToTriggerDir);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerPos, playerPos + playerLookDir);
    }
}
