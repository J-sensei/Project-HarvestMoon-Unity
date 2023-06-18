using System.Collections.Generic;
using UnityEngine;
using Utilities;
using DG.Tweening;
using UI.Combat;
using StateMachine.Player;
using Utilities.Audio;
using TopDownCamera;
using GameDateTime;
using Inventory;
using Entity;

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
        [SerializeField] private GameObject combatUI;
        [SerializeField] private Transform endAction;
        [SerializeField] private Transform startAction;
        [SerializeField] private Transform playerActionUI;
        [SerializeField] private List<Transform> enemyActionUIs;

        [SerializeField] private bool start = true;

        [Header("UI")]
        [SerializeField] private GameObject winUI;
        [SerializeField] private GameObject loseUI;
        [SerializeField] private GameObject itemUI;

        [Header("Audio")]
        [SerializeField] private BGMData victoryBGM;
        [SerializeField] private BGMData loseBGM;

        public bool CombatStart { get { return start; } set { start = value; } }
        public CombatCharacterBase Player { get { return player; } }

        private CombatCharacterBase _selectedCharacter;

        public TopDownCamera.TopDownCamera Camera { get { return topDownCamera; } }

        protected override void AwakeSingleton()
        {
            // Delete player state machine
            PlayerStateMachine previousPlayer = GameObject.FindObjectOfType<PlayerStateMachine>();
            if(previousPlayer != null)
            {
                Destroy(previousPlayer.gameObject);
            }

            // TODO: Instantiate combat characters
            // Random decide delete 1 - 2 enemy
            int rand = Random.Range(0, 3); // Delete 0, 1 or 2 enemy when battle start
            Debug.Log("Rand: " + rand);
            for(int i = 0; i < rand; i++)
            {
                // Destroy
                Destroy(enemies[enemies.Count - 1].gameObject);
                Destroy(enemyActionUIs[enemyActionUIs.Count - 1].gameObject);

                // Remove
                enemies.RemoveAt(enemies.Count - 1);
                enemyActionUIs.RemoveAt(enemyActionUIs.Count - 1);
            }

            // Create CharacterTurns
            characterTurns.Add(new(player, playerActionUI, player.Type));

            for(int i = 0; i < enemies.Count; i++)
            {
                characterTurns.Add(new(enemies[i], enemyActionUIs[i], enemies[i].Type));
            }

            Select(enemies[0]);

            // Init UI
            if (winUI.activeSelf)
            {
                winUI.SetActive(false);
                loseUI.SetActive(false);
                itemUI.SetActive(false);
            }
            else
            {
                winUI.SetActive(true);
                winUI.SetActive(false);
                loseUI.SetActive(true);
                loseUI.SetActive(false);
                itemUI.SetActive(true);
                itemUI.SetActive(false);
            }
        }

        private void Start()
        {
            // Load player status
            ((PlayerStatus)player.CharacterStatus).Load(GameStateManager.Instance.LoadTempData().playerStatus);
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
            if (!start) return;
            if (player.Die)
            {
                CinematicCamera.Instance.LoseCamera();
                start = false;
                return;
            }

            // Busy state, check when character is finish attacking
            if(state == CombatState.Busy)
            {
                if (!_currentBusyCharacter.Attacking)
                {
                    state = CombatState.DecidingTurn;
                    if(_currentBusyCharacter.Type == CombatCharacterType.Player)
                    {
                        // Get the remove index?
                        bool remove = false;
                        List<int> removeIndex = new();
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            if (enemies[i] == null || enemies[i].Die)
                            {
                                removeIndex.Add(i);
                                remove = true;
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
                            // TODO: Open Win Screen and click to go back to previous scene :)
                            state = CombatState.Win;
                            Debug.Log("Player WIN!!!!");
                            //Win();
                            CinematicCamera.Instance.WinCamera();
                        }
                        else
                        {
                            if(remove)
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

        public void Win()
        {
            // Hide battle ui
            combatUI.SetActive(false);

            // Show win screen
            Vector3 pos = winUI.transform.position;
            float endVal = pos.y;
            pos.y += 100;
            winUI.gameObject.SetActive(true);
            winUI.transform.position = pos;
            winUI.transform.DOMoveY(endVal, 0.15f);

            player.WinAnimation();
            AudioManager.Instance.PlayMusic(victoryBGM, false);
            //CinematicCamera.Instance.WinCamera();
        }

        public void Lose()
        {
            // Hide battle ui
            combatUI.SetActive(false);

            // Show lose screen
            Vector3 pos = loseUI.transform.position;
            float endVal = pos.y;
            pos.y += 100;
            loseUI.gameObject.SetActive(true);
            loseUI.transform.position = pos;
            loseUI.transform.DOMoveY(endVal, 0.15f);

            AudioManager.Instance.PlayMusic(loseBGM, false);
        }

        public void BackToScene()
        {
            // TODO: Load back to previous scene
            GameManager.Instance.ExitCombat();
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
                player.Attack(_selectedCharacter); // Attack selected enemy

                _currentBusyCharacter = player;
                return true;
            }

            return false;
        }

        public bool PlayerThrow(ItemData item)
        {

            if (state == CombatState.Waiting && !player.Attacking)
            {
                state = CombatState.Busy;
                player.Throw(_selectedCharacter, item);

                _currentBusyCharacter = player;
                return true;
            }
            return false;
        }

        public void Skip()
        {
            state = CombatState.DecidingTurn;
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

        public void MainMenu()
        {
            GameMenu.Instance.DisableGameMenu(false); // TO display back the game menu
            GameUIManager.Instance.DisableMenu(false);
            GameManager.Instance.MainMenu();
        }
    }

}