using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Attributes;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        private void Start() 
        {
            health = GetComponent<Health>();
        }


        void Update()
        {
            if(health.IsDead()) return;
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;

        }

        private bool InteractWithCombat()
        {
          RaycastHit[] hits =  Physics.RaycastAll(GetMouseRay());
          foreach ( RaycastHit hit in hits)
          {
              AttackTarget target = hit.transform.GetComponent<AttackTarget>();
              if (target == null)
              {
                  continue;
              }


              if (!GetComponent<AttackCombat>().CanAttack(target.gameObject))
              {
                  continue;
              }

              if(Input.GetMouseButton(0))
              {
                  GetComponent<AttackCombat>().Attack(target.gameObject);
              }
                return true;
          }
          return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<CharaterMovement>().StartMoveAction(hit.point, 1f);
                }
               return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}