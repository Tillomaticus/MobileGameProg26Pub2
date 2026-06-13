using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _roationSpeed = 5f;

    public enum PlayerState
    {
        Idle,
        Walking
    }

    PlayerState _playerState = PlayerState.Idle;


    float _currentHeading = 0;
    float _targetHeading;

    public void SetPlayerState(PlayerState newPlayerState)
    {
        _playerState = newPlayerState;
        SetAnimation();
    }

    void Update()
    {
        // könnten hier auch this.transform.rotation.y statt _currentHeading nehmen, aber das wäre deutlich unleserlicher
        if (_currentHeading == _targetHeading)
            return;

        RotateTowardsTarget();
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.Euler(0, _targetHeading, 0);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * _roationSpeed);

        _currentHeading = this.transform.rotation.y;
    }

    public void AdjustHeight()
    {
        Ray ray = new Ray(Vector3.up * 2000, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    public void SetLookDirection(float heading)
    {
        _targetHeading = heading;
    }


    void SetAnimation()
    {
        if (_playerState == PlayerState.Walking)
            _animator.SetBool("IsWalking", true);
        else
            _animator.SetBool("IsWalking", false);
    }

}
