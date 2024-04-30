using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("Текст пользовательского интерфейса для отображения имени игрока")]
        [SerializeField] private Text playerNameText;
        [Tooltip("Слайдер пользовательского интерфейса для отображения состояния здоровья игрока")]
        [SerializeField] private Slider playerHealthSlider;
        [Tooltip("Смещение в пикселях от цели игрока")]
        [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        private PlayerManager _target;
        private float _characterControllerHeight = 0f;
        private Transform _targetTransform;
        private Renderer _targetRenderer;
        private CanvasGroup _canvasGroup;
        private Vector3 _targetPosition;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("CanvasRoom").GetComponent<Transform>(), false);
        }

        private void Update()
        {
            if (_target == null)
            {  
                return;
            }

            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = _target.health;
            }
        }

        private void LateUpdate()
        {
            // Не показывайте пользовательский интерфейс, если камера нас не видит, чтобы избежать возможных ошибок при просмотре пользовательского интерфейса, но не самого плеера.
            if (_targetRenderer != null)
            {
                this._canvasGroup.alpha = _targetRenderer.isVisible ? 1f : 0f;
            }

            // Следуйте за целевым игровым объектом на экране.
            if (_targetTransform != null)
            {
                _targetPosition = _targetTransform.position;
                _targetPosition.y += _characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + screenOffset;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager target)
        {
           this._target = target;

            _targetTransform = this._target.GetComponent<Transform>();
            _targetRenderer = this._target.GetComponentInChildren<Renderer>();

            CharacterController characterController = _target.GetComponent<CharacterController>();

            // Получить данные от проигрывателя, которые не изменятся в течение срока службы этого компонента
            if (characterController != null)
            {
                _characterControllerHeight = characterController.height;
            }

            if (playerNameText != null)
            {
                playerNameText.text = this._target.photonView.Owner.NickName;
            }
        }

        #endregion
    }
}
