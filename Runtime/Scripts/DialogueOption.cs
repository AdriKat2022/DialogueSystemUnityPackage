using System;
using TMPro;
using UnityEngine;

namespace AdriKat.DialogueSystem.UI
{
    public class DialogueOption : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _optionText;

        private int _id;
        private Action<int> _callback;

        public void InitializeOption(int id, string optionText, Action<int> callback)
        {
            _id = id;
            _optionText.text = optionText;
            _callback = callback;
        }

        public void OnClick()
        {
            _callback?.Invoke(_id);
        }
    }
}