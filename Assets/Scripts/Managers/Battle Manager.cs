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

        [Header("Team Prefabs")]
        [SerializeField]
        private GameObject[] _playerUnits;

        [SerializeField]
        private GameObject[] _enemyUnits;

        [Header("Spawn Settings")]
        [SerializeField]
        [Min(0.5f)]
        private float _spawnSpacing = 2f;

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
        /// Spawns both teams near their designated spawn points.
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
        /// Spawns all units around a team's origin point.
        /// </summary>
        private void SpawnTeam(
            GameObject[] prefabs,
            Transform origin)
        {
            if (origin == null)
            {
                Debug.LogError("BattleManager is missing a Spawn Point.");
                return;
            }

            if (prefabs == null || prefabs.Length == 0)
                return;

            int unitsPerRow = 4;

            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject prefab = prefabs[i];

                if (prefab == null)
                    continue;

                int row = i / unitsPerRow;
                int column = i % unitsPerRow;

                float xOffset =
                    (column - (unitsPerRow - 1) * 0.5f) * _spawnSpacing;

                float zOffset =
                    row * _spawnSpacing;

                Vector3 spawnPosition =
                    origin.position +
                    origin.right * xOffset +
                    origin.forward * zOffset;

                GameObject spawned =
                    Instantiate(
                        prefab,
                        spawnPosition,
                        origin.rotation);

                UnitController unit =
                    spawned.GetComponent<UnitController>();

                if (unit != null)
                {
                    TeamManager.Instance.RegisterUnit(unit);
                }
            }
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