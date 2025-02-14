using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _obstacleObjects;

    [SerializeField]
    private HunterCharacter _hunterCharacter;

    private void Awake()
    {
        if (_obstacleObjects != null)
        {
            _obstacleObjects.SetActive(true);
        }
        if(_hunterCharacter != null)
        {
            _hunterCharacter.gameObject.SetActive(true);
        }
    }
}
