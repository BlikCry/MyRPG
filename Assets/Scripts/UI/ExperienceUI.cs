using UnityEngine;

namespace UI
{
    internal class ExperienceUI: MonoBehaviour
    {
        [SerializeField]
        private TextFormatter levelFormatter;
        [SerializeField]
        private TextFormatter experienceFormatter;

        private void Update()
        {
            levelFormatter.Format(CharacterBody.Instance.LevelSystem.Level);
            experienceFormatter.Format(CharacterBody.Instance.LevelSystem.Experience, CharacterBody.Instance.LevelSystem.MaxExperience);
        }
    }
}
