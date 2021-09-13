using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SmoothMovesDemo
{
    /// <summary>
    /// Simple script to manage various game states and settings.
    /// </summary>
    public class GameManger : MonoBehaviour
    {
        public bool IsPaused = false;
        public UnityEvent<bool> OnChangePauseState;

        private void Start()
        {
            SetPauseState(IsPaused);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                TogglePauseState();
            }
        }

        public void TogglePauseState()
        {
            IsPaused = !IsPaused;
            SetPauseState(IsPaused);
        }

        public void SetPauseState(bool paused)
        {
            Cursor.visible = paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            OnChangePauseState?.Invoke(IsPaused);
            AudioListener.pause = paused;
            Time.timeScale = paused ? 0.0f : 1.0f;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            // Exit play mode if in the editor.
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }
    }
}