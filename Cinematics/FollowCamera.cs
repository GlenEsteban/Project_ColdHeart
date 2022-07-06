using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using coldheart_core;
using UnityEngine;

namespace coldheart_cinematics {
    public class FollowCamera : MonoBehaviour
    {
        CharacterManager characterManager;
        CinemachineVirtualCamera cinemachineVirtualCamera;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
        void OnEnable() {
            characterManager.onSwitchCharacterAction += UpdateFollowTarget;
        }
        void UpdateFollowTarget() {
            cinemachineVirtualCamera.m_Follow = characterManager.GetCurrentPlayerCharacter().transform;
        }
    }
}
