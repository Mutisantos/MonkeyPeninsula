using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	//Contenedor de puntos para moverse en pantalla
	public GameObject PointsContainer;
	//Puntos de recorrido de patrulla
	[SerializeField]
	private List<Transform> Waypoints;
	//Velocidad de movimiento del personaje
	public float step = 0.01f;
	//Para poder mover al jugador y al enemigo 
	public Rigidbody2D CharacterBody;
	public Rigidbody2D EnemyBody;
	public Animator CharacterAnimator;
	public Animator EnemyAnimator;
	public List<AudioClip> SoundEffects;
	public int MiddlePoint = 3;
	public float SeparationDistance = 3.00f;
	public float PositionThreshold = 0.25f;

	[SerializeField]
	private Vector2 NextPosition;
	private bool ChangedTargetPoint = false;
	public int TargetPoint;
	private int currentPlayerScore = 0;
	private int currentEnemyScore = 0;

	// Use this for initialization
	void Start()
	{
		TargetPoint = MiddlePoint;
		if (PointsContainer != null)
		{
			Transform[] points = PointsContainer.GetComponentsInChildren<Transform>();
			Waypoints.Clear();
			for (int i = 1; i < points.Length; i++)
			{
				Waypoints.Add(points[i]);
			}
		}
		NextPosition = Waypoints[MiddlePoint].GetComponent<Transform>().position;

	}

	void FixedUpdate()
	{
		if (GameManager.Instance.IsMovementInProgress()) {
			MoveToNextPosition();
		}
		CheckAndUpdateAnimatorOnDialogInProgress();
		CheckAndUpdateAnimatorOAttackInProgress();
		CheckVictoryOrDefeat();
	}

	private void CheckVictoryOrDefeat()
	{
        if (GameManager.Instance.HasPlayerScoreReachedLimit())
        {
			CharacterAnimator.SetBool("Victory", true);
			EnemyAnimator.SetBool("Defeat", true);
		}
        else if (GameManager.Instance.HasEnemyScoreReachedLimit())
		{
			EnemyAnimator.SetBool("Victory", true);
			CharacterAnimator.SetBool("Defeat", true);
		}
        else
        {
			EnemyAnimator.SetBool("Victory", false);
			CharacterAnimator.SetBool("Victory", false);
			EnemyAnimator.SetBool("Defeat", false);
			CharacterAnimator.SetBool("Defeat", false);
		}
	}

		private void CheckAndUpdateAnimatorOnDialogInProgress()
    {
        if (GameManager.Instance.IsPlayerTurn())
        {
			if (GameManager.Instance.IsInsultInProgress())
			{
				CharacterAnimator.SetBool("InsultInProgress", true);
			}
			else if (GameManager.Instance.IsResponseInProgress())
            {
				EnemyAnimator.SetBool("InsultInProgress", true);
			}
            else
            {
				CharacterAnimator.SetBool("InsultInProgress", false);
				EnemyAnimator.SetBool("InsultInProgress", false);
			}
        }
        else
        {
			if (GameManager.Instance.IsInsultInProgress())
			{
				EnemyAnimator.SetBool("InsultInProgress", true);
			}
			else if (GameManager.Instance.IsResponseInProgress())
			{
				CharacterAnimator.SetBool("InsultInProgress", true);
			}
			else
			{
				CharacterAnimator.SetBool("InsultInProgress", false);
				EnemyAnimator.SetBool("InsultInProgress", false);
			}
		}
    }

	private void CheckAndUpdateAnimatorOAttackInProgress()
	{
		if (GameManager.Instance.IsAttackInProgress())
		{
			CharacterAnimator.SetBool("AttackInProgress",true);
			EnemyAnimator.SetBool("AttackInProgress", true);
			if (!ChangedTargetPoint)
			{
				SoundManager.Instance.RandomizeFx(this.SoundEffects.ToArray());
				if (currentPlayerScore < GameManager.Instance.playerScore)
				{
					currentPlayerScore = GameManager.Instance.playerScore;
					TargetPoint++;
				}
				if (currentEnemyScore < GameManager.Instance.enemyScore)
				{
					currentEnemyScore = GameManager.Instance.enemyScore;
					TargetPoint--;
				}
				NextPosition = Waypoints[TargetPoint].position;
				ChangedTargetPoint = true;
			}
			MoveToNextPosition();
		}
        else
        {
			CharacterAnimator.SetBool("AttackInProgress", false);
			EnemyAnimator.SetBool("AttackInProgress", false);
			ChangedTargetPoint = false;
		}
	}




	public void MoveToNextPosition()
	{
		Vector2 distanceFromWaypoint = new Vector2(SeparationDistance, 0);
		EnemyBody.MovePosition(Vector2.MoveTowards(EnemyBody.position, NextPosition + distanceFromWaypoint, step));
		EnemyAnimator.SetBool("MovementInProgress", true);
		CharacterBody.MovePosition(Vector2.MoveTowards(CharacterBody.position, NextPosition - distanceFromWaypoint, step));
		CharacterAnimator.SetBool("MovementInProgress", true);
		GameManager.Instance.SetMovementInProgress(true);
		float enemyDistance = Vector2.Distance(EnemyBody.position, NextPosition + distanceFromWaypoint);
		float playerDistance = Vector2.Distance(CharacterBody.position, NextPosition - distanceFromWaypoint);
		if (playerDistance < PositionThreshold && enemyDistance < PositionThreshold)
		{
			CharacterAnimator.SetBool("MovementInProgress", false);
			EnemyAnimator.SetBool("MovementInProgress", false);
			GameManager.Instance.SetMovementInProgress(false);
			GameManager.Instance.SetDuelStarted(true);
		}
	}


	public void SetTargetPoint(int index){
		this.MiddlePoint = index;
	}


	

}
