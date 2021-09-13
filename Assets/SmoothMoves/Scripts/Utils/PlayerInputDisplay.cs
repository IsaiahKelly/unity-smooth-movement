using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothMovesDemo
{
    /// <summary>
    /// Displays the player input keys on the UI text.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class PlayerInputDisplay : MonoBehaviour
    {
        [SerializeField]
        private Text _displayText;

        private void Start()
        {
            _displayText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            var playerInput = FindObjectOfType<PlayerInput>();

            if (!playerInput)
                return;

            var sb = new StringBuilder();

            sb.AppendLine("INPUT KEYS");
            sb.AppendLine("Menu: Escape");
            sb.AppendLine("Foward: " + playerInput.ForwardKey);
            sb.AppendLine("Backward: " + playerInput.BackwardKey);
            sb.AppendLine("Strafe Left: " + playerInput.StrafeLeftKey);
            sb.AppendLine("Strafe Right: " + playerInput.StrafeRightKey);
            sb.AppendLine("Jump: " + playerInput.JumpKey);

            _displayText.text = sb.ToString();
        }
    }
}