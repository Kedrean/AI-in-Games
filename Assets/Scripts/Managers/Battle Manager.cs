using Assets.Scripts.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Controls the overall battle state.
    /// Handles starting, ending, and monitoring battles.
    /// </summary>
    public sealed class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }


        public enum BattleState
        {
            Waiting,
            Preparing,
            Fighting,
            Victory,
            Defeat
        }


        public BattleState CurrentState { get; private set; }


        public event Action BattleStarted;
        public event Action BattleEnded;


        [Header("Teams")]
        [SerializeField]
        private Team _playerTeam = Team.Human;

        [SerializeField]
        private Team _enemyTeam = Team.Skeleton;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            CurrentState = BattleState.Waiting;
        }


        private void Update()
        {
            if (CurrentState != BattleState.Fighting)
                return;


            CheckBattleResult();
        }


        /// <summary>
        /// Begins the battle.
        /// </summary>
        public void StartBattle()
        {
            if (CurrentState == BattleState.Fighting)
                return;


            CurrentState = BattleState.Preparing;


            PrepareBattle();


            CurrentState = BattleState.Fighting;

            BattleStarted?.Invoke();
        }


        private void PrepareBattle()
        {
            // Reserved for future preparation logic:
            // - unit placement
            // - countdown
            // - pre-battle effects
        }


        private void CheckBattleResult()
        {
            var playerUnits =
                TeamManager.Instance.GetLivingTeam(_playerTeam);

            var enemyUnits =
                TeamManager.Instance.GetLivingTeam(_enemyTeam);


            if (playerUnits.Count == 0)
            {
                EndBattle(false);
                return;
            }


            if (enemyUnits.Count == 0)
            {
                EndBattle(true);
            }
        }


        /// <summary>
        /// Ends the battle.
        /// </summary>
        private void EndBattle(bool playerWon)
        {
            CurrentState =
                playerWon
                ? BattleState.Victory
                : BattleState.Defeat;


            BattleEnded?.Invoke();
        }


        public bool IsBattleActive()
        {
            return CurrentState == BattleState.Fighting;
        }
    }
}