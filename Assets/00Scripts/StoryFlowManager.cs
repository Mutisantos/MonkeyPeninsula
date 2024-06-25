using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public enum DialogMode
{
    QUEST_MODE, DUEL_MODE
}

public class StoryFlowManager : MonoBehaviour
{
    public Text EnemyHistoryText;
    public Text PlayerHistoryText;
    public float ButtonThreshold;
    public float WidthMargin;
    public float Height;
    public DialogMode currentMode;
    public GameObject ButtonContinue;
    public GameObject ButtonAnswerPrefab;
    public GameObject PlayerTextParent;
    public GameObject EnemyTextParent;
    public GameObject AnswersParent;
    public Transform ButtonScrollerPanel;

    private StoryNode currentNode;
    private bool EnableUIElements = false;

    private void Start()
    {

        if (currentMode.Equals(DialogMode.DUEL_MODE))
        {
            GameManager.Instance.ResetValues();
            if (GameManager.Instance.IsPlayerTurn())
            {
                currentNode = GameManager.Instance.BuildStoryNodeByInsults();
                EnemyHistoryText.text = string.Empty;
            }
            else
            {
                currentNode = GameManager.Instance.BuildStoryNodeByComebacks();
                EnemyHistoryText.text = "\n\n" + currentNode.History;
            }
        }
        else
        {
            currentNode = StoryFiller.FillStory();
        }
        FillUi();
        HideUIElementsOnStart();
        GameManager.Instance.SetMovementInProgress(true);
    }


    private void Update()
    {
        if (GameManager.Instance.IsDuelStarted() && !this.EnableUIElements)
        {
            if (GameManager.Instance.IsPlayerTurn())
            {
                this.PlayerTextParent.SetActive(true);
                this.AnswersParent.SetActive(true);
                this.EnemyTextParent.SetActive(false);
            }
            else
            {
                this.PlayerTextParent.SetActive(false);
                this.AnswersParent.SetActive(false);
                this.EnemyTextParent.SetActive(true);
                StartCoroutine(HandleEnemyAttack());
            }
            this.EnableUIElements = true;
        }
    }

    private void HideUIElementsOnStart()
    {
        ButtonContinue.SetActive(false);
        this.EnemyTextParent.SetActive(false);
        this.PlayerTextParent.SetActive(false);
        this.AnswersParent.SetActive(false);
        this.EnableUIElements = false;
        EnemyHistoryText.text = string.Empty;
        PlayerHistoryText.text = string.Empty;
    }

    private void FillUi()
    {
        ClearOptions();
        var index = 0;
        var initialHeight = 1 - ButtonThreshold;
        foreach (var answer in currentNode.Options)
        {
            index = addNewButtonOption(index, initialHeight, answer);
        }
    }


    private int addNewButtonOption(int index, float initialHeight, string answer)
    {
        var buttonAnswerCopy = Instantiate(ButtonAnswerPrefab);
        buttonAnswerCopy.transform.SetParent(ButtonScrollerPanel);
        buttonAnswerCopy.GetComponent<RectTransform>().anchorMax
            = new Vector2(1 - WidthMargin, initialHeight - (Height * index));
        buttonAnswerCopy.GetComponent<RectTransform>().anchorMin
            = new Vector2(WidthMargin, initialHeight - (Height * (index + 1)));
        buttonAnswerCopy.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        buttonAnswerCopy.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        FillListener(buttonAnswerCopy.GetComponent<Button>(), index);
        buttonAnswerCopy.GetComponentInChildren<Text>().text = answer;
        index++;
        return index;
    }

    private void FillListener(Button button, int index)
    {
        button.onClick.AddListener(() => { AnswerSelected(index); });
    }

    private void AnswerSelected(int index)
    {
        if (currentMode.Equals(DialogMode.QUEST_MODE))
        {
            ProcessStoryNode(index);
        }
        else if (currentMode.Equals(DialogMode.DUEL_MODE))
        {
            if (GameManager.Instance.IsPlayerTurn())
            {
                StartCoroutine(ProcessPlayerTurnDuelNode(index));
            }
            else
            {
                StartCoroutine(ProcessEnemyTurnDuelNode(index));
            }

        }
    }

    IEnumerator HandleEnemyAttack()
    {
        EnemyHistoryText.text = "\n\n" + currentNode.History;
        PlayerTextParent.SetActive(false);
        GameManager.Instance.SetInsultInProgress(true);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.SetInsultInProgress(false);
        AnswersParent.SetActive(true);

    }

