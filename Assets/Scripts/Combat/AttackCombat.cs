using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat
{
    public class AttackCombat : MonoBehaviour, IAction {

        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
       
        
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;

        private void Awake() 
        {
            
        }

        private void Start() 
        {
            EquipWeapon(defaultWeapon);
        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<CharaterMovement>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<CharaterMovement>().Cancel();
                AttackBehavoir();

            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);

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
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return (Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetWeaponRange());
        }

        public bool CanAttack(GameObject attackTarget)
        {
            if (attackTarget == null)
            {
                return false;
            }
            Health targetToTest = attackTarget.GetComponent<Health>();
            return(targetToTest != null && !targetToTest.IsDead());
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();

        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<CharaterMovement>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        //Add animation hit
        void Hit()
        {
            print("tyring to hit");
            if (target == null)
            {
                return;
            }
            
            target.TakeDamage(currentWeapon.GetWeaponDamage());
        }

    }
}