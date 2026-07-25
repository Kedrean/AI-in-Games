using Assets.Scripts.Core;
using Assets.Scripts.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Controls the overall battle state.
    /// Handles spawning units, starting, ending,
    /// and monitoring battles.
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

        [Serializable]
        private class ArmyUnit
        {
            public GameObject Prefab;

            [Min(1)]
            public int Count = 1;
        }

        public BattleState CurrentState { get; private set; }

        public event Action BattleStarted;
        public event Action BattleEnded;

        [Header("Teams")]
        [SerializeField]
        private Team _playerTeam = Team.Human;

        [SerializeField]
        private Team _enemyTeam = Team.Skeleton;

        [Header("Spawn Points")]
        [SerializeField]
        private Transform _playerSpawnPoint;

        [SerializeField]
        private Transform _enemySpawnPoint;

        [Header("Player Army")]
        [SerializeField]
        private ArmyUnit[] _playerUnits;

        [Header("Enemy Army")]
        [SerializeField]
        private ArmyUnit[] _enemyUnits;

        [Header("Spawn Settings")]
        [SerializeField]
        [Min(0.5f)]
        private float _spawnSpacing = 2f;

        [SerializeField]
        [Min(1)]
        private int _unitsPerRow = 4;

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

        /// <summary>
        /// Spawns both teams.
        /// </summary>
        private void PrepareBattle()
        {
            SpawnTeam(
                _playerUnits,
                _playerSpawnPoint);

            SpawnTeam(
                _enemyUnits,
                _enemySpawnPoint);
        }

        /// <summary>
        /// Spawns an entire army around its spawn point.
        /// </summary>
        private void SpawnTeam(
            ArmyUnit[] army,
            Transform origin)
        {
            if (origin == null)
            {
                Debug.LogError("BattleManager is missing a Spawn Point.");
                return;
            }

            if (army == null || army.Length == 0)
                return;

            int spawnIndex = 0;

            foreach (ArmyUnit armyUnit in army)
            {
                if (armyUnit == null)
                    continue;

                if (armyUnit.Prefab == null)
                    continue;

                for (int i = 0; i < armyUnit.Count; i++)
                {
                    int row = spawnIndex / _unitsPerRow;
                    int column = spawnIndex % _unitsPerRow;

                    float xOffset =
                        (column - (_unitsPerRow - 1) * 0.5f) * _spawnSpacing;

                    float zOffset =
                        row * _spawnSpacing;

                    Vector3 spawnPosition =
                        origin.position +
                        origin.right * xOffset +
                        origin.forward * zOffset;

                    Instantiate(
                        armyUnit.Prefab,
                        spawnPosition,
                        origin.rotation);

                    spawnIndex++;
                }
            }
        }

        /// <summary>
        /// Checks whether either team has been eliminated.
        /// </summary>
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