    IEnumerator ProcessEnemyTurnDuelNode(int index)
    {
        PlayerHistoryText.text = "\n\n" + currentNode.Options[index];
        EnemyHistoryText.text = "\n\n" + currentNode.History;
        AnswersParent.SetActive(false);
        PlayerTextParent.SetActive(true);
        GameManager.Instance.SetResponseInProgress(true);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.SetResponseInProgress(false);

        if (GameManager.Instance.IsAnswerCorrectComback(GameManager.Instance.GetCurrentInsult(), currentNode.Options[index]))
        {
            // El jugador responde bien, ahora es su turno de insultar
            // Ahora el listado es el de los insultos y el enemigo debe de traer una respuesta aleatoria
            GameManager.Instance.SetAttackInProgress(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.SetAttackInProgress(false);
            ChangeTurnForPlayer();
        }
        else
        {
            //El jugador no replica correctamente, es un punto para el enemigo
            GameManager.Instance.UpdateAndGetEnemyScore();
            GameManager.Instance.SetAttackInProgress(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.SetAttackInProgress(false);
            AnswersParent.SetActive(true);
            if (GameManager.Instance.HasEnemyScoreReachedLimit())
            {
                ProcessDuelResult("\n\n Heh, tu espada es tán patética como tus palabras. Te faltan 1000 años para derrotarme.");
            }
            else
            {
                // El juego continua, el enemigo sigue insultando, el jugador intenta defenderse
                PrepareEnemyTurn();
            }
        }
    }


    IEnumerator ProcessPlayerTurnDuelNode(int index)
    {
        AnswersParent.SetActive(false);
        EnemyTextParent.SetActive(false);
        PlayerTextParent.SetActive(true);
        //Los parametros se invierten, el insulto está del lado del jugador al seleccionarla
        // la respuesta está proferida por el enemigo.
        PlayerHistoryText.text += "\n\n" + currentNode.Options[index];
        EnemyHistoryText.text = "\n\n" + GameManager.Instance.PrepareEnemyResponse(currentNode.Options[index]);
        //Insulta el jugador
        GameManager.Instance.SetInsultInProgress(true);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.SetInsultInProgress(false);
        //Responde el enemigo
        GameManager.Instance.SetResponseInProgress(true);
        EnemyTextParent.SetActive(true);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.SetResponseInProgress(false);
        if (GameManager.Instance.IsAnswerCorrectComback(currentNode.Options[index], GameManager.Instance.GetCurrentComeback()))
        {
            GameManager.Instance.SetAttackInProgress(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.SetAttackInProgress(false);
            AnswersParent.SetActive(true);
            ChangeTurnForEnemy();
        }
        else
        {
            //El enemigo falla en dar una respuesta correcta. Punto para el Jugador.
            GameManager.Instance.UpdateAndGetPlayerScore();
            GameManager.Instance.SetAttackInProgress(true);
            yield return new WaitForSeconds(3f);
            GameManager.Instance.SetAttackInProgress(false);
            AnswersParent.SetActive(true);
            if (GameManager.Instance.HasPlayerScoreReachedLimit())
            {
                ProcessDuelResult("\n\n De acuerdo, me rindo, tu habilidad con la espada es impresionante.");
            }
            else
            {
                // El juego continua, el jugador sigue insultando, el enemigo intenta defenderse
                PreparePlayerTurn();
            }
        }
    }

    private void PreparePlayerTurn()
    {
        PlayerHistoryText.text = string.Empty;
        currentNode = GameManager.Instance.BuildStoryNodeByInsults();
        currentNode.OnNodeVisited?.Invoke();
        FillUi();
        EnemyTextParent.SetActive(false);
        PlayerTextParent.SetActive(false);
    }

    private void PrepareEnemyTurn()
    {
        currentNode = GameManager.Instance.BuildStoryNodeByComebacks();
        currentNode.OnNodeVisited?.Invoke();
        EnemyHistoryText.text = currentNode.History;
        PlayerHistoryText.text = string.Empty;
        FillUi();
    }

    private void ProcessDuelResult(string enemyResultQuote)
    {
        EnemyHistoryText.text = enemyResultQuote;
        ClearOptions();
        ButtonContinue.SetActive(true);
        AnswersParent.SetActive(false);
    }

    private void ChangeTurnForEnemy()
    {
        GameManager.Instance.ChangeTurn();
        currentNode = GameManager.Instance.BuildStoryNodeByComebacks();
        currentNode.OnNodeVisited?.Invoke();
        EnemyHistoryText.text = currentNode.History;
        PlayerHistoryText.text = string.Empty;
        PlayerTextParent.SetActive(false);
        FillUi();
    }

    private void ChangeTurnForPlayer()
    {
        AnswersParent.SetActive(true);
        EnemyTextParent.SetActive(false);
        PlayerTextParent.SetActive(false);
        GameManager.Instance.ChangeTurn();
        currentNode = GameManager.Instance.BuildStoryNodeByInsults();
        currentNode.OnNodeVisited?.Invoke();
        EnemyHistoryText.text = string.Empty;
        PlayerHistoryText.text = string.Empty;
        FillUi();
    }

    private void ProcessStoryNode(int index)
    {
        EnemyHistoryText.text += "\n" + currentNode.Options[index];
        if (!currentNode.IsFinal)
        {
            currentNode = currentNode.NextNode[index];
            currentNode.OnNodeVisited?.Invoke();
            FillUi();
        }
        else
        {
            EnemyHistoryText.text += "\n" + "TOCA ESCAPE PARA CONTINUAR";
        }
    }

    private void ClearOptions()
    {
        foreach (Transform child in ButtonScrollerPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


}
