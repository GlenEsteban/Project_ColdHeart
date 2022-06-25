using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace coldheart_core {
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] GameObject mainPlayerCharacter;
        [SerializeField] GameObject currentPlayerCharacter;
        [SerializeField] GameObject playerCharacterTarget;
        [SerializeField] List<GameObject> playerCharacters;
        [SerializeField] List<GameObject> enemyCharacters;
        public event Action onSwitchCharacterAction;
        public GameObject GetMainPlayerCharacter () {
            return mainPlayerCharacter;
        }
        public GameObject GetCurrentPlayerCharacter () {
            return currentPlayerCharacter;
        }
        public GameObject GetPlayerCharacterTarget() {
            return playerCharacterTarget;
        }
        public void SetPlayerCharacterTarget(GameObject target) {
            playerCharacterTarget = target;
        }
        public bool GetIsAPlayerCharacter(GameObject character) {
            return playerCharacters.Contains(character);
        }
        public bool GetIsAnEnemyCharacter(GameObject character) {
            return enemyCharacters.Contains(character);
        }
        void Start()
        {
            SetCurrentPlayerCharacter();
            SetPlayerCharacterTarget(currentPlayerCharacter);
        }
        void SetCurrentPlayerCharacter()
        {
            if (mainPlayerCharacter != null) {
                currentPlayerCharacter = mainPlayerCharacter;
                playerCharacters.Add(mainPlayerCharacter);
                mainPlayerCharacter.tag = "Player";
            }
            else {
                print("Main Player Character is not set in Character Manager!");
            }
        }
        public void RegisterPlayerCharacter(GameObject playerCharacter, bool isControlledByPlayer) {
            playerCharacters.Add(playerCharacter);
            playerCharacter.tag = "Player";

            if (isControlledByPlayer) {
                currentPlayerCharacter = playerCharacter;
            }
        }
        public void UnregisterPlayerCharacter(GameObject playerCharacter) {
            bool isAPlayerCharacter = playerCharacters.Contains(playerCharacter);
            if (isAPlayerCharacter) {
                playerCharacters.Remove(playerCharacter);
                playerCharacter.tag = "Untagged";

                if (playerCharacter == currentPlayerCharacter) {
                    currentPlayerCharacter = mainPlayerCharacter;
                }
            }
        }
        public void SwitchCurrentPlayerCharacter() {
            int currentPlayerCharacterIndex = playerCharacters.IndexOf(currentPlayerCharacter);
            int nextPlayerCharacterIndex;
            if (currentPlayerCharacterIndex == playerCharacters.Count - 1) {
                nextPlayerCharacterIndex = 0;
            }
            else {
                nextPlayerCharacterIndex = currentPlayerCharacterIndex + 1;
            }
            currentPlayerCharacter = playerCharacters[nextPlayerCharacterIndex];
            onSwitchCharacterAction();
        }
        public void RegisterEnemyCharacter(GameObject enemyCharacter) {
            playerCharacters.Add(enemyCharacter);
            enemyCharacter.tag = "Enemy";
        }
        public void UnregisterEnemyCharacter(GameObject enemyCharacter) {
            bool isAnEnemyCharacter = playerCharacters.Contains(enemyCharacter);
            if (isAnEnemyCharacter) {
                playerCharacters.Remove(enemyCharacter);
                enemyCharacter.tag = "Untagged";
            }
        }
    }
}