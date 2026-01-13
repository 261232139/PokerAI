using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem.UI
{
    public class BaseView : MonoBehaviour
    {
        [SerializeField] protected Button closeButton;

        public BaseView Instance;
        private void Awake()
        {
            Instance = this;
            closeButton?.onClick.AddListener(OnCloseButtonClicked);
        }

        private bool isInitialized = false;
        protected virtual void Start()
        {
            if (!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }
        }
        protected virtual void Initialize()
        {

        }


        private void OnCloseButtonClicked()
        {
            Hide();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}