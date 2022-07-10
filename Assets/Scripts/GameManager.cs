using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }
	private int caveSizeX = 5;
	private int caveSizeY = 5;
	private int repeat = 1500;
	public int currentLevel = 0;
	public bool postProcOn = false;
	public int difficulty = 1;


	public float playerMaxHealth = 10f;
	public float playerHealth = 10f;
	public float playerMaxMana = 5f;
	public float playerMana = 5f;
	public float playerExperience = 0f;
	public float playerExpToLevelUp = 10f;
	public float playerAttackSpeedMultiplier = 1f;
	public float playerMoveSpeedMultiplier = 1f;
	public float playerHealthRegen = 0f;
	public float playerManaRegen = 0f;
	public float playerLifeOnHit = 0f;
    public float playerLifeSteal = 0f;
    public float playerCritChance = 5f;
	public float playerCritDamage = 50f;
	public float playerHandDamage = 1f;
	public float playerSwordDamage = 3f;
	public float playerAxeDamage = 2f;
	public float playerMagicDamage = 0.1f;
	public int playerPoints = 0;
	public bool playerIsRight = true;
	public bool playerIsAlternating = false;
	public float playerChannelingFrequency = 10f;
	public float playerEffectTimeToLive = 1f;
	public float playerEffectRangedSpeed = 1f;
	public float playerZoom = 1f;
	public string playerLeftHand = "";
	public string playerRightHand = "";


	public float monsterDamage = 0.5f;
	public float monsterMaxHealth = 5f;
	public float monsterAttackSpeedMultiplier = 1f;
	public float monsterMoveSpeedMultiplier = 1f;
	public int monsterExperience = 5;

	public string monsterStatsIncreased = "";


	public string playerName { get; set; }
	public int score { get; set; }

	public int CaveSizeX {
		get { return caveSizeX; }
		set {
			caveSizeX = value;
		}
	}

	public int CaveSizeY {
		get { return caveSizeY; }
		set {
			caveSizeY = value;
		}
	}

	public int Repeat {
		get { return repeat; }
		set {
			repeat = value;
		}
	}

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public void AddScore(int points) {
		score += points;
	}

	public void NextLevel() {
		UpdatePlayerStatsInManager();
		IncreaseMonsterStats();
		currentLevel++;
		int addSizeBasedOnDifficulty = Mathf.Min(difficulty, 150);
		caveSizeX = Random.Range(15 + addSizeBasedOnDifficulty, 50 + addSizeBasedOnDifficulty);
		caveSizeY = Random.Range(15 + addSizeBasedOnDifficulty, 50 + addSizeBasedOnDifficulty);
		repeat = 500 + addSizeBasedOnDifficulty * 60;
		
		SceneManager.LoadScene(1);
    }

	private void UpdatePlayerStatsInManager() {
		PlayerController p = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		
		playerMaxHealth = p.MaxHealth;
		playerHealth = p.Health;
		playerMaxMana = p.MaxMana;
		playerMana = p.Mana;
		playerExperience = p.Experience;
		playerExpToLevelUp = p.ExpToLevelUp;
		playerAttackSpeedMultiplier = p.AttackSpeedMultiplier;
		playerMoveSpeedMultiplier = p.MoveSpeedMultiplier;
		playerHealthRegen = p.HealthRegen;
		playerManaRegen = p.ManaRegen;
		playerLifeOnHit = p.LifeOnHit;
		playerLifeSteal = p.LifeSteal;
		playerCritChance = p.CritChance;
		playerCritDamage = p.CritDamage;
		playerHandDamage = p.HandDamage;
		playerSwordDamage = p.SwordDamage;
		playerAxeDamage = p.AxeDamage;
		playerMagicDamage = p.MagicDamage;
		playerPoints = p.Points;
		playerIsRight = p.IsRight;
		playerIsAlternating = p.IsAlternating;
		playerChannelingFrequency = p.ChannelingFrequency;
		playerEffectTimeToLive = p.EffectTimeToLive;
		playerEffectRangedSpeed = p.EffectRangedSpeed;
		playerZoom = p.Zoom;
		if (p.EquippedWeaponLHand) playerLeftHand = p.EquippedWeaponLHand.name;
		if (p.EquippedWeaponRHand) playerRightHand = p.EquippedWeaponRHand.name;
	}

	private void IncreaseMonsterStats() {
		monsterStatsIncreased = "Monster Stats:";

		float tmpDmgInc = 0f;
		float tmpHealthInc = 0f;
		float tmpAttInc = 0f;
		float tmpMoveInc = 0f;
		int tmpExp = 2 + (Mathf.FloorToInt(difficulty / 2));

		for (int i = 0; i < difficulty; i++) {
			int tmpRng = Random.Range(1, 5);
			if (tmpRng == 1) { tmpDmgInc = Mathf.Round((tmpDmgInc + 0.2f) * 10) / 10f; }
			if (tmpRng == 2) { tmpHealthInc = Mathf.Round((tmpHealthInc + 3f) * 10) / 10f; }
			if (tmpRng == 3) { tmpAttInc = Mathf.Round((tmpAttInc + 0.03f) * 100) / 100f; }
			if (tmpRng == 4) { tmpMoveInc = Mathf.Round((tmpMoveInc + 0.02f) * 100) / 100f; }
		}

		monsterDamage = Mathf.Round((monsterDamage + tmpDmgInc) * 10) / 10f;
		monsterMaxHealth = Mathf.Round((monsterMaxHealth + tmpHealthInc) * 10) / 10f;
		monsterAttackSpeedMultiplier = Mathf.Round((monsterAttackSpeedMultiplier + tmpAttInc) * 100) / 100f;
		monsterMoveSpeedMultiplier = Mathf.Round((monsterMoveSpeedMultiplier + tmpMoveInc) * 100) / 100f;
		monsterExperience += tmpExp;

		monsterStatsIncreased += "\n\t- Health: " + monsterMaxHealth + " (+" + tmpHealthInc + ")";
		monsterStatsIncreased += "\n\t- Damage: " + monsterDamage + " (+" + tmpDmgInc + ")";
		monsterStatsIncreased += "\n\t- Attack Speed: " + monsterAttackSpeedMultiplier + " (+" + tmpAttInc + ")";
		monsterStatsIncreased += "\n\t- Move Speed: " + monsterMoveSpeedMultiplier + " (+" + tmpMoveInc + ")";
		monsterStatsIncreased += "\n\t- Experience: " + monsterExperience + " (+" + tmpExp + ")";
	}

	public void ResetPlayerAndMonsterValues() {
		currentLevel = 0;
		monsterStatsIncreased = "";

		playerMaxHealth = 10f;
		playerHealth = 10f;
		playerMaxMana = 5f;
		playerMana = 5f;
		playerExperience = 0f;
		playerExpToLevelUp = 10f;
		playerAttackSpeedMultiplier = 1f;
		playerMoveSpeedMultiplier = 1f;
		playerHealthRegen = 0f;
		playerManaRegen = 0f;
		playerLifeOnHit = 0f;
		playerLifeSteal = 0f;
		playerCritChance = 5f;
		playerCritDamage = 50f;
		playerHandDamage = 1f;
		playerSwordDamage = 3f;
		playerAxeDamage = 2f;
		playerMagicDamage = 0.1f;
		playerPoints = 0;
		playerIsRight = true;
		playerIsAlternating = false;
		playerChannelingFrequency = 10f;
		playerEffectTimeToLive = 1f;
		playerEffectRangedSpeed = 1f;
		playerZoom = 1f;
		playerLeftHand = "";
		playerRightHand = "";


		monsterDamage = 0.5f;
		monsterMaxHealth = 5f;
		monsterAttackSpeedMultiplier = 1f;
		monsterMoveSpeedMultiplier = 1f;
		monsterExperience = 5;
	}
}
