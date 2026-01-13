using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem.UI
{
    public class GameSeatComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI chipPoolText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI actionChipText;
        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(OnConfirmButtonClicked);
        }

        private void OnConfirmButtonClicked()
        {

        }

    }
}