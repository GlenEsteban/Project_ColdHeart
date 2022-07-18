using System;
using UnityEngine;

namespace coldheart_combat {
    public class AbilityRunner : MonoBehaviour {
        IAbility instantAbility;
        IAbility chargeUpAbility;
        public void AssignAbility(IAbility abilityToAdd, abilityTypes abilityType) {
            if (abilityType == abilityTypes.NoAbility) {
                print(gameObject.name + " has an ability with an undetermined type");
            }
            if (abilityType == abilityTypes.InstantAbility) {
                instantAbility = abilityToAdd;
            }
            else if (abilityType == abilityTypes.ChargeUpAbility) {
                chargeUpAbility = abilityToAdd;
            }
        }
        public void UseInstantAbility() {
            if (instantAbility != null) {
                instantAbility.Use(gameObject);
            }
            else {
                print("This character does not have a instant ability");
            }
        }
        public void UseChargedAbility() {
            if (chargeUpAbility != null) {
                chargeUpAbility.Use(gameObject);
            }
            else {
                print("This character does not have a charged ability");
            }
        }
    }
    public interface IAbility {
        void Use(GameObject currentGameObject);
    }
    public enum abilityTypes {
        NoAbility,
        InstantAbility,
        ChargeUpAbility
    }
}