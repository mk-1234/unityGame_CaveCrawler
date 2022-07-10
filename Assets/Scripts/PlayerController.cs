using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    private UIController uiController;
    public GameObject moveIndicatorPrefab;
    public GameObject swordPrefab;
    public GameObject axePrefab;
    private GameObject equippedWeaponLHand;
    private GameObject equippedWeaponRHand;
    public Transform leftHand;
    public Transform rightHand;
    public Transform spawnPos;
    float frameCounter = 1;
    bool clicked = false;
    private Animator animator;
    private bool isMoving = false;
    private Vector3 finalSpotToMoveTowards;
    private Vector3 playerToFinalPos;
    private GameObject clickedEntity;
    string entityToReach;
    Vector3 entityPosition;
    bool movingToEntity = false;
    bool isAttacking = false;
    public GameObject attackAreaSpherePrefab;
    private float damage;
    private float leftHandDamage;
    private float rightHandDamage;
    private float maxHealth = 10f;
    public float health;
    private float maxMana = 10f;
    private float mana;
    private float experience = 0f;
    private float expToLevelUp = 10f;
    private float baseAttackSpeed = 1f;
    public float attackSpeedMultiplier;
    private float attackSpeed;
    private float baseMoveSpeed = 3f;
    public float moveSpeedMultiplier;
    private float moveSpeed;
    private float attackRange = 1.5f;
    private float healthRegen = 0f;
    private float manaRegen = 0f;
    private float lifeOnHit = 0f;
    private float lifeSteal = 0f;
    private float critChance = 5f;
    private float critDamage = 50f;

    float attackBufferPeriod = 2.0f;
    float elapsedTimeAfterAttack = 0f;
    bool startedCounting = false;
    float attackBufferRange = 0f;

    private float handDamage = 1f;
    private float swordDamage = 3f;
    private float axeDamage = 2f;
    private float magicDamage = 0.1f;
    private float manaCost = 0.3f;

    private int points = 100;

    private bool mouseHoldOnEntity = false;
    private bool isRight = true;
    private bool isAlternating = false;

    private bool isDead = false;
    private Vector3 posAtDeath;

    public float channelingFrequency;
    public float effectTimeToLive = 1f;
    public float effectRangedSpeed = 1f;
    private float timeSinceLastCreated = 0f;

    private float zoom = 1f;
    private float regeneratedHealthInOneSecond = 0f;
    private float timeSpentRegeneratingHealth = 0f;
    private float regeneratedManaInOneSecond = 0f;
    private float timeSpentRegeneratingMana = 0f;


    public float HandDamage {
        get { return handDamage; }
    }
    public float SwordDamage {
        get { return swordDamage; }
    }
    public float AxeDamage {
        get { return axeDamage; }
    }
    public float MagicDamage {
        get { return magicDamage; }
    }
    public float EffectTimeToLive {
        get { return effectTimeToLive; }
    }
    public float EffectRangedSpeed {
        get { return effectRangedSpeed; }
    }
    public float ChannelingFrequency {
        get { return channelingFrequency; }
    }
    public float AttackSpeed {
        get { return attackSpeed; }
    }
    public float AttackSpeedMultiplier {
        get { return attackSpeedMultiplier; }
    }
    public float MoveSpeed {
        get { return moveSpeed; }
    }
    public float MoveSpeedMultiplier {
        get { return moveSpeedMultiplier; }
    }
    public float Health {
        get { return health; }
    }
    public float MaxHealth {
        get { return maxHealth; }
    }
    public float HealthRegen {
        get { return healthRegen; }
    }
    public float Mana {
        get { return mana; }
    }
    public float MaxMana {
        get { return maxMana; }
    }
    public float ManaRegen {
        get { return manaRegen; }
    }
    public float CritChance {
        get { return critChance; }
    }
    public float CritDamage {
        get { return critDamage; }
    }
    public float LifeOnHit {
        get { return lifeOnHit; }
    }
    public float LifeSteal {
        get { return lifeSteal; }
    }
    public float Experience {
        get { return experience; }
    }
    public float ExpToLevelUp {
        get { return expToLevelUp; }
    }
    public float Zoom {
        get { return zoom; }
    }
    public GameObject EquippedWeaponLHand {
        get { return equippedWeaponLHand; }
    }
    public GameObject EquippedWeaponRHand {
        get { return equippedWeaponRHand; }
    }



    public bool IsMoving {
        get { return isMoving; }
    }

    public bool IsRight {
        get { return isRight; }
    }

    public bool IsAlternating {
        get { return isAlternating; }
    }

    public bool IsDead {
        get { return isDead; }
    }

    public float Damage {
        get { return damage; }
    }

    public int Points {
        get { return points; }
    }


    void Awake() {
        cam = Camera.main;
        cam.GetComponent<CameraController>().player = transform;
        uiController = GameObject.Find("BaseUI").GetComponent<UIController>();
        if (uiController) {
            uiController.player = this;
        }
        animator = GetComponent<Animator>();
        InitializePlayerStats();
    }

    void Start() {
        agent.updateRotation = false;
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        CalculateDamage();
    }

    void Update() {

        float dTime = Time.deltaTime;

        if (isDead) {
            transform.position = posAtDeath;
            return;
        }

        UpdateAttackAndMoveSpeed();


        if (health < maxHealth) {
            float tmpValue = healthRegen * dTime;
            health = Mathf.Min(health + tmpValue, maxHealth);
            regeneratedHealthInOneSecond += tmpValue;
            timeSpentRegeneratingHealth += dTime;
            if (timeSpentRegeneratingHealth > 1f || health == maxHealth) {
                timeSpentRegeneratingHealth = 0f;
                if (regeneratedHealthInOneSecond > 0.5f) {
                    uiController.CreateText(2, Mathf.Floor(regeneratedHealthInOneSecond * 10) / 10, cam.WorldToScreenPoint(transform.position + Vector3.up), 1);
                }
                regeneratedHealthInOneSecond = 0f;
            }
        }
        if (mana < maxMana) {
            float tmpValue = manaRegen * dTime;
            mana = Mathf.Min(mana + tmpValue, maxMana);
            regeneratedManaInOneSecond += tmpValue;
            timeSpentRegeneratingMana += dTime;
            if (timeSpentRegeneratingMana > 1f || mana == maxMana) {
                timeSpentRegeneratingMana = 0f;
                if (regeneratedManaInOneSecond > 0.5f) {
                    uiController.CreateText(3, Mathf.Floor(regeneratedManaInOneSecond * 10) / 10, cam.WorldToScreenPoint(transform.position + Vector3.up), 1);
                }
                regeneratedManaInOneSecond = 0f;
            }
        }


        if (Input.anyKeyDown && !uiController.startCountdownForLevelText) uiController.startCountdownForLevelText = true;

        if (Input.mouseScrollDelta.y < -0.1f) {
            zoom = Mathf.Min(4f, zoom + 0.1f);
            cam.GetComponent<CameraController>().AddZoomToOffset(zoom);
            print("zoom: " + zoom + " --> " + Input.mouseScrollDelta.y);
        } else if (Input.mouseScrollDelta.y > 0.1f) {
            zoom = Mathf.Max(zoom - 0.1f, 0.3f);
            cam.GetComponent<CameraController>().AddZoomToOffset(zoom);
            print("zoom: " + zoom + " --> " + Input.mouseScrollDelta.y);
        }


        if (Input.GetKeyDown(KeyCode.Escape)) {
            uiController.ShowMenu = !uiController.ShowMenu;
            if (uiController.ShowMenu) Time.timeScale = 0f; else Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            uiController.ShowStatsPanel = !uiController.ShowStatsPanel;
            if (uiController.ShowStatsPanel) Time.timeScale = 0f; else Time.timeScale = 1f;
        }

        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift) && !isAttacking && !mouseHoldOnEntity &&
            !uiController.ShowMenu && !uiController.ShowStatsPanel) {
            if (!clicked || frameCounter % 10 == 0) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 200f)) {
                    if (hit.collider.CompareTag("Ground")) {
                        Vector3 pos = new Vector3(hit.point.x, 0.1f, hit.point.z);
                        NavMeshHit navHit;
                        if (NavMesh.SamplePosition(pos, out navHit, 100, 1)) {
                            pos = new Vector3(navHit.position.x, pos.y, navHit.position.z);
                        }
                        Instantiate(moveIndicatorPrefab, pos, Quaternion.identity);
                        agent.SetDestination(pos);
                        finalSpotToMoveTowards = pos;
                        agent.isStopped = false;
                        animator.SetBool("isMoving", true);
                        isMoving = true;
                        playerToFinalPos = transform.position - finalSpotToMoveTowards;
                        agent.updateRotation = true;
                        clickedEntity = null;
                        entityToReach = string.Empty;
                        movingToEntity = false;
                        isAttacking = false;
                    }
                    if (hit.collider.CompareTag("Monster") && !clicked) {
                        agent.SetDestination(hit.point);
                        entityPosition = hit.transform.position;
                        clickedEntity = hit.collider.gameObject;
                        finalSpotToMoveTowards = new Vector3(clickedEntity.transform.position.x, 0.5f, clickedEntity.transform.position.z);
                        agent.isStopped = false;
                        animator.SetBool("isMoving", true);
                        isMoving = true;
                        movingToEntity = true;
                        entityToReach = "Monster";
                        agent.updateRotation = true;
                        mouseHoldOnEntity = true;
                    }
                }
                clicked = true;
            }
            frameCounter = frameCounter % 10 == 0 ? 1 : frameCounter + 1;
        }

        if (Input.GetMouseButtonUp(0)) {
            frameCounter = 0;
            clicked = false;
            mouseHoldOnEntity = false;
        }
        if (Input.GetMouseButtonUp(1)) {
            timeSinceLastCreated = 0f;
        }
        if (Input.GetMouseButton(1) && !uiController.ShowMenu && !uiController.ShowStatsPanel) {
            if (isMoving) {
                agent.isStopped = true;
                isMoving = false;
                isAttacking = false;
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] rangedHits;
            rangedHits = Physics.RaycastAll(ray, 100f);
            for (int i = 0; i < rangedHits.Length; i++) {
                if (rangedHits[i].collider.CompareTag("Ground")) {

                    transform.rotation = Quaternion.LookRotation(new Vector3(
                        rangedHits[i].point.x - transform.position.x,
                        0f,
                        rangedHits[i].point.z - transform.position.z
                    ));

                    int repeat = Mathf.FloorToInt(timeSinceLastCreated / (1f / channelingFrequency));

                    if (repeat > 0) {
                        for (int j = 0; j < repeat; j++) {
                            if (mana >= manaCost) {
                                Vector3 rngOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                                Vector3 tmpPos = spawnPos.position + rngOffset + transform.forward * 0.5f;
                                GameObject attackSphere = Instantiate(attackAreaSpherePrefab, tmpPos, transform.rotation);
                                attackSphere.transform.localScale = Vector3.one * 0.5f;
                                attackSphere.GetComponent<AttackAreaSphere>().player = this;
                                attackSphere.GetComponent<AttackAreaSphere>().type = AttackType.RANGED;
                                timeSinceLastCreated = Mathf.Max(0f, timeSinceLastCreated - 1f);
                                mana = Mathf.Max(0f, mana - manaCost);
                            }
                        }
                    } else {
                        timeSinceLastCreated += dTime;
                    }
                }
            }
        }

        if (isMoving) {
            playerToFinalPos = transform.position - finalSpotToMoveTowards;
        }

        if (movingToEntity) {
            if (playerToFinalPos.magnitude < attackRange + attackBufferRange) {
                StartAttack(finalSpotToMoveTowards);
            }
        } else {
            if (playerToFinalPos.magnitude < 0.3f) {
                if (isMoving) {
                    animator.SetBool("isMoving", false);
                    agent.isStopped = true;
                    agent.updateRotation = false;
                    isMoving = false;
                    mouseHoldOnEntity = false;
                }
            }
        }

        if (startedCounting) {
            elapsedTimeAfterAttack += Time.deltaTime;
            if (elapsedTimeAfterAttack > attackBufferPeriod) {
                elapsedTimeAfterAttack = 0f;
                startedCounting = false;
                attackBufferRange = 0f;
            }
        }

        uiController.UpdatePlayerBaseStats();

    }

    void StartAttack(Vector3 goalPos) {
        float x = transform.position.x - goalPos.x;
        float z = transform.position.z - goalPos.z;
        float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;

        animator.SetBool("isMoving", false);
        agent.isStopped = true;
        agent.updateRotation = false;
        isMoving = false;
        movingToEntity = false;
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("isRight", isRight);
        transform.localRotation = Quaternion.Euler(0, angle + 180f, 0);
        agent.updateRotation = false;
    }

    private void CalculateDamage() {
        if (equippedWeaponRHand) {
            string tmpName = equippedWeaponRHand.name;
            if (tmpName == "W_Sword_001(Clone)") {
                rightHandDamage = swordDamage;
            } else if (tmpName == "W_Axe_001(Clone)") {
                rightHandDamage = axeDamage;
            }
        } else {
            rightHandDamage = handDamage;
        }
        if (equippedWeaponLHand) {
            string tmpName = equippedWeaponLHand.name;
            if (tmpName == "W_Sword_001(Clone)") {
                leftHandDamage = swordDamage;
            } else if (tmpName == "W_Axe_001(Clone)") {
                leftHandDamage = axeDamage;
            }
        } else {
            leftHandDamage = handDamage;
        }
        damage = GetActiveHandDamage();
        uiController.UpdatePlayerDamageText();
    }

    private float GetActiveHandDamage() {
        if (isRight) return Mathf.Round(rightHandDamage * 100) / 100f; else return Mathf.Round(leftHandDamage * 100) / 100f;
    }

    private void UpdateAttackAndMoveSpeed() {
        attackSpeed = baseAttackSpeed * attackSpeedMultiplier;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
        animator.SetFloat("attSpeedMult", attackSpeedMultiplier);
        animator.SetFloat("moveSpeedMult", moveSpeedMultiplier);
        agent.speed = moveSpeed;
    }

    public void TakeDamage(float dmg) {
        health = Mathf.Max(0f, health - dmg);
        uiController.UpdatePlayerBaseStats();
        if (health <= 0f) {
            isDead = true;
            animator.SetTrigger("isDead");
            posAtDeath = transform.position;
            agent.enabled = false;
            Destroy(agent);
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<Rigidbody>());

            GameObject forWeaponsTest = new GameObject("HolderObject");
            if (equippedWeaponLHand) {
                equippedWeaponLHand.transform.SetParent(forWeaponsTest.transform);
                Rigidbody leftWeaponRb = equippedWeaponLHand.AddComponent<Rigidbody>();
                equippedWeaponLHand.AddComponent<BoxCollider>();
                leftWeaponRb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
            }
            if (equippedWeaponRHand) {
                equippedWeaponRHand.transform.SetParent(forWeaponsTest.transform);
                Rigidbody rightWeaponRb = equippedWeaponRHand.AddComponent<Rigidbody>();
                equippedWeaponRHand.AddComponent<BoxCollider>();
                rightWeaponRb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
            }
            uiController.ShowGameOverPanel = true;
        }
    }

    public void Event_CreateEffect() {
        foreach (var c in animator.GetCurrentAnimatorClipInfo(0)) {
            if (c.clip.name == "Armature|Swing_1h_r" || c.clip.name == "Armature|Swing_1h_l") {
                GameObject attackSphere = Instantiate(attackAreaSpherePrefab, finalSpotToMoveTowards, Quaternion.identity);
                attackSphere.GetComponent<AttackAreaSphere>().player = this;
                if (clickedEntity) {
                    attackSphere.GetComponent<AttackAreaSphere>().MonsterHashCode = clickedEntity.GetHashCode();
                }
            }
        }
    }

    public void Event_EndingAnim() {
        Monster m = clickedEntity.GetComponent<Monster>();
        if (!mouseHoldOnEntity) {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
        if (m) {
            if (m.IsDead) {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                mouseHoldOnEntity = false;
            }
        }
        if (isAlternating) {
            print("changing side");
            isRight = !isRight;
            animator.SetBool("isRight", isRight);
            CalculateDamage();
        }
        startedCounting = true;
        attackBufferRange = 0.2f;
    }

    public void AddLifeOnHitAndLifeSteal(float damage) {
        if (health < maxHealth) {
            float tmpValue = lifeOnHit + damage * (lifeSteal / 100f);
            if (tmpValue > 0) {
                health = Mathf.Min(health + tmpValue, maxHealth);
                uiController.CreateText(2, Mathf.Round(tmpValue * 10) / 10f, cam.WorldToScreenPoint(transform.position + Vector3.up), 1);
            }
        }
    }

    public void AddExperience(int exp) {
        experience += exp;
        if (experience >= expToLevelUp) {
            LevelUp();
        }
    }

    public void LevelUp() {
        points += 5;
        health = maxHealth;
        mana = maxMana;
        experience = experience - expToLevelUp;
        expToLevelUp = 10 + GameManager.instance.currentLevel * 10;
    }

    public void IncreaseHandDamage() {
        if (points > 0) { handDamage += 0.1f; points--; CalculateDamage(); }
    }

    public void IncreaseAxeDamage() {
        if (points > 0) { axeDamage += 0.1f; points--; CalculateDamage(); }
    }

    public void IncreaseSwordDamage() {
        if (points > 0) { swordDamage += 0.1f; points--; CalculateDamage(); }
    }

    public void IncreaseMagicDamage() {
        if (points > 0) { magicDamage += 0.05f; points--; }
    }

    public void IncreaseMagicDuration() {
        if (points > 0) { effectTimeToLive += 0.05f; points--; }
    }

    public void IncreaseMagicSpeed() {
        if (points > 0) { effectRangedSpeed += 0.02f; points--; }
    }

    public void IncreaseMagicFreq() {
        if (points > 0) { channelingFrequency += 1f; points--; }
    }

    public void IncreaseAttackSpeed() {
        if (points > 0) { attackSpeedMultiplier += 0.02f; points--; UpdateAttackAndMoveSpeed(); }
    }

    public void IncreaseMoveSpeed() {
        if (points > 0) { moveSpeedMultiplier += 0.01f; points--; UpdateAttackAndMoveSpeed(); }
    }

    public void IncreaseMaxHealth() {
        if (points > 0) { maxHealth += 2f; points--; }
    }

    public void IncreaseHealthRegen() {
        if (points > 0) { healthRegen += 0.1f; points--; }
    }

    public void IncreaseLifeOnHit() {
        if (points > 0) { lifeOnHit += 0.2f; points--; }
    }

    public void IncreaseLifeSteal() {
        if (points > 0) { lifeSteal += 0.1f; points--; }
    }

    public void IncreaseCritChance() {
        if (points > 0) { critChance += 0.3f; points--; }
    }

    public void IncreaseCritDamage() {
        if (points > 0) { critDamage += 1f; points--; }
    }

    public void IncreaseMaxMana() {
        if (points > 0) { maxMana += 1f; points--; }
    }

    public void IncreaseManaRegen() {
        if (points > 0) { manaRegen += 0.1f; points--; }
    }


    public void UnequipWeaponLeftHand() {
        Destroy(equippedWeaponLHand);
        equippedWeaponLHand = null;
        CalculateDamage();
    }
    public void EquipAxeLeftHand() {
        if (equippedWeaponLHand) UnequipWeaponLeftHand();
        equippedWeaponLHand = Instantiate(axePrefab, leftHand);
        float tmpScale = 1f / transform.GetChild(0).localScale.x;
        equippedWeaponLHand.transform.localScale = Vector3.one * tmpScale;
        CalculateDamage();
    }
    public void EquipSwordLeftHand() {
        if (equippedWeaponLHand) UnequipWeaponLeftHand();
        equippedWeaponLHand = Instantiate(swordPrefab, leftHand);
        float tmpScale = 1f / transform.GetChild(0).localScale.x;
        equippedWeaponLHand.transform.localScale = Vector3.one * tmpScale;
        CalculateDamage();
    }
    public void UnequipWeaponRightHand() {
        Destroy(equippedWeaponRHand);
        equippedWeaponRHand = null;
        CalculateDamage();
    }
    public void EquipAxeRightHand() {
        if (equippedWeaponRHand) UnequipWeaponRightHand();
        equippedWeaponRHand = Instantiate(axePrefab, rightHand);
        float tmpScale = 1f / transform.GetChild(0).localScale.x;
        equippedWeaponRHand.transform.localScale = Vector3.one * tmpScale;
        CalculateDamage();
    }
    public void EquipSwordRightHand() {
        if (equippedWeaponRHand) UnequipWeaponRightHand();
        equippedWeaponRHand = Instantiate(swordPrefab, rightHand);
        float tmpScale = 1f / transform.GetChild(0).localScale.x;
        equippedWeaponRHand.transform.localScale = Vector3.one * tmpScale;
        CalculateDamage();
    }
    public void SetAttackToLeft() {
        isRight = false;
        CalculateDamage();
    }
    public void SetAttackToRight() {
        isRight = true;
        CalculateDamage();
    }
    public void SetAttackToAlternating() {
        isAlternating = !isAlternating;
        CalculateDamage();
    }


    private void InitializePlayerStats() {
        maxHealth = GameManager.instance.playerMaxHealth;
        health = GameManager.instance.playerHealth;
        maxMana = GameManager.instance.playerMaxMana;
        mana = GameManager.instance.playerMana;
        experience = GameManager.instance.playerExperience;
        expToLevelUp = GameManager.instance.playerExpToLevelUp;
        attackSpeedMultiplier = GameManager.instance.playerAttackSpeedMultiplier;
        moveSpeedMultiplier = GameManager.instance.playerMoveSpeedMultiplier;
        healthRegen = GameManager.instance.playerHealthRegen;
        manaRegen = GameManager.instance.playerManaRegen;
        lifeOnHit = GameManager.instance.playerLifeOnHit;
        lifeSteal = GameManager.instance.playerLifeSteal;
        critChance = GameManager.instance.playerCritChance;
        critDamage = GameManager.instance.playerCritDamage;
        handDamage = GameManager.instance.playerHandDamage;
        swordDamage = GameManager.instance.playerSwordDamage;
        axeDamage = GameManager.instance.playerAxeDamage;
        magicDamage = GameManager.instance.playerMagicDamage;
        points = GameManager.instance.playerPoints;
        isRight = GameManager.instance.playerIsRight;
        isAlternating = GameManager.instance.playerIsAlternating;
        channelingFrequency = GameManager.instance.playerChannelingFrequency;
        effectTimeToLive = GameManager.instance.playerEffectTimeToLive;
        effectRangedSpeed = GameManager.instance.playerEffectRangedSpeed;
        zoom = GameManager.instance.playerZoom;

        string tmpLeftHand = GameManager.instance.playerLeftHand;
        string tmpRightHand = GameManager.instance.playerRightHand;

        if (tmpLeftHand == "W_Sword_001(Clone)") {
            EquipSwordLeftHand();
        } else if (tmpLeftHand == "W_Axe_001(Clone)") {
            EquipAxeLeftHand();
        } else {
            UnequipWeaponLeftHand();
        }

        if (tmpRightHand == "W_Sword_001(Clone)") {
            EquipSwordRightHand();
        } else if (tmpRightHand == "W_Axe_001(Clone)") {
            EquipAxeRightHand();
        } else {
            UnequipWeaponRightHand();
        }
    }
}
