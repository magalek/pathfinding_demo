using Game;
using UnityEngine;

namespace UI
{
    public class UIOptionPanel : MonoBehaviour
    {
        [SerializeField] private UIBoolToggle boolTogglePrefab;
        [SerializeField] private Transform optionsPanelRoot;

        private void Start()
        {
            Instantiate(boolTogglePrefab,  optionsPanelRoot).Initialize(GameManager.Current.Options.PathDrawingOption);
        }
    }
}