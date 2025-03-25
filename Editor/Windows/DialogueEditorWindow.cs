using AdriKat.DialogueSystem.Graph;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Utility
{
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueGraphView graphView;
        private readonly string defaultFileName = "DialogueFileName";
        private static TextField fileNameTextField;
        private Button saveButton;

        [MenuItem("Window/DialogueEditorWindow")]
        public static void ShowExample()
        {
            DialogueEditorWindow wnd = GetWindow<DialogueEditorWindow>("DialogueEditorWindow");
            //wnd.titleContent = new GUIContent();
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }

        public static void UpdateFileName(string fileName)
        {
            fileNameTextField.value = fileName;
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new();
            fileNameTextField = DialogueElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = DialogueElementUtility.CreateButton("Save", () => Save());
            Button loadButton = DialogueElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DialogueElementUtility.CreateButton("Clear", () => Clear());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);

            //toolbar.AddStyleSheets("DialogueToolBarStyles");
            toolbar.AddStyleSheetsGUIDs("56e71c5ed8a693f4293ce0ee78db317d");
            rootVisualElement.Add(toolbar);
        }


        #region Toolbar Methods
        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name", "Ok");
                return;
            }

            DialogueIOUtility.Initialize(graphView, fileNameTextField.value);
            DialogueIOUtility.Save();
        }

        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Dialogue Graphs", DialogueIOUtility.GRAPHS_SAVE_PATH, "asset");

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Invalid file path");
                return;
            }

            Clear();
            DialogueIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(path));
            DialogueIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
            fileNameTextField.value = defaultFileName;
        }
        #endregion

        private void AddStyles()
        {
            //rootVisualElement.AddStyleSheets("DialogueVariables");
            rootVisualElement.AddStyleSheetsGUIDs("aba5c975eccbb86409b93f173fbcf6ab");
        }

        private void AddGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
    }
}