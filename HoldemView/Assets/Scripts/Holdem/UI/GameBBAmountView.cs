using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem.UI
{
    public class GameBBAmountView : BaseView
    {
        [SerializeField] private TMP_InputField sbAmountText;
        [SerializeField] private TMP_InputField bbAmountText;
        [SerializeField] private TMP_InputField anteAmountText;
        [SerializeField] private Button confirmButton;

        protected override void Initialize()
        {
            base.Initialize();

            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }


        private void OnConfirmButtonClicked()
        {
            if (!int.TryParse(sbAmountText.text, out int sb))
                return;

            if (!int.TryParse(bbAmountText.text, out int bb))
                return;

            if (!int.TryParse(anteAmountText.text, out int ante))
                return;

            if (sb == 0)
            {
                Debug.LogError("请输入有效的盲注金额！");
                return;
            }
            if (bb == 0)
            {
                Debug.LogError("请输入有效的盲注金额！");
                return;
            }

            PokerGameManager.Instance.SetBlinds(sb, bb, ante);
            Debug.Log($"盲注设置已更新 - SB: {sb}, BB: {bb}, Ante: {ante}");
            this.Hide();
        }

    }
}