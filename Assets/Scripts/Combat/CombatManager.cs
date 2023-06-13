using Entity;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using DG.Tweening;

namespace Combat
{
    public enum CombatState
    {
        Waiting,
        /// <summary>
        /// Deciding which character should go first
        /// </summary>
        DecidingTurn,
        /// <summary>
        /// 
        /// </summary>
        Busy,
    }

    [System.Serializable]
    public struct CharacterTurn
    {
        public CombatCharacterBase character;
        public Transform uiTransform;
        public CombatCharacterType type;

        public CharacterTurn(CombatCharacterBase character, Transform uiTransform, CombatCharacterType type)
        {
            this.character = character;
            this.uiTransform = uiTransform;
            this.type = type;
        }
    }

    public class CombatManager : Singleton<CombatManager>
    {
        [SerializeField] private CombatState state = CombatState.DecidingTurn;
        [SerializeField] private CombatCharacterBase player;
        [SerializeField] private CombatCharacterBase[] enemies;
        private CombatCharacterBase _currentBusyCharacter;

        [Header("Turns")]
        private const float TURN_DISTANCE_TO_RESET = 1f;
        [SerializeField] private List<CharacterTurn> characterTurns = new();
        [Header("Turn UI")]
        [SerializeField] private Transform endAction;
        [SerializeField] private Transform startAction;
        [SerializeField] private Transform playerActionUI;
        [SerializeField] private Transform[] enemyActionUIs;

        protected override void AwakeSingleton()
        {
            // TODO: Instantiate combat characters

            // Create CharacterTurns
            characterTurns.Add(new(player, playerActionUI, player.Type));

            for(int i = 0; i < enemies.Length; i++)
            {
                characterTurns.Add(new(enemies[i], enemyActionUIs[i], enemies[i].Type));
            }
        }

        private void Update()
        {
            if(state == CombatState.Busy)
            {
                if (!_currentBusyCharacter.Attacking)
                {
                    state = CombatState.DecidingTurn;
                }
            }
            else if(state == CombatState.DecidingTurn)
            {
                UpdateTurn();
            }
            else if (state == CombatState.Waiting && Input.GetKeyDown(KeyCode.Space))
            {
                PlayerAttack();
            }
        }

        /// <summary>
        /// Move and update characters turn
        /// </summary>
        public void UpdateTurn()
        {
            for(int i = 0; i < characterTurns.Count; i++)
            {
                Vector3 move = (endAction.position - characterTurns[i].uiTransform.position).normalized * characterTurns[i].character.CharacterStatus.Speed * Time.deltaTime;
                characterTurns[i].uiTransform.position += move;

                if(Vector3.Distance(characterTurns[i].uiTransform.position, endAction.position) <= TURN_DISTANCE_TO_RESET)
                {
                    characterTurns[i].uiTransform.position = startAction.position;

                    // TODO:
                    // Player can control
                    if(characterTurns[i].type == CombatCharacterType.Player)
                    {
                        state = CombatState.Waiting;
                        return; // Prevent any bug happen 
                    }
                    else
                    {
                        state = CombatState.Busy;
                        EnemyAttack(characterTurns[i].character);
                        return;
                    }

                    // Enemy AI do actions
                }
            }
        }

        /// <summary>
        /// Player attack enemy
        /// </summary>
        /// <returns></returns>
        public bool PlayerAttack()
        {
            if (!player.Attacking)
            {
                state = CombatState.Busy;
                player.Attack(enemies[0]); // TODO£º Update to selected enemy

                _currentBusyCharacter = player;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enemy attack player
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool EnemyAttack(CombatCharacterBase character)
        {
            if (!character.Attacking)
            {
                state = CombatState.Busy;
                character.Attack(player);

                _currentBusyCharacter = character;
                return true;
            }

            return false;
        }
    }

}