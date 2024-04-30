using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("����� ����������������� ���������� ��� ����������� ����� ������")]
        [SerializeField] private Text playerNameText;
        [Tooltip("������� ����������������� ���������� ��� ����������� ��������� �������� ������")]
        [SerializeField] private Slider playerHealthSlider;
        [Tooltip("�������� � �������� �� ���� ������")]
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
            // �� ����������� ���������������� ���������, ���� ������ ��� �� �����, ����� �������� ��������� ������ ��� ��������� ����������������� ����������, �� �� ������ ������.
            if (_targetRenderer != null)
            {
                this._canvasGroup.alpha = _targetRenderer.isVisible ? 1f : 0f;
            }

            // �������� �� ������� ������� �������� �� ������.
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

            // �������� ������ �� �������������, ������� �� ��������� � ������� ����� ������ ����� ����������
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
