using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace coldheart_core {
    public class CharacterManager : MonoBehaviour {
        [SerializeField] GameObject mainPlayerCharacter;
        [SerializeField] GameObject currentPlayerCharacter;
        [SerializeField] List<GameObject> playerCharacters;
        [SerializeField] List<GameObject> enemyCharacters;
        public event Action onSwitchCharacterAction;
        public event Action onAllPlayerCharactersFollowCurrentPlayer;
        public GameObject GetMainPlayerCharacter () {
            return mainPlayerCharacter;
        }
        public GameObject GetCurrentPlayerCharacter () {
            return currentPlayerCharacter;
        }
        public List<GameObject> GetEnemyCharacters() {
            return enemyCharacters;
        }
        public Boolean CheckIfCharacterIsAPlayerCharacter(GameObject character) {
            return playerCharacters.Contains(character);
        }
        void Start() {
            SetCurrentPlayerCharacter();
        }
        void SetCurrentPlayerCharacter() {
            if (mainPlayerCharacter != null) {
                currentPlayerCharacter = mainPlayerCharacter;
                playerCharacters.Add(mainPlayerCharacter);
            }
            else {
                print("Main Player Character is not set in Character Manager!");
            }
        }
        public void RegisterPlayerCharacter(GameObject playerCharacter, bool isControlledByPlayer) {
            playerCharacters.Add(playerCharacter);

            if (isControlledByPlayer) {
                currentPlayerCharacter = playerCharacter;
            }
        }
        public void UnregisterPlayerCharacter(GameObject playerCharacter) {
            bool isAPlayerCharacter = playerCharacters.Contains(playerCharacter);
            if (isAPlayerCharacter) {
                playerCharacters.Remove(playerCharacter);

                if (playerCharacter == currentPlayerCharacter) {
                    currentPlayerCharacter = mainPlayerCharacter;
                }
            }
        }
        public void RegisterEnemyCharacter(GameObject enemyCharacter) {
            enemyCharacters.Add(enemyCharacter);
        }
        public void UnregisterEnemyCharacter(GameObject enemyCharacter) {
            bool isAnEnemyCharacter = playerCharacters.Contains(enemyCharacter);
            if (isAnEnemyCharacter) {
                playerCharacters.Remove(enemyCharacter);
            }
        }
        public void SwitchToNextCharacter() {
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
        public void SwitchToPreviousCharacter() {
            int currentPlayerCharacterIndex = playerCharacters.IndexOf(currentPlayerCharacter);
            int previousPlayerCharacterIndex;
            if (currentPlayerCharacterIndex == 0) {
                previousPlayerCharacterIndex = playerCharacters.Count - 1;
            }
            else {
                previousPlayerCharacterIndex = currentPlayerCharacterIndex - 1;
            }
            currentPlayerCharacter = playerCharacters[previousPlayerCharacterIndex];
            onSwitchCharacterAction();
        }
        public void AllPlayerCharactersFollowCurrentPlayer() {
            onAllPlayerCharactersFollowCurrentPlayer();
        }
    }
}