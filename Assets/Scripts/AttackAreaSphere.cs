using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType
{
    MELEE,
    RANGED
}

public class AttackAreaSphere : MonoBehaviour
{
    public PlayerController player;
    public GameObject baseUI;
    public Monster monster;
    private int monsterHashCode;
    private List<GameObject> objects = new List<GameObject>();
    public AttackType type = AttackType.MELEE;
    private Vector3 rangedOffset;
    private float timeToLive;
    private float rangedSpeed;
    private MeshRenderer meshRenderer;
    private Material material;
    private Color rangedColor = new Color(1f, 1f, 0f, 1f);
    private bool consumed = false;

    public int MonsterHashCode {
        get { return monsterHashCode; }
        set { monsterHashCode = value; }
    }

    private void Awake() {
        baseUI = GameObject.Find("BaseUI");
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void Start() {
        rangedOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        if (type == AttackType.MELEE) {
            material.color = new Color(0.3f, 0.3f, 0f, 1f);
        }
        if (type == AttackType.RANGED) {
            if (player) {
                timeToLive = player.EffectTimeToLive;
                rangedSpeed = player.EffectRangedSpeed;
            }
        }
    }

    private void Update() {
        if (type == AttackType.RANGED) {
            transform.Translate((transform.forward + rangedOffset) * rangedSpeed * Time.deltaTime, Space.World);
        }
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0 || transform.position.y > 2f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {

        objects.Add(other.gameObject);

        if (monster) {
            if (other.gameObject.CompareTag("Monster")) {
                return;
            }
            if (other.gameObject.CompareTag("Player")) {
                float damageDealt = monster.Damage;
                float crit = (Random.Range(1, 100) <= 10) ? 2 : 1;
                other.GetComponent<PlayerController>().TakeDamage(damageDealt * crit);
                baseUI.GetComponent<UIController>().CreateText(
                    0,
                    damageDealt,
                    Camera.main.WorldToScreenPoint(other.transform.position),
                    crit
                    );
                enabled = false;
            }
        }

        if (player && !consumed) {
            if (other.gameObject.CompareTag("Player")) {
                return;
            }
            if (other.gameObject.CompareTag("Monster")) {
                if (type == AttackType.MELEE && other.gameObject.GetHashCode() == monsterHashCode) {
                    float damageDealt = player.Damage;
                    bool isCrit = Mathf.FloorToInt(player.CritChance * 100) >= Random.Range(1, 10000);
                    float crit = isCrit ? (1f + player.CritDamage / 100f) : 1f;
                    float tmpPlRotY = (player.transform.rotation.eulerAngles.y + 360f) % 360f;
                    float tmpMRotY = (other.gameObject.transform.rotation.eulerAngles.y + 360f) % 360f;
                    float tmpAngle = Mathf.Abs(tmpPlRotY - tmpMRotY);
                    player.AddLifeOnHitAndLifeSteal(damageDealt * crit);
                    other.GetComponent<Monster>().TakeDamage(
                        damageDealt * crit, 
                        (tmpAngle >= 90 && tmpAngle < 270) ? (player.IsRight ? true : false) : (player.IsRight ? false : true)
                        );
                    baseUI.GetComponent<UIController>().CreateText(
                        1,
                        damageDealt,
                        Camera.main.WorldToScreenPoint(other.transform.position),
                        crit
                        );
                    enabled = false;
                }
                if (type == AttackType.RANGED) {
                    float damageDealt = player.MagicDamage;
                    bool isCrit = Mathf.FloorToInt(player.CritChance * 100) >= Random.Range(1, 10000);
                    float crit = isCrit ? (1f + player.CritDamage / 100f) : 1f;
                    float tmpDotRight = Vector3.Dot(player.transform.forward, other.gameObject.transform.right);
                    other.GetComponent<Monster>().TakeDamage(
                        damageDealt * crit,
                        (tmpDotRight > 0) ? true : false
                        );
                    baseUI.GetComponent<UIController>().CreateText(
                        1,
                        damageDealt,
                        Camera.main.WorldToScreenPoint(other.transform.position),
                        crit
                        );

                    consumed = true;
                }
            }
        }

        if (type == AttackType.MELEE) Destroy(gameObject);
    }
}
