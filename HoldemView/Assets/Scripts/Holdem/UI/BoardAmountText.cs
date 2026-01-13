using System.Collections;
using TMPro;
using UnityEngine;

namespace Holdem.UI
{
    public class BoardAmountText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;

        // Use this for initialization
        void Start()
        {
            EventManager.Instance.AddListener<int>(EventName.BOARD_AMOUNT_CHANGED, OnBoardAmountChanged);
        }

        private void OnBoardAmountChanged(int amount)
        {
            amountText.text = amount.ToString();
        }
    }
}