using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InsultTuple
{
    public string insult;
    public string comeback;
}
[System.Serializable]
public class AnswerList
{
    public InsultTuple[] answers;
}

//Singleton para GameManager
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public TextAsset InsultsFile;
    public string InsultsFileName;

    [SerializeField]
    public int playerScore = 0;
    [SerializeField]
    public int enemyScore = 0;
    [SerializeField]
    private Dictionary<string, string> insultsCorrectResponse = new();
    [SerializeField]
    private string currentInsult = "";
    [SerializeField]
    private string currentComeback = "";
    [SerializeField]
    private bool PlayerTurn;
    [SerializeField]
    private bool InsultInProgress;
    [SerializeField]
    private bool ResponseInProgress;
    [SerializeField]
    private bool MovementInProgress;
    [SerializeField]
    private bool AttackInProgress;
    [SerializeField]
    private bool DuelStarted;

    public const int VICTORY_SCORE = 3;
    [Range(0.0f, 0.9f)]
    public float ENEMY_ACCURACY_RATE = 0.5f;

    private void MakeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Awake()
    {
        MakeSingleton(); 
        ResetValues();
        LoadInsults();
    }


    public void ResetValues()
    {
        this.playerScore = 0;
        this.enemyScore = 0;
        System.Random rand = new System.Random();
        this.PlayerTurn = rand.NextDouble() > 0.5;
    }

    public bool HasPlayerScoreReachedLimit()
    {
        return playerScore == VICTORY_SCORE;
    }
    public bool HasEnemyScoreReachedLimit()
    {
        return enemyScore == VICTORY_SCORE;
    }


    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetEnemyScore()
    {
        return enemyScore;
    }
    public string GetCurrentInsult()
    {
        return currentInsult;
    }
    public string GetCurrentComeback()
    {
        return currentComeback;
    }

    public bool IsPlayerTurn()
    {
        return PlayerTurn;
    }


    public int UpdateAndGetPlayerScore()
    {
        this.playerScore++;
        return this.playerScore;
    }

    public int UpdateAndGetEnemyScore()
    {
        this.enemyScore++;
        return this.enemyScore;
    }


    public bool ChangeTurn()
    {
       this.PlayerTurn = !this.PlayerTurn;
       return this.PlayerTurn;
    }

    public void SetInsultInProgress(bool value)
    {
        this.InsultInProgress = value;
    }

    public bool IsInsultInProgress()
    {
        return this.InsultInProgress;
    }


    public void SetMovementInProgress(bool value)
    {
        this.MovementInProgress = value;
    }

    public bool IsMovementInProgress()
    {
        return this.MovementInProgress;
    }
    public void SetResponseInProgress(bool value)
    {
        this.ResponseInProgress = value;
    }
    public bool IsAttackInProgress()
    {
        return this.AttackInProgress;
    }
    public void SetAttackInProgress(bool value)
    {
        this.AttackInProgress = value;
    }

    public bool IsResponseInProgress()
    {
        return this.ResponseInProgress;
    }

    public void SetDuelStarted(bool value)
    {
        this.DuelStarted = value;
    }

    public bool IsDuelStarted()
    {
        return this.DuelStarted;
    }

    private void LoadInsults()
    {
        var jsonTextFile2 = Resources.Load<TextAsset>(InsultsFileName);
        Debug.Log(jsonTextFile2);
        var jsonTextFile = InsultsFile.text;
        AnswerList jsonList = JsonUtility.FromJson<AnswerList>(jsonTextFile);
        foreach (InsultTuple tuple in jsonList.answers)
        {
            this.insultsCorrectResponse.Add(tuple.insult, tuple.comeback);
        }
    }


    public string GetRandomInsult()
    {
        System.Random rand = new System.Random();
        List<string> insults = Enumerable.ToList(this.insultsCorrectResponse.Keys);
        int size = this.insultsCorrectResponse.Count;
        string randomInsult = insults[rand.Next(size)];
        Debug.Log($"Insult: {randomInsult}");
        return randomInsult;
    }
    public string GetRandomAnswer()
    {
        System.Random rand = new System.Random();
        List<string> insults = Enumerable.ToList(this.insultsCorrectResponse.Values);
        int size = this.insultsCorrectResponse.Count;
        string randomInsult = insults[rand.Next(size)];
        Debug.Log($"Enemy prepares: {randomInsult}");
        return randomInsult;
    }


    public bool IsAnswerCorrectComback(string insult, string answer)
    {
        string comeback = this.insultsCorrectResponse[insult];
        Debug.Log($"Expected:{comeback}, Got: {answer}");
        return answer.Equals(comeback);
    }

    public StoryNode BuildStoryNodeByComebacks()
    {
        List<string> allOptions = Enumerable.ToList(this.insultsCorrectResponse.Values);
        currentInsult = GetRandomInsult();
        return GenerateStoryNodeByAnswerOptions(allOptions, currentInsult);
    }

    public StoryNode BuildStoryNodeByInsults()
    {
        List<string> allOptions = Enumerable.ToList(this.insultsCorrectResponse.Keys);
        return GenerateStoryNodeByAnswerOptions(allOptions, currentComeback);
    }

    public string PrepareEnemyResponse(string insult)
    {
        System.Random rand = new System.Random();
        if (rand.NextDouble() < ENEMY_ACCURACY_RATE)
        {
            currentComeback = this.insultsCorrectResponse[insult];
        }
        else { 
            currentComeback = GetRandomAnswer();
        }
        return currentComeback;
    }


    private StoryNode GenerateStoryNodeByAnswerOptions(List<string> allOptions, string history)
    {
        return new StoryNode
        {
            History = history,
            Options = allOptions.ToArray()
        };
    }






}
