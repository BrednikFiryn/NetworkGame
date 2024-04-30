using System;
using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class CameraWork : MonoBehaviour
    {
        [Tooltip("���������� � ��������� ��������� x-z �� ����")]
        [SerializeField] private float distance = 7.0f;

        [Tooltip("������, �� ������� �� �����, ����� ������ ���������� ��� �����")]
        [SerializeField] private float height = 3.0f;

        [Tooltip("��������� ������ ���� ��������� �� ��������� �� ����, ��������, ����� ����� ������ ����� � ��������� ���������� �� �����.")]
        [SerializeField] private Vector3 centerOffset = Vector3.zero;

        [Tooltip("���������� ��� �������� ��� false, ���� ��������� �������� ������ ��������� � ������� Photon Network, � ������� ��������� OnStart Following(), ����� ��� ����������.")]
        [SerializeField] private bool followOnStart = false;

        [Tooltip("�����������, ����������� ������ ��������� �� �����")]
        [SerializeField] private float smoothSpeed = 0.125f;

        // ������������ �������������� �������� �������
        private Transform _cameraTransform;

        // ������������� ���������� ������ ��� ���������� �����������, ���� ���� �������� ��� ������ �����������
        private bool _isFollowing;

        // ��� ��� �������� ������
        private Vector3 _cameraOffset = Vector3.zero;

        private void Start()
        {
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }

        private void LateUpdate()
        {
            // ������ �������������� ����� �� ����������� ��� �������� ������,
            // ������� ��� ����� ��������� ������� ��������, ����� �������� ������ �������� ������ ���, ����� �� ��������� ����� �����, � �������� ������������, ����� ��� ����������
            if (_cameraTransform == null && _isFollowing)
            {
                OnStartFollowing();
            }

            if (_isFollowing) Follow();
        }

        /// ��������� ������� start following.
        /// ����������� ���, ���� �� ����� �������������� �� �� ������, �� ��� �������, ������ ��� ����������, ����������� ����� photon.
        public void OnStartFollowing()
        {
            _cameraTransform = Camera.main.transform;
            _isFollowing = true;
            // �� ������ �� ����������, �� ����� ��������� � ������� �����
            Cut();
        }

        /// <summary>
        /// ������ �������� � ����
        /// </summary>
        private void Follow()
        {
            _cameraOffset.z = -distance;
            _cameraOffset.y = height;

            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, this.transform.position + this.transform.TransformVector(_cameraOffset), smoothSpeed * Time.deltaTime);

            _cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        private void Cut()
        {
            _cameraOffset.z = -distance;
            _cameraOffset.y = height;

            _cameraTransform.position = this.transform.position + this.transform.TransformVector(_cameraOffset);
            _cameraTransform.LookAt(this.transform.position + centerOffset);
        }
    }
}
