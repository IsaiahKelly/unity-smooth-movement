using UnityEngine;
using UnityEngine.UI;

namespace SmoothMovesDemo.Utils
{
    /// <summary>
    /// Displays average frames per second.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class FrameRateCounter : MonoBehaviour
    {
        private const float _updateInterval = 0.5f;
        private const string _display = "{0} FPS";

        private float _nextUpdateTime = 0.0f;
        private int _currentRate = 0;
        private int _accumulatorRate = 0;
        private Text _displayText;

        private void Start()
        {
            _nextUpdateTime = Time.realtimeSinceStartup + _updateInterval;
            _displayText = GetComponent<Text>();
        }

        private void Update()
        {
            _accumulatorRate++;

            if (Time.realtimeSinceStartup > _nextUpdateTime)
            {
                _currentRate = (int)(_accumulatorRate / _updateInterval);
                _accumulatorRate = 0;
                _nextUpdateTime += _updateInterval;
                _displayText.text = string.Format(_display, _currentRate);
            }
        }
    }
}