﻿using UnityEngine;
using UnityEngine.UI;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 통해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Text ammoText; // 탄약을 표시할 UI 텍스트
    public Gun gun; // 사용할 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    private Animator playerAnimator; // 애니메이터 컴포넌트

    private PlayerInput playerInput; // 플레이어의 입력
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private void Start() {
        // 사용할 컴포넌트들을 가져온다
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnDisable() {
        // 컴포넌트가 비활성화될때 총을 완전 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전
        if (playerInput.fire)
        {
            // 발사 입력 감지시 총 발사
            gun.Fire();
        }
        else if (playerInput.reload)
        {
            // 재장전 입력 감지시 재장전
            var reloading = gun.Reload();
            if (reloading)
            {
                // 재장전 성공시에만 재장전 애니메이션 재생
                playerAnimator.SetTrigger("Reload");
            }
        }

        UpdateUI(); //남은 탄약 UI를 갱신
    }

    private void UpdateUI() {
        // 총의 탄약에 관한 UI를 갱신
        if (gun != null && ammoText != null)
        {
            // UI 텍스트에 현재 탄창의 탄약과 전체 탄약을 표시
            ammoText.text = gun.magAmmo + "/" + gun.ammoRemain;
        }
    }

    private void OnAnimatorIK(int layerIndex) {
        // 애니메이터의 IK를 갱신한다

        // 총의 기준점 위치를 3D 모델의 오른쪽 팔꿈치 위치로 이동시킨다
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
    }
}