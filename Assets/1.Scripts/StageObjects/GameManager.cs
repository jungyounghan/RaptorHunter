using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private State _state;
    [SerializeField]
    private GameObject _obstacleObjects;
    [SerializeField]
    private Controller _playerController;

    private void Awake()
    {
        if (_obstacleObjects != null)
        {
            _obstacleObjects.SetActive(true);
        }
        if (_playerController != null)
        {
            _playerController.Initialize(SetStamina, SetLife);
            _playerController.gameObject.SetActive(true);
            _playerController.Revive();
        }
    }

    public void SetLife(uint current, uint max)
    {
        _state.SetLife(current, max);
    }

    public void SetStamina(float current, float max)
    {
        _state.SetStamina(current, max);
    }
}