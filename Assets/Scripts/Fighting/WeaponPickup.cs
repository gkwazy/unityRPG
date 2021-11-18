using System;
using System.Collections;
using System.Collections.Generic;
using RPG.HealthObject;
using RPG.Manager;
using UnityEngine;


namespace RPG.Fighting
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healingPercent = 0;
        [SerializeField] float respawnTime = 5;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject character)
        {
            if (weapon != null)
            {
                character.GetComponent<AttackCombat>().EquipWeapon(weapon);
            }

            if (healingPercent > 0)
            {
                character.GetComponent<Health>().restoreHealth(healingPercent);
            }
            
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
            
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleSpherecast(HeroManager callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if ((Vector3.Distance(gameObject.transform.position, callingController.transform.position) < 2))
                {
                    PickUp(callingController.gameObject);
                    return true;
                }
                else {
                    return false;
                }
            }
            return true;
        }

        public CursorShape GetShapeOfCursor()
        {
            return CursorShape.Pickup;
        }
    }
}
