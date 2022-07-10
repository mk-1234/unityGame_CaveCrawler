using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public NavMeshAgent agent;
    public NavMeshObstacle obstacle;
    public Vector3 goalPos;
    public bool isMoving;
    private Animator anim;
    public bool isAttacking;
    private PlayerController player;
    public float aggrRange = 3f;
    public float attackRange = 1f;
    private float restTime = 3f;
    private float timePassed = 0f;
    private bool resting = false;
    public Canvas healthBarCanvas;
    public Image healthBarBackgroundImage;
    public Image healthBarImage;
    private float maxHealth;
    private float health;
    private Vector3 posToAttack;

    float attackBufferPeriod = 2.0f;
    float elapsedTimeAfterAttack = 0f;
    bool startedCounting = false;
    float attackBufferRange = 0f;

    public GameObject attackAreaSpherePrefab;
    private float damage;
    private float baseAttackSpeed = 1f;
    public float attackSpeedMultiplier;
    private float attackSpeed;
    private float baseMoveSpeed = 1f;
    public float moveSpeedMultiplier;
    private float moveSpeed;

    private bool isDead;
    private int experience;
    private Vector3 posAtDeath;

    private Material material;
    private Color initialMaterialColor;
    private Color hoverMaterialColor = new Color(0.6f, 0f, 0f);

    public bool IsDead {
        get { return isDead; }
    }
    public float Damage {
        get { return damage; }
    }

    private void Awake() {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        healthBarBackgroundImage.enabled = false;
        healthBarImage.enabled = false;
        material = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
        if (!material) {
            print("failed to get material!");
        } else {
            initialMaterialColor = material.color;
        }
        InitializeMonsterStats();
        health = maxHealth;
    }

    private void InitializeMonsterStats() {
        damage = GameManager.instance.monsterDamage;
        attackSpeedMultiplier = GameManager.instance.monsterAttackSpeedMultiplier;
        moveSpeedMultiplier = GameManager.instance.monsterMoveSpeedMultiplier;
        maxHealth = GameManager.instance.monsterMaxHealth;
        experience = GameManager.instance.monsterExperience;
    }

    private void Start() {

    }

    private void OnMouseEnter() {
        if (material) {
            material.color = hoverMaterialColor;
        }
    }

    private void OnMouseExit() {
        if (material) {
            material.color = initialMaterialColor;
        }
    }

    void Update() {

        if (isDead) {
            transform.position = posAtDeath;
            return;
        }

        attackSpeed = baseAttackSpeed * attackSpeedMultiplier;
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
        anim.SetFloat("attSpeedMult", attackSpeedMultiplier);
        anim.SetFloat("moveSpeedMult", moveSpeedMultiplier);
        agent.speed = moveSpeed;

        if (health < maxHealth) {
            if (!healthBarBackgroundImage.enabled) {
                healthBarBackgroundImage.enabled = true;
                healthBarImage.enabled = true;
            }
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            healthBarBackgroundImage.transform.position = new Vector3(screenPoint.x, screenPoint.y + 50f, screenPoint.z);
            healthBarImage.transform.localScale = new Vector3(health / maxHealth, healthBarImage.transform.localScale.y, healthBarImage.transform.localScale.z);
        }

        if (!isMoving && !isAttacking) {
            if (IsPlayerInAttackRange()) {
                if (!player.GetComponent<PlayerController>().IsDead) {
                    anim.SetBool("isAttacking", true);
                    if (agent.enabled) { agent.isStopped = true; }
                    isAttacking = true;
                    agent.enabled = false;
                    obstacle.enabled = true;
                    transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                    timePassed = 0f;
                    resting = false;
                    posToAttack = player.transform.position;
                } else {
                    resting = true;
                }
            } else if (IsPlayerInRange()) {
                obstacle.enabled = false;
                agent.enabled = true;
                goalPos = player.transform.position;
                agent.SetDestination(goalPos);
                agent.isStopped = false;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isMoving", true);
                isMoving = true;
                timePassed = 0f;
                resting = false;
            } else {
                if (!resting) {
                    Vector2 rPos = Random.insideUnitCircle * 10f;
                    goalPos = new Vector3(transform.position.x + rPos.x, transform.position.y, transform.position.z + rPos.y);

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(goalPos, out hit, 100, 1)) {
                        goalPos = new Vector3(hit.position.x, transform.position.y, hit.position.z);
                    }

                    obstacle.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(goalPos);
                    agent.isStopped = false;
                    anim.SetBool("isMoving", true);
                    isMoving = true;
                }
            }
        }

        if (isMoving && !isAttacking) {
            if (IsPlayerInAttackRange()) {
                if (!player.IsDead) {
                    anim.SetBool("isMoving", false);
                    anim.SetBool("isAttacking", true);
                    agent.isStopped = true;
                    isMoving = false;
                    agent.enabled = false;
                    obstacle.enabled = true;
                    isAttacking = true;
                    transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                    timePassed = 0f;
                    resting = false;
                    posToAttack = player.transform.position;
                } else {
                    resting = true;
                }
            } else if (IsPlayerInRange()) {
                goalPos = player.transform.position;
                agent.SetDestination(goalPos);
                agent.isStopped = false;
                anim.SetBool("isMoving", true);
                timePassed = 0f;
                resting = false;
            } else {
                if ((transform.position - goalPos).magnitude < 0.3f) {
                    agent.isStopped = true;
                    isMoving = false;
                    agent.enabled = false;
                    obstacle.enabled = true;
                    anim.SetBool("isMoving", false);
                    resting = true;
                }
            }
        }

        if (resting) {
            timePassed += Time.deltaTime;
            if (timePassed >= restTime) {
                timePassed = 0f;
                resting = false;
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
    }

    private bool IsPlayerInRange() {
        return Vector3.Distance(transform.position, player.transform.position) + (player.IsDead ? 1000f : 0f) <= aggrRange;
    }

    private bool IsPlayerInAttackRange() {
        return Vector3.Distance(transform.position, player.transform.position) + (player.IsDead ? 1000f : 0f) <= attackRange + attackBufferRange;
    }

    public void TakeDamage(float dmg, bool toLeft) {
        health = Mathf.Max(0f, health - dmg);
        if (health <= 0f) {
            isDead = true;
            anim.SetTrigger("isDead");
            anim.SetFloat("deathBlowAngle", toLeft ? 270f : 90f);
            posAtDeath = transform.position;
            agent.enabled = false;
            obstacle.enabled = false;
            Destroy(agent);
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(healthBarImage);
            Destroy(healthBarBackgroundImage);
            player.AddExperience(experience);
        }
    }

    public void Event_CreateEffect() {
        foreach (var c in anim.GetCurrentAnimatorClipInfo(0)) {
            if (c.clip.name == "Armature.001|Attack1Monster") {
                if (!player.IsDead) {
                    GameObject attackSphere = Instantiate(attackAreaSpherePrefab, posToAttack, Quaternion.identity);
                    attackSphere.GetComponent<AttackAreaSphere>().monster = this;
                }
            }
        }
    }

    public void Event_EndingAnim() {
        if (!IsPlayerInAttackRange()) {
            isAttacking = false;
            anim.SetBool("isAttacking", false);
        }
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        startedCounting = true;
        attackBufferRange = 0.2f;
    }
}
