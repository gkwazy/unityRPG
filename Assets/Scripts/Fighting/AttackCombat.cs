using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.HealthObject;
using RPG.Stats;
using System.Collections.Generic;
using RPG.DeveloperTools;
using UnityEngine.Events;

namespace RPG.Fighting
{
    public class AttackCombat : MonoBehaviour, IMode, ISaveable, IModifierProvider {

        
      
        [SerializeField] Transform rightItemTransform = null;
        [SerializeField] Transform leftItemTransform = null;
        [SerializeField] float defaultSwingTimer = 1f;
        [SerializeField] Boolean usingWeapons;
        [SerializeField] UnityEvent noWeaponSound;
        [SerializeField] WeaponConfig startingWeapon = null;
        
        Health target;
        float swingTimer = Mathf.Infinity;
        WeaponConfig equippedWeaponHandler;
        SlowLoad<Weapon> equippedWeapon;

        private void Awake() {
            equippedWeaponHandler = startingWeapon;
            equippedWeapon = new SlowLoad<Weapon>(SetStartingWeapon);
        }

        private void Start() 
        {
            equippedWeapon.Initialize();
        }

        private void Update() {

            swingTimer += Time.deltaTime;

            if (target == null) return;

            if (target.Killed()) return;

            if (!InRangeForAttack(target.transform))
            {
                GetComponent<CharaterMovement>().Move(target.transform.position, 1f);
            }
            else
            {
                GetComponent<CharaterMovement>().Cancel();
                FightingAI();

            }
        }

        private Weapon SetStartingWeapon()
        {
          return BindWeapon(startingWeapon);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            equippedWeaponHandler = weapon;
            equippedWeapon.value = BindWeapon(weapon);

        }

        private Weapon BindWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightItemTransform, leftItemTransform, animator);
        }

        public Health GetTarget()
        {
             return target;
        }

        private void FightingAI()
        {
            transform.LookAt(target.transform.position);

            if (swingTimer > defaultSwingTimer)
            {
                // This will do damage with the Hit trigger from animation
                ResetAttack();
                swingTimer = 0;

            }

        }

        private void ResetAttack()
        {

            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool InRangeForAttack(Transform targetTransform)
        {
            return (Vector3.Distance(transform.position, targetTransform.position) < equippedWeaponHandler.GetWeaponRange());
        }

        public bool AbleToFight(GameObject attackTarget)
        {
            
            if (attackTarget == null)
            {
                return false;
            }
            if (!GetComponent<CharaterMovement>().AbleToMove(attackTarget.transform.position) &&
                !InRangeForAttack(attackTarget.transform)) 
            {
                return false;
            }
            Health wantedTarget = attackTarget.GetComponent<Health>();
            return(wantedTarget != null && !wantedTarget.Killed());
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionContoller>().StartAction(this);
            target = combatTarget.GetComponent<Health>();

        }

        public void Cancel()
        {
            StopCombat();
            target = null;
            GetComponent<CharaterMovement>().Cancel();
        }

        private void StopCombat()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        //Add animation hit
        void Hit()
        {
            if (target == null)
            {
                return;
            }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            
            if (equippedWeapon.value != null)
            {
               equippedWeapon.value.OnHit(); 
            }
            else
            {
                noWeaponSound.Invoke();
            }

            if (equippedWeaponHandler.HasProjectile())
            {
                equippedWeaponHandler.LaunchProjectile(rightItemTransform,leftItemTransform, target, gameObject, damage);
            }
            else
            {
                target.DamageHealth(gameObject, damage);
            }
        }

        public object GetWeaponState()
        {
            return equippedWeaponHandler.name;
        }

        public void RestoreWeaponState(object state)
        {
            string startingWeaponName = (string) state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(startingWeaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetBonusAdditive(Stat stat)
        {
           if (stat == Stat.Damage)
           {
               yield return equippedWeaponHandler.GetWeaponDamage();
           }
        }

        public IEnumerable<float> GetBonusPercentage(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return equippedWeaponHandler.GetPercentageBonus();
            }
        }
    }
}