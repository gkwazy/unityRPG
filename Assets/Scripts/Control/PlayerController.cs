using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {

        void Update()
        {
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
            print("nothing to do");

        }

        private bool InteractWithCombat()
        {
          RaycastHit[] hits =  Physics.RaycastAll(GetMouseRay());
          foreach ( RaycastHit hit in hits)
          {
              AttackTarget target = hit.transform.GetComponent<AttackTarget>();
              if (!GetComponent<PlayerAttack>().CanAttack(target))
              {
                  continue;
              }

              if(Input.GetMouseButtonDown(0))
              {
                  GetComponent<PlayerAttack>().Attack(target);
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
                    GetComponent<CharaterMovement>().StartMoveAction(hit.point);
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