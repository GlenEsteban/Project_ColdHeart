using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace coldheart_combat {
    [RequireComponent(typeof(AbilityRunner))]
    public class Combat : MonoBehaviour {
        [SerializeField] [Range (.02f, 10f)] float instantAbilityCoolDown;
        [SerializeField] [Range (.02f, 10f)] float chargeUpAbilityChargeTime;
        float timeSinceLastInstantAbility = Mathf.Infinity;
        bool isChargingUpAbility;
        float timerForChargedAbilityChargeUpTime;
        AbilityRunner abilityRunner;
        public void SetIsChargingUpAbility(bool state) {
            isChargingUpAbility = state;
        } 
        void Awake() {
            abilityRunner = GetComponent<AbilityRunner>();
        }
        void Update() {
            timeSinceLastInstantAbility += Time.deltaTime;
            
            ControlChargedUpAbility();
        }
        public void CallThrottledInstantAbility() {
            if (timeSinceLastInstantAbility >= instantAbilityCoolDown) {
                abilityRunner.UseInstantAbility();
                timeSinceLastInstantAbility = 0;
            }
        }
        public void ControlChargedUpAbility() {
            if(isChargingUpAbility) {
                timerForChargedAbilityChargeUpTime += Time.deltaTime;
                if (timerForChargedAbilityChargeUpTime > chargeUpAbilityChargeTime) {
                    abilityRunner.UseChargedAbility();
                    timerForChargedAbilityChargeUpTime = 0;
                }
            }
            else {
                timerForChargedAbilityChargeUpTime = 0;
            }
        }
        public void CallInstantAbility() {
            // if time since last used ability is greater than ability hold time, use ability.
            abilityRunner.UseInstantAbility();
        }
    }
}
