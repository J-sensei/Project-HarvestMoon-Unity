using System.Collections.Generic;
using UnityEngine;
using Utilities;
using DG.Tweening;
using UI.Combat;

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
        /// <summary>
        /// Player Win
        /// </summary>
        Win,
        /// <summary>
        /// Player Lose
        /// </summary>
        Lose
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
        [SerializeField] private TopDownCamera.TopDownCamera topDownCamera;
        [SerializeField] private CombatState state = CombatState.DecidingTurn;
        [SerializeField] private CombatCharacterBase player;
        [SerializeField] private List<CombatCharacterBase> enemies;
        private CombatCharacterBase _currentBusyCharacter;

        [Header("Turns")]
        private const float TURN_DISTANCE_TO_RESET = 1f;
        [SerializeField] private List<CharacterTurn> characterTurns = new();
        [Header("Turn UI")]
        [SerializeField] private Transform endAction;
        [SerializeField] private Transform startAction;
        [SerializeField] private Transform playerActionUI;
        [SerializeField] private List<Transform> enemyActionUIs;

        private CombatCharacterBase _selectedCharacter;

        public TopDownCamera.TopDownCamera Camera { get { return topDownCamera; } }

        protected override void AwakeSingleton()
        {
            // TODO: Instantiate combat characters

            // Create CharacterTurns
            characterTurns.Add(new(player, playerActionUI, player.Type));

            for(int i = 0; i < enemies.Count; i++)
            {
                characterTurns.Add(new(enemies[i], enemyActionUIs[i], enemies[i].Type));
            }

            Select(enemies[0]);
        }

        public void Select(CombatCharacterBase character)
        {
            if(_selectedCharacter != null)
            {
                _selectedCharacter.Deselect();
                _selectedCharacter = null;
            }
            _selectedCharacter = character;
            _selectedCharacter.Select();
        }

        private void Update()
        {
            if(state == CombatState.Busy)
            {
                if (!_currentBusyCharacter.Attacking)
                {
                    state = CombatState.DecidingTurn;
                    if(_currentBusyCharacter.Type == CombatCharacterType.Player)
                    {
                        // Get the remove index?
                        List<int> removeIndex = new();
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if (enemies[i] == null || enemies[i].Die)
                            {
                                removeIndex.Add(i);
                            }
                        }

                        // Remove die enemy
                        for (int i = 0; i < removeIndex.Count; i++)
                        {
                            int index = removeIndex[i];
                            enemies.RemoveAt(index);
                            Destroy(enemyActionUIs[index].gameObject);
                            enemyActionUIs.RemoveAt(index);
                            characterTurns.RemoveAt(index + 1); // + 1 for player
                        }

                        if (enemies.Count == 0)
                        {
                            // TODO: WIN
                            state = CombatState.Win;
                            Debug.Log("Player WIN!!!!");
                        }
                        else
                        {
                            Select(enemies[0]);
                        }
                    }
                }
            }
            else if(state == CombatState.DecidingTurn)
            {
                UpdateTurn();
            }
            //else if (state == CombatState.Waiting && Input.GetKeyDown(KeyCode.Space))
            //{
            //    PlayerAttack();
            //}
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
                        CombatUIManager.Instance.ToggleActionUI(true); // Show UI for the player to choose
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
            if (state == CombatState.Waiting && !player.Attacking)
            {
                state = CombatState.Busy;
                player.Attack(_selectedCharacter); // TODO£º Update to selected enemy

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