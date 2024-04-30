using Photon.Pun;
using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        [SerializeField] private float directionDampTime = 0.25f;
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager ����������� Animator ���������", this);
            }
        }



        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) return;
            SpeedControl();
        }

        private void SpeedControl()
        {
            if (!animator) return;

            // ���������� � ��������
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // ��������� ������ ������ � ��� ������, ���� �� �����.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (vertical < 0) vertical = 0;

            animator.SetFloat("Speed", horizontal * horizontal + vertical * vertical);
            animator.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime);
        }
    }
}
