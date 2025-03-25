using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Core
{
    public class Dialogue : MonoBehaviour
    {
        #region Dialogue Selection Variables (Serialized by Custom Editor)
        // Dialogue Scriptable Object
        [SerializeField] private DialogueContainerSO dialogueContainer;
        [SerializeField] private DialogueGroupSO dialogueGroup;
        [SerializeField] private DialogueSO dialogue;

        // Filters
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialogueOnly;

        // Indexes
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
        #endregion

        private bool firstCall = true;
        private ExecutableDialogueSO currentDialogue;

        private int lastDialogueIndex;
        private List<ExecutableDialogueSO> dialoguesHistory;

        #region Initialization Methods
        private void Awake()
        {
            ResetToFirstDialogue();
        }
        #endregion

        #region Getters

        public bool IsInitialized() => currentDialogue != null;

        public bool IsStartOfDialogue() => firstCall;

        public bool IsEndOfDialogue() => !firstCall && currentDialogue.Choices.Count == 1 && currentDialogue.Choices[0].NextDialogue == null;

        public bool IsEndOfDialogue(int conditionalChoice)
        {
            if (currentDialogue.Type == DialogueType.MultipleChoice)
            {
                return currentDialogue.Choices[conditionalChoice].NextDialogue == null;
            }

            return IsEndOfDialogue();
        }

        public bool IsChoiceAvailable() => currentDialogue.Choices.Count > 1;

        public List<string> GetCurrentChoices()
        {
            if (!IsChoiceAvailable())
            {
                Debug.LogWarning("Tried to get the choices but there are no choices available!");
                return null;
            }

            List<string> choices = new();
            foreach (var choice in currentDialogue.Choices)
            {
                choices.Add(choice.Text);
            }

            return choices;
        }

        #endregion

        #region Dialogue Control Methods

        /// <summary>
        /// Moves to the next dialogue and returns it.<br/>
        /// <strong>If this is the first time you're iterating on this dialogue, it will return the first dialogue.</strong> You don't have to call GetCurrent() for the first one.<br/>
        /// </summary>
        /// <param name="choiceNumber">Move with this choice index. This parameter is ignored if the current dialogue is single-choice.</param>
        /// <returns>Returns the dialogue if there is one. Returns null if you reached the end.</returns>
        public ExecutableDialogueSO GetNext(int choice = 0)
        {
            if (firstCall)
            {
                // First call, return the current dialogue without moving, otherwise it would be skipped.
                firstCall = false;
                return currentDialogue;
            }

            if (IsEndOfDialogue(choice))
            {
                return null;
            }

            MoveNext(choice);

            return currentDialogue;
        }

        /// <summary>
        /// Returns the current dialogue without advancing.<br/>
        /// </summary>
        public ExecutableDialogueSO GetCurrent()
        {
            return currentDialogue;
        }

        /// <summary>
        /// Returns the previous dialogue by moving back and returning it.<br/>
        /// This method is useful for implementing a back button in your dialogue UI.<br/>
        /// It can be used as much as you want, but it will return null if you're at the first dialogue.<br/>
        /// </summary>
        /// <returns></returns>
        public DialogueSO GetBack()
        {
            // If there are no dialogues in the history, return null
            if (lastDialogueIndex < 0)
            {
                return null;
            }

            MoveBack();

            return currentDialogue;
        }
        #endregion

        #region Moving Helper Methods
        /// <summary>
        /// Moves the dialogue selection to the first dialogue. This will guarantees that the next GetNext() call returns the first dialogue.<br/>
        /// The first dialogue corresponds to the selected dialogue in the inspector.<br/>
        /// You don't need to call this method if this is the first time you're iterating through the dialogue.<br/>
        /// </summary>
        public void ResetToFirstDialogue()
        {
            firstCall = true;
            lastDialogueIndex = -1;

            dialoguesHistory = new List<ExecutableDialogueSO>();
            currentDialogue = ResolveConditionalDialogue(dialogue);
        }

        /// <summary>
        /// Moves to the next dialogue.<br/>
        /// </summary>
        /// <param name="choiceNumber">If you're currently at a multi-choice node, provide the selected option.</param>
        public void MoveNext(int choice = 0)
        {
            if (IsEndOfDialogue(choice))
            {
                Debug.LogWarning("Tried to get the next dialogue but you reached the end!");
                return;
            }

            if (currentDialogue.Type == DialogueType.MultipleChoice)
            {
                // Multiple Choice
                if (choice < 0 || choice >= currentDialogue.Choices.Count)
                {
                    Debug.LogError($"Choice index was invalid! You choose the choice {choice} and there are {currentDialogue.Choices.Count}." +
                        (choice < 0 ? "\nAnd no, negative indexes don't count..." : ""));
                    return;
                }

                dialoguesHistory.Add(currentDialogue);
                lastDialogueIndex++;

                currentDialogue = ResolveConditionalDialogue(currentDialogue.Choices[choice].NextDialogue);
            }
            else
            {
                // Single Choice
                dialoguesHistory.Add(currentDialogue);
                lastDialogueIndex++;

                currentDialogue = ResolveConditionalDialogue(currentDialogue.Choices[0].NextDialogue);
            }
        }

        /// <summary>
        /// Moves back to the previous dialogue. The history is written when the dialogue progresses.<br/>
        /// </summary>
        public void MoveBack()
        {
            if (lastDialogueIndex < 0)
            {
                Debug.LogWarning("There are no dialogues in the history to move back to.");
                return;
            }

            currentDialogue = dialoguesHistory[lastDialogueIndex];
            lastDialogueIndex--;
        }
        #endregion

        /// <summary>
        /// Conditional branches are NOT counted in the history.
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        private ExecutableDialogueSO ResolveConditionalDialogue(DialogueSO dialogue)
        {
            if (dialogue is ExecutableDialogueSO executableDialogue)
            {
                return executableDialogue;
            }

            DialogueConditionalBranchSO conditionalDialogue = dialogue as DialogueConditionalBranchSO;

            // Check if the conditions are met
            bool finalResult = false;

            if (conditionalDialogue.ConditionsToBeMet == ConditionType.All)
            {
                finalResult = true;
                foreach (DialogueConditionData condition in conditionalDialogue.Conditions)
                {
                    if (!condition.Evaluate())
                    {
                        finalResult = false;
                        break;
                    }
                }
            }
            else
            {
                finalResult = false;
                foreach (DialogueConditionData condition in conditionalDialogue.Conditions)
                {
                    if (condition.Evaluate())
                    {
                        finalResult = true;
                        break;
                    }
                    else
                    {
                        Debug.Log("Condition not met: " + condition);
                    }
                }
            }

            // If the conditions are met, move to the true dialogue, otherwise move to the false dialogue.
            if (finalResult)
            {
                return ResolveConditionalDialogue(conditionalDialogue.DialogueOnTrue);
            }
            else
            {
                return ResolveConditionalDialogue(conditionalDialogue.DialogueOnFalse);
            }
        }
    }
}