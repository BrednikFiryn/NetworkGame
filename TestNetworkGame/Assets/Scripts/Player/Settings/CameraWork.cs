using System;
using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class CameraWork : MonoBehaviour
    {
        [Tooltip("Расстояние в локальной плоскости x-z до цели")]
        [SerializeField] private float distance = 7.0f;

        [Tooltip("Высота, на которой мы хотим, чтобы камера находилась над целью")]
        [SerializeField] private float height = 3.0f;

        [Tooltip("Позвольте камере быть смещенной по вертикали от цели, например, чтобы лучше видеть сцену и уменьшить расстояние до земли.")]
        [SerializeField] private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Установите это значение как false, если компонент сборного модуля создается с помощью Photon Network, и вручную вызывайте OnStart Following(), когда это необходимо.")]
        [SerializeField] private bool followOnStart = false;

        [Tooltip("Сглаживание, позволяющее камере следовать за целью")]
        [SerializeField] private float smoothSpeed = 0.125f;

        // кэшированное преобразование целевого объекта
        private Transform _cameraTransform;

        // поддерживайте внутренний флажок для повторного подключения, если цель потеряна или камера переключена
        private bool _isFollowing;

        // Кэш для смещения камеры
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
            // Объект преобразования может не разрушиться при загрузке уровня,
            // поэтому нам нужно учитывать угловые ситуации, когда основная камера меняется каждый раз, когда мы загружаем новую сцену, и повторно подключаться, когда это происходит
            if (_cameraTransform == null && _isFollowing)
            {
                OnStartFollowing();
            }

            if (_isFollowing) Follow();
        }

        /// Запускает событие start following.
        /// Используйте это, если во время редактирования вы не знаете, за чем следить, обычно это экземпляры, управляемые сетью photon.
        public void OnStartFollowing()
        {
            _cameraTransform = Camera.main.transform;
            _isFollowing = true;
            // мы ничего не сглаживаем, мы сразу переходим к нужному кадру
            Cut();
        }

        /// <summary>
        /// Плавно следуйте к цели
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
