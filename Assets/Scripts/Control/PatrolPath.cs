using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            const float waypointGizmoRadius = 0.3f;
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = getNextWaypoint(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

       public int getNextWaypoint(int i)
        {   
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }
    }
}
