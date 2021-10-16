using UnityEngine;

namespace Entities
{
    public class ExperienceContainer
    {
        public int Experience { get; private set; }
        public int MaxExperience { get; private set; }
        public int Level { get; private set; }

        public float HealthMultiplier => 3 + (Level - 1) / 19f * 7;
        public float AttackMultiplier => 0.5f + (Level - 1) / 19f * 1.5f;
        public float SpeedMultiplier => 1 + (Level - 1) / 19f * 1;

        public ExperienceContainer(int level, int exp)
        {
            Experience = exp;
            Level = level;
            MaxExperience = GetMaxExperience(level);
        }

        public void AddExperience(int experience)
        {
            Experience += experience;
            while (Experience >= MaxExperience)
            {
                Experience -= MaxExperience;
                Level++;
                MaxExperience = GetMaxExperience(Level);
            }
        }

        private int GetMaxExperience(int level)
        {
            return Mathf.RoundToInt(500 + (Mathf.Pow(level, 1.1f) - 1) * 500);
        }
    }
}
