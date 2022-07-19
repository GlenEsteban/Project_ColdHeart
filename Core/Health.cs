using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace coldheart_core {
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth;
        [SerializeField] float currentHealth;
        [SerializeField] float invincibilityTime;
        [SerializeField] bool isInvincible;
        [SerializeField] float deathEventTime = 2f;
        CharacterManager characterManager;
        float timerForInvincibility;
        public event Action onCharacterDeath;
        public float GetCurrentHealth() {
            return currentHealth;
        }
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
        }
        IEnumerator Start() {
            ResetHealth();

            while (true) {
                yield return new WaitForSeconds(1f);
                ReduceHealth(3f);
            }
        }
        void Update() {
            ControlInvinciblity();
        }
        void ResetHealth() {
            currentHealth = maxHealth;
        }
        public void ReduceHealth(float damageDealt) {
            if (isInvincible) {return;}

            currentHealth = Mathf.Max(currentHealth -damageDealt, 0);
            if (currentHealth == 0) {
                if (onCharacterDeath != null) {
                    HandleCharacterDeathEvent();
                }
            }

            GainInvincibility(invincibilityTime);
        }
        public void RestoreHealth(float healAmount) {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        }
        public void GainInvincibility(float time) {
            isInvincible = true;
            timerForInvincibility = time;
        }
        void ControlInvinciblity() {
            if (isInvincible) {
                timerForInvincibility -= Time.deltaTime;
                if (timerForInvincibility <= 0) {
                    isInvincible = false;
                    timerForInvincibility = 0;
                }
            }
        }
        void HandleCharacterDeathEvent() {
            onCharacterDeath();
            
            GameObject mainPlayerCharacter = characterManager.GetMainPlayerCharacter();
            GameObject currentPlayerCharacter = characterManager.GetCurrentPlayerCharacter();
            if (mainPlayerCharacter == gameObject && currentPlayerCharacter == gameObject) {
                print(gameObject.name + mainPlayerCharacter.name + currentPlayerCharacter.name + " Game Over");
            }
            else if (mainPlayerCharacter == gameObject && !currentPlayerCharacter == gameObject) {
                characterManager.SwitchToMainPlayerCharacter();
            }
            else if (!mainPlayerCharacter == gameObject && currentPlayerCharacter == gameObject) {
                StartCoroutine("SwitchAfterDeathEventTime");
            }
            else {
                print("One of your followers died.");
            }

            if (tag == "Player") {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else if (tag == "Enemy") {
                characterManager.UnregisterEnemyCharacter(gameObject);
            }

            Destroy(gameObject, deathEventTime);
        }
        IEnumerator SwitchAfterDeathEventTime() {
            yield return new WaitForSeconds(deathEventTime);
            characterManager.SwitchToMainPlayerCharacter();
        }
    }
}
