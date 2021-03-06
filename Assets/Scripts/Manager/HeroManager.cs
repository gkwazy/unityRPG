using UnityEngine;
using RPG.Movement;
using RPG.Fighting;
using System;
using RPG.HealthObject;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Manager
{
    public class HeroManager : MonoBehaviour
    {
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorShape shape;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavmeshProjectionDistance = 3.0f;
        [SerializeField] float raycastRadius = 1f;
      

        private void Awake() 
        {
            health = GetComponent<Health>();
        }


        void Update()
        {
            if(ClickOnUI()) return;
            if(health.Killed()) 
            {
                SetCursor(CursorShape.None);
                return;
            }
            if(ClickOnEnemy()) return;
            if(ClickOnMap()) return;
            SetCursor(CursorShape.None);
        }

        private bool ClickOnEnemy()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                IRaycastable [] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleSpherecast(this))
                    {
                        SetCursor(raycastable.GetShapeOfCursor());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(),raycastRadius);
            float[] distances = new float[hits.Length];
            for(int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }

        private bool ClickOnUI()
        {
           if (EventSystem.current.IsPointerOverGameObject())
           {
               SetCursor(CursorShape.UI);
               return true;
           }
           return false;
        }

        private bool ClickOnMap()
        {
           
            Vector3 hit;
            bool hasHit = RaycastNavMesh(out hit);
            if (hasHit)
            {
                if (!GetComponent<CharaterMovement>().AbleToMove(hit)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<CharaterMovement>().StartMoveAction(hit, 1f);
                }
                SetCursor(CursorShape.Movement);
               return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit,
             maxNavmeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;


            
            return true;
        }


        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorShape type)
        {
            CursorMapping mapping = getCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping getCursorMapping(CursorShape cursorType)
        {
            foreach (CursorMapping map in cursorMappings)
            {
                if (map.shape == cursorType)
                {
                    return map;
                }
            }
            return cursorMappings[0];
        }
    }
}