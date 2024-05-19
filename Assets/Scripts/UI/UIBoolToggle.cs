using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIBoolToggle : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI label;
        private Option<bool> Option { get; set; }

        public void Initialize(Option<bool> option)
        {
            toggle = GetComponentInChildren<Toggle>();
            toggle.isOn = option.Value;
            toggle.onValueChanged.AddListener(OnValueChanged);
            Option = option;
            label.text = option.DisplayName;
        }

        private void OnValueChanged(bool value)
        {
            Option.Value = value;
        }
    }
}