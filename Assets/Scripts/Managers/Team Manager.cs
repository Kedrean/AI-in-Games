using Assets.Scripts.Core;
using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    /// <summary>
    /// Tracks units belonging to each team.
    /// Handles team registration and lookup.
    /// </summary>
    public sealed class TeamManager : MonoBehaviour
    {
        public static TeamManager Instance { get; private set; }


        private readonly Dictionary<Team, List<UnitController>> _teams = new();


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;


            foreach (Team team in System.Enum.GetValues(typeof(Team)))
            {
                _teams.Add(
                    team,
                    new List<UnitController>());
            }
        }


        /// <summary>
        /// Registers a unit into its assigned team.
        /// </summary>
        public void RegisterUnit(UnitController unit)
        {
            if (unit == null)
                return;


            Team team = unit.Data.Team;


            if (!_teams.ContainsKey(team))
            {
                _teams.Add(
                    team,
                    new List<UnitController>());
            }


            if (!_teams[team].Contains(unit))
            {
                _teams[team].Add(unit);
            }
        }


        /// <summary>
        /// Removes a unit from its team.
        /// </summary>
        public void UnregisterUnit(UnitController unit)
        {
            if (unit == null)
                return;


            Team team = unit.Data.Team;


            if (_teams.ContainsKey(team))
            {
                _teams[team].Remove(unit);
            }
        }


        /// <summary>
        /// Returns all units belonging to a team.
        /// </summary>
        public IReadOnlyList<UnitController> GetTeam(
            Team team)
        {
            if (_teams.TryGetValue(team, out var units))
            {
                return units;
            }


            return new List<UnitController>();
        }


        /// <summary>
        /// Returns living units belonging to a team.
        /// </summary>
        public List<UnitController> GetLivingTeam(
            Team team)
        {
            List<UnitController> livingUnits = new();


            if (!_teams.TryGetValue(team, out var units))
                return livingUnits;


            foreach (UnitController unit in units)
            {
                if (unit != null &&
                    unit.Health.IsAlive)
                {
                    livingUnits.Add(unit);
                }
            }


            return livingUnits;
        }
    }
}