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
                yield return new WaitForSeconds(3f);
                ReduceHealth(10f);
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
        void HandleCharacterDeathEvent()
        {
            print(gameObject.name + " died...");
            onCharacterDeath();

            GameObject mainPlayerCharacter = characterManager.GetMainPlayerCharacter();
            GameObject currentPlayerCharacter = characterManager.GetCurrentPlayerCharacter();
            if (mainPlayerCharacter == gameObject) {
                characterManager.SwitchToMainPlayerCharacter();
            }
            else if (mainPlayerCharacter != gameObject && currentPlayerCharacter == gameObject) {
                StartCoroutine("DelaySwitchToMainPlayerCharacter");
            }

            if (characterManager.GetMainPlayerCharacter() != gameObject) {
                Destroy(gameObject, deathEventTime + 1f);
            }
        }
        IEnumerator DelaySwitchToMainPlayerCharacter() {
            yield return new WaitForSeconds(deathEventTime);
            characterManager.SwitchToMainPlayerCharacter();
            UnregisterCharacter();
            gameObject.SetActive(false);
        }
        void UnregisterCharacter() {
            if (tag == "Player") {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else if (tag == "Enemy") {
                characterManager.UnregisterEnemyCharacter(gameObject);
            }
        }
    }
}
