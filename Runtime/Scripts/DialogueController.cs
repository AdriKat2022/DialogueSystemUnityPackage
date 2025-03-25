using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AdriKat.DialogueSystem.Core
{
    public class DialogueController : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private float _timePerCharacterAnimation = 0.05f;
        [SerializeField] private float _timePerOptionAnimation = 0.3f;
        [SerializeField] private float _delayBetweenOptionsAnimation = 0.1f;

        [Header("Default Behaviour")]
        [SerializeField] private bool _resetStoryOnDialogueStart = true;

        [Header("References")]
        [SerializeField] private GameObject _titleTextContainer;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _mainText;
        [Space]
        [SerializeField] private InputActionReference _continueAction;
        [Space]
        [SerializeField] private CanvasGroup _optionsContainer;
        [SerializeField] private DialogueOption _optionPrefab;

        private RectTransform _rectTransform;
        private int _lastOptionSelected = 0;
        private bool _isOptionSelected = false;
        private bool _dialogueRunning = false;

        #region Initialization
        private void OnEnable()
        {
            _continueAction.action.Enable();
            _continueAction.action.performed += ContinueAction_performed;
        }

        private void OnDisable()
        {
            _continueAction.action.Disable();
            _continueAction.action.performed -= ContinueAction_performed;
        }

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
            _optionsContainer.gameObject.SetActive(false);
        }
        #endregion

        /// <summary>
        /// Goes through a dialogue and displays the text and choices.
        /// </summary>
        /// <param name="dialogue"></param>
        public void StartDialogue(Dialogue dialogue)
        {
            if (_dialogueRunning) return;
            _dialogueRunning = true;

            gameObject.SetActive(true);

            StartCoroutine(GoThroughDialogue(dialogue, _resetStoryOnDialogueStart));
        }

        /// <summary>
        /// Goes through a dialogue and displays the text and choices.
        /// </summary>
        /// <param name="forceRestart">Resets the dialogue to the first bubble before starting if this true.</param>
        public void StartDialogue(Dialogue dialogue, bool forceRestart)
        {
            if (_dialogueRunning) return;
            _dialogueRunning = true;

            Debug.Log("Start of dialogue");

            gameObject.SetActive(true);

            StartCoroutine(GoThroughDialogue(dialogue, forceRestart));
        }

        private void EndDialogue()
        {
            gameObject.SetActive(false);
            _dialogueRunning = false;
        }

        private IEnumerator GoThroughDialogue(Dialogue dialogue, bool forceRestart)
        {
            Debug.Log("Going through dialogue : " + dialogue);

            if (forceRestart)
            {
                dialogue.ResetToFirstDialogue();
            }

            if (!dialogue.IsInitialized())
            {
                Debug.LogError("Dialogue is not initialized! Avoid using it in awake if the dialogue is enabled at the same time.");
                yield break;
            }

            while (!dialogue.IsEndOfDialogue())
            {
                ExecutableDialogueSO currentDialogueBubble = dialogue.GetNext(_lastOptionSelected);

                if (currentDialogueBubble == null)
                {
                    Debug.LogError("Current dialogue is null!");
                    yield break;
                }

                _titleText.text = "";
                _mainText.text = currentDialogueBubble.Text;

                // If the title text begins with // then hide the title text container
                _titleTextContainer.SetActive(!string.IsNullOrEmpty(_titleText.text) && !_titleText.text.StartsWith("//"));

                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

                yield return AnimateText(currentDialogueBubble.Text, _mainText);

                if (dialogue.IsChoiceAvailable())
                {
                    // Choice has to be made!
                    var choices = dialogue.GetCurrentChoices();
                    yield return WaitForOptionSelection(choices);
                }
                else
                {
                    // No choice
                    yield return new WaitUntil(() => _continueAction.action.triggered);
                }

                yield return null;
            }

            EndDialogue();
        }

        private IEnumerator WaitForOptionSelection(List<string> choices)
        {
            _isOptionSelected = false;
            yield return DisplayOptions(choices);
            yield return new WaitUntil(() => _isOptionSelected);
        }

        private IEnumerator DisplayOptions(List<string> choices)
        {
            if (choices == null)
            {
                Debug.LogError("Choices are null!");
                yield break;
            }

            // Clear the options panel
            _optionsContainer.alpha = 0;
            _optionsContainer.gameObject.SetActive(true);
            foreach (Transform child in _optionsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Initialize all options
            DialogueOption[] options = new DialogueOption[choices.Count];
            int optionIndex = 0;
            foreach (var choice in choices)
            {
                options[optionIndex] = Instantiate(_optionPrefab, _optionsContainer.transform);
                options[optionIndex].InitializeOption(optionIndex, choice, OptionSelected);
                optionIndex++;
            }
            yield return null;

            // Force to recalculate the layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            _optionsContainer.alpha = 1;

            // Optionally animate the options
            foreach (var option in options)
            {
                StartCoroutine(AnimateOption(option, _timePerOptionAnimation));

                yield return new WaitForSeconds(_delayBetweenOptionsAnimation);
            }
        }

        #region Callbacks
        private void OptionSelected(int optionIndex)
        {
            if (_isOptionSelected) return;

            _lastOptionSelected = optionIndex;
            _isOptionSelected = true;

            _optionsContainer.gameObject.SetActive(false);
        }

        private void ContinueAction_performed(InputAction.CallbackContext obj)
        {
            // TODO: Make a small animation if a dialogue is playing.
            // TODO: Also skip the text animation if there is any running.
        }
        #endregion

        #region Animations

        private IEnumerator AnimateOption(DialogueOption option, float timePerOptionAnimation)
        {
            CanvasGroup optionCanvas = option.GetComponent<CanvasGroup>();
            float elapsedTime = 0;

            while (elapsedTime < timePerOptionAnimation)
            {
                optionCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / timePerOptionAnimation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator AnimateText(string text, TextMeshProUGUI textComponent)
        {
            textComponent.maxVisibleCharacters = 0;
            textComponent.text = text;
            foreach (char _ in text)
            {
                textComponent.maxVisibleCharacters++;
                yield return new WaitForSeconds(_timePerCharacterAnimation);
            }
        }

        #endregion

    }
}