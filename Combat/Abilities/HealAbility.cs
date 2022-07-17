using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace coldheart_combat {
    public class HealAbility : MonoBehaviour, IAbility {
        [SerializeField] abilityTypes abilityType;
        void Start() {
            GetComponent<AbilityRunner>().AssignAbility(this, abilityType);
        }
        public void Use(GameObject currentGameObject) {
            Debug.Log("Patch up soldier...");
        }
    }
}