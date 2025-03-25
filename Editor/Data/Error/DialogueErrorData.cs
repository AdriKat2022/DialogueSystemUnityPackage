using UnityEngine;

namespace AdriKat.DialogueSystem.Graph
{
    public class DialogueErrorData
    {
        public Color Color { get; set; }

        public DialogueErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte)Random.Range(65, 255),
                (byte)Random.Range(50, 176),
                (byte)Random.Range(50, 176),
                255
            );
        }
    }
}