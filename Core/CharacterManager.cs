using System;
using System.Collections.Generic;
using UnityEngine;

namespace coldheart_core
{
    public class CharacterManager : MonoBehaviour {
        [SerializeField] GameObject mainPlayerCharacter;
        [SerializeField] GameObject currentPlayerCharacter;
        [SerializeField] List<GameObject> playerCharacters;
        [SerializeField] List<GameObject> enemyCharacters;
        public event Action onSwitchCharacterAction;
        public event Action onFollowMeAction;
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
            AssignCurrentPlayerCharacter();
        }
        void AssignCurrentPlayerCharacter() {
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
            if (nextPlayerCharacterIndex <= 0 || nextPlayerCharacterIndex > playerCharacters.Count) {
                currentPlayerCharacter = playerCharacters[0];
            }
            else {
                currentPlayerCharacter = playerCharacters[nextPlayerCharacterIndex];
            }

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
            if (previousPlayerCharacterIndex <= 0 || previousPlayerCharacterIndex > playerCharacters.Count) {
                currentPlayerCharacter = playerCharacters[0];
            }
            else {
                currentPlayerCharacter = playerCharacters[previousPlayerCharacterIndex];
            }

            onSwitchCharacterAction();
        }
        public void SwitchToMainPlayerCharacter() {
            int mainPlayerCharacterIndex = playerCharacters.IndexOf(mainPlayerCharacter);
            if (mainPlayerCharacterIndex < 0 || mainPlayerCharacterIndex >= playerCharacters.Count) {
                currentPlayerCharacter = playerCharacters[0];
            }
            else {
                currentPlayerCharacter = playerCharacters[mainPlayerCharacterIndex];
            }

            onSwitchCharacterAction();
        }
        public void AllPlayerCharactersFollowCurrentPlayer() {
            onFollowMeAction();
        }
    }
}