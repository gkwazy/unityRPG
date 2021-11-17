using UnityEngine;
using RPG.HealthObject;
using RPG.Control;

namespace RPG.Fighting
{
    [RequireComponent(typeof(Health))]
    public class AttackTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetShapeOfCursor()
        {
            return CursorType.Combat;
        }

        public bool HandleSpherecast(HeroController callingController)
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