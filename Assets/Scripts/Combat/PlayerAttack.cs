using UnityEngine;
using RPG.Movement;
using RPG.Core;


namespace RPG.Combat
{
    public class PlayerAttack : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f; 
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;       
        
        Health target;
        float timeSinceLastAttack = 0;

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<CharaterMovement>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<CharaterMovement>().Cancel();
                AttackBehavoir();

            }
        }

        private void AttackBehavoir()
        {
            transform.LookAt(target.transform.position);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will do damage with the Hit trigger from animation
                TriggerAttack();
                timeSinceLastAttack = 0;

            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return (Vector3.Distance(transform.position, target.transform.position) < weaponRange);
        }

        public bool CanAttack(AttackTarget attackTarget)
        {
            if (attackTarget == null)
            {
                return false;
            }
            Health targetToTest = attackTarget.GetComponent<Health>();
            return(targetToTest != null && !targetToTest.IsDead());
        }

        public void Attack(AttackTarget combatTarget)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            target = combatTarget.GetComponent<Health>();

        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        //Add animation hit
        void Hit()
        {
            if (target == null)
            {
                return;
            }
            
            target.TakeDamage(weaponDamage);
        }

    }
}