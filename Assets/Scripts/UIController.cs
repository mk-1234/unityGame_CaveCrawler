using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject damageTextPrefab;
    private List<GameObject> texts = new List<GameObject>();
    public PlayerController player;
    [SerializeField] private TextMeshProUGUI playerDamageText;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Image statsPanelImage;
    [SerializeField] GameObject baseStats;
    [SerializeField] GameObject playerStats;
    private bool showMenu;
    private bool showStatsPanel;
    private bool showGameOverPanel;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI monsterStatsText;
    private float durationOfLevelText = 3f;
    public bool startCountdownForLevelText = false;

    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI playerManaText;
    [SerializeField] TextMeshProUGUI playerExpText;

    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] TextMeshProUGUI handDamageText;
    [SerializeField] TextMeshProUGUI axeDamageText;
    [SerializeField] TextMeshProUGUI swordDamageText;
    [SerializeField] TextMeshProUGUI magicDamageText;
    [SerializeField] TextMeshProUGUI magicSpeedText;
    [SerializeField] TextMeshProUGUI magicDurationText;
    [SerializeField] TextMeshProUGUI magicFreqText;
    [SerializeField] TextMeshProUGUI attackSpeedText;
    [SerializeField] TextMeshProUGUI moveSpeedText;
    [SerializeField] TextMeshProUGUI maxHealthText;
    [SerializeField] TextMeshProUGUI healthRegenText;
    [SerializeField] TextMeshProUGUI lifeOnHitText;
    [SerializeField] TextMeshProUGUI lifeStealText;
    [SerializeField] TextMeshProUGUI maxManaText;
    [SerializeField] TextMeshProUGUI manaRegenText;
    [SerializeField] TextMeshProUGUI critChanceText;
    [SerializeField] TextMeshProUGUI critDamageText;

    public bool ShowMenu {
        get {
            return showMenu;
        }
        set {
            showMenu = value;
            menuPanel.SetActive(showMenu);
        }
    }

    public bool ShowGameOverPanel {
        get {
            return showGameOverPanel;
        }
        set {
            showGameOverPanel = value;
            gameOverPanel.SetActive(showGameOverPanel);
        }
    }

    public bool ShowStatsPanel {
        get {
            return showStatsPanel;
        }
        set {
            showStatsPanel = value;
            UpdateStats();
            playerStats.SetActive(showStatsPanel);
            if (showStatsPanel) {
                statsPanelImage.enabled = true;
            } else {
                statsPanelImage.enabled = false;
            }
        }
    }

    private void Awake() {
        menuPanel.SetActive(showMenu);
        baseStats.SetActive(!showStatsPanel);
        playerStats.SetActive(showStatsPanel);
        gameOverPanel.SetActive(showGameOverPanel);
    }

    private void Start() {
        levelText.text = ("LEVEL " + (GameManager.instance.currentLevel + 1));
        monsterStatsText.text = GameManager.instance.monsterStatsIncreased;
    }

    private void Update() {

        if (startCountdownForLevelText && levelText) {
            if (durationOfLevelText <= 0f) {
                Destroy(levelText.gameObject);
                Destroy(monsterStatsText.gameObject);
            } else {
                durationOfLevelText -= Time.deltaTime;
            }
        }

        if (texts.Count > 0) {
            for (int i = texts.Count - 1; i >= 0; i--) {
                DamageText t = texts[i].GetComponent<DamageText>();
                if (t.Duration > 0f) {
                    t.Move();
                } else {
                    texts.RemoveAt(i);
                    Destroy(t.gameObject);
                }
            }
        }
    }

    public void CreateText(int entityType, float damage, Vector3 startPos, float crit) {
        GameObject damageText = Instantiate(damageTextPrefab, transform);
        damageText.transform.position = startPos;
        TextMeshProUGUI text = damageText.GetComponent<TextMeshProUGUI>();
        text.SetText((Mathf.Round(damage * crit * 10) / 10f).ToString());
        DamageText dText = damageText.GetComponent<DamageText>();
        if (entityType == 0) {
            dText.InitialFontSize = 16f;
            dText.FontSizeIncreaseFactor = 2f;
        } else {
            dText.InitialFontSize = 32f;
            dText.FontSizeIncreaseFactor = 4f;
        }
        if (crit > 1) {
            text.color = entityType == 0 ? new Color(1f, 0f, 0f) : new Color(1f, 0.6f, 0f);
            dText.IsCrit = true;
        } else {
            if (entityType == 0) {
                text.color = new Color(0.6f, 0f, 0f);
            } else if (entityType == 1) {
                text.color = new Color(1f, 1f, 0f);
            } else if (entityType == 2) {
                text.color = new Color(0.2f, 0.7f, 0f);
            } else {
                text.color = new Color(0f, 0.2f, 0.7f);
            }
            dText.IsCrit = false;
        }
        texts.Add(damageText);
    }

    public void UpdatePlayerDamageText() {
        playerDamageText.text = (
            "Damage: " + player.Damage + 
            "\nAlternating: " + player.IsAlternating + 
            "\nActive Hand: " + (player.IsRight ? "Right" : "Left") +
            "\n\nLeft Hand: " + (player.EquippedWeaponLHand ? 
                (player.EquippedWeaponLHand.name == "W_Axe_001(Clone)" ? "Axe" : 
                    (player.EquippedWeaponLHand.name == "W_Sword_001(Clone)" ? "Sword" : "No Weapon")) : "No Weapon") +
            "\nRight Hand: " + (player.EquippedWeaponRHand ?
                (player.EquippedWeaponRHand.name == "W_Axe_001(Clone)" ? "Axe" :
                    (player.EquippedWeaponRHand.name == "W_Sword_001(Clone)" ? "Sword" : "No Weapon")) : "No Weapon")
            );
    }

    public void UpdatePlayerBaseStats() {
        playerHealthText.text = "Health: " + (Mathf.Floor(player.Health * 10) / 10) + " / " + player.MaxHealth;
        playerManaText.text = "Mana: " + (Mathf.Floor(player.Mana * 10) / 10) + " / " + player.MaxMana;
        playerExpText.text = "Experience: " + player.Experience + " / " + player.ExpToLevelUp;
    }

    public void UpdateStats() {
        pointsText.text = "Attribute Points: " + player.Points;
        handDamageText.text = "Hand Damage: " + (Mathf.Round(player.HandDamage * 10) / 10);
        axeDamageText.text = "Axe Damage: " + (Mathf.Round(player.AxeDamage * 10) / 10);
        swordDamageText.text = "Sword Damage: " + (Mathf.Round(player.SwordDamage * 10) / 10);
        magicDamageText.text = "Magic Damage: " + (Mathf.Round(player.MagicDamage * 100) / 100);
        magicSpeedText.text = "Magic Speed: " + (Mathf.Round(player.EffectRangedSpeed * 100) / 100);
        magicDurationText.text = "Magic Duration: " + (Mathf.Round(player.EffectTimeToLive * 100) / 100) + "s";
        magicFreqText.text = "Magic Frequency: " + (Mathf.Round(player.ChannelingFrequency * 10) / 10) + "/s";
        attackSpeedText.text = "Attack Speed: " + (Mathf.Round(player.AttackSpeed * 100) / 100) + "/s";
        moveSpeedText.text = "Move Speed: " + (Mathf.Round(player.MoveSpeed * 100) / 100);
        maxHealthText.text = "Max Health: " + (Mathf.Round(player.MaxHealth * 10) / 10);
        healthRegenText.text = "Health Regeneration: " + (Mathf.Round(player.HealthRegen * 10) / 10) + "/s";
        lifeOnHitText.text = "Life On Hit: " + (Mathf.Round(player.LifeOnHit * 10) / 10) + "/hit";
        lifeStealText.text = "Life Steal: " + (Mathf.Round(player.LifeSteal * 10) / 10) + "%";
        maxManaText.text = "Max Mana: " + (Mathf.Round(player.MaxMana * 10) / 10);
        manaRegenText.text = "Mana Regeneration: " + (Mathf.Round(player.ManaRegen * 10) / 10) + "/s";
        critChanceText.text = "Critical Chance: " + (Mathf.Round(player.CritChance * 10) / 10) + "%";
        critDamageText.text = "Critical Damage: +" + (Mathf.Round(player.CritDamage * 10) / 10) + "%";
    }
}
