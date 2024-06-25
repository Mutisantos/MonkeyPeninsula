using UnityEngine;
using UnityEngine.UI;


public class DisplayScore : MonoBehaviour
{


    public Text playerScoreText;
    public Text enemyScoreText;

    void Start()
    {
        playerScoreText.text = GameManager.Instance.GetPlayerScore().ToString();
        enemyScoreText.text = GameManager.Instance.GetEnemyScore().ToString();
    }

}
