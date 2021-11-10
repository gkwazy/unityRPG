using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavmeshProjectionDistance = 3.0f;
      

        private void Awake() 
        {
            health = GetComponent<Health>();
        }


        void Update()
        {
            if(InteractWithUI()) return;
            if(health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return;
            }
            if(InteractWithCompoment()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithCompoment()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                IRaycastable [] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for(int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithUI()
        {
           if (EventSystem.current.IsPointerOverGameObject())
           {
               SetCursor(CursorType.UI);
               return true;
           }
           return false;
        }

        private bool InteractWithMovement()
        {
           
            Vector3 hit;
            bool hasHit = RaycastNavMesh(out hit);
            if (hasHit)
            {
                if (!GetComponent<CharaterMovement>().CanMoveTo(hit)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<CharaterMovement>().StartMoveAction(hit, 1f);
                }
                SetCursor(CursorType.Movement);
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

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = getCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping getCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping map in cursorMappings)
            {
                if (map.type == cursorType)
                {
                    return map;
                }
            }
            return cursorMappings[0];
        }
    }
}