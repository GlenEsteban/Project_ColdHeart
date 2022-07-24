using System;
using System.Collections;
using UnityEngine;

namespace coldheart_core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth;
        [SerializeField] float currentHealth;
        [SerializeField] float invincibilityTime;
        [SerializeField] float deathEventTime = 2f;
        public float GetCurrentHealth() {
            return currentHealth;
        }
        public event Action onCharacterDeath;
        CharacterManager characterManager;
        float timerForInvincibility;
        bool isInvincible;
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
        public void ResetHealth() {
            currentHealth = maxHealth;
        }
        public void ReduceHealth(float damageDealt) {
            bool isDead = currentHealth <= 0;
            if (isInvincible || isDead) {return;}

            currentHealth = Mathf.Max(currentHealth - damageDealt, 0);
            if (currentHealth == 0) {
                if (onCharacterDeath != null) {
                    HandleCharacterDeathEvent();
                }
            }

            GainInvincibility(invincibilityTime);
        }
        public void RestoreHealth(float healAmount) {
            bool isDead = currentHealth <= 0;
            if (isDead) {return;}
            
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
            onCharacterDeath();

            if (tag == "Player") {
                GameObject mainPlayerCharacter = characterManager.GetMainPlayerCharacter();
                GameObject currentPlayerCharacter = characterManager.GetCurrentPlayerCharacter();
                if (mainPlayerCharacter == gameObject) {
                    characterManager.SwitchToMainPlayerCharacter();
                }
                else if (mainPlayerCharacter != gameObject && currentPlayerCharacter == gameObject) {
                    StartCoroutine("DelaySwitchToMainPlayerCharacter");
                    UnregisterCharacter();
                }
            }
            else if (tag == "Enemy") {
                UnregisterCharacter();
            }

            if (characterManager.GetMainPlayerCharacter() != gameObject) {
                Destroy(gameObject, deathEventTime + 1f);
            } 
        }
        void UnregisterCharacter() {
            if (tag == "Player") {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else if (tag == "Enemy") {
                characterManager.UnregisterEnemyCharacter(gameObject);
            }
        }
        IEnumerator DelaySwitchToMainPlayerCharacter() {
            yield return new WaitForSeconds(deathEventTime);
            characterManager.SwitchToMainPlayerCharacter();
        }
    }
}