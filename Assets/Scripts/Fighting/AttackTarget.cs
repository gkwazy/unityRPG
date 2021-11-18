using UnityEngine;
using RPG.HealthObject;
using RPG.Manager;

namespace RPG.Fighting
{
    [RequireComponent(typeof(Health))]
    public class AttackTarget : MonoBehaviour, IRaycastable
    {
        public CursorShape GetShapeOfCursor()
        {
            return CursorShape.Combat;
        }

        public bool HandleSpherecast(HeroManager callingController)
        {
            if (!callingController.GetComponent<AttackCombat>().AbleToFight(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<AttackCombat>().Attack(gameObject);
            }
            return true;
        }
    }
}