using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progession", menuName = "Stats/New Progession", order = 0)]
    public class Progession : ScriptableObject
    {

        [SerializeField] ProgessionCharacterClass[] characterClass = null;

        [System.Serializable]
        class ProgessionCharacterClass
        {
            [SerializeField] CharacterClass characterClass;
            [SerializeField] float[] health;

        }
    }

}