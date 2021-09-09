using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class PlayerAttack : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f; 
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;       
        
        Transform target;
        float timeSinceLastAttack = 0;

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (!GetIsInRange())
            {
                GetComponent<CharaterMovement>().MoveTo(target.position);
            }
            else
            {
                GetComponent<CharaterMovement>().Cancel();
                AttackBehavoir();

            }
        }

        private void AttackBehavoir()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will do damage with the Hit trigger from animation
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
                
            }
           
        }

        private bool GetIsInRange()
        {
            return (Vector3.Distance(transform.position, target.position) < weaponRange);
        }

        public void Attack(AttackTarget combatTarget)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            target = combatTarget.transform;

        }

        public void Cancel()
        {
            target = null;
        }

        //Add animation hit
        void Hit()
        {
            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

    }
}