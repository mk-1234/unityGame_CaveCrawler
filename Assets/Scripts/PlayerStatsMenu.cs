using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsMenu : MonoBehaviour
{
    private UIController uiController;

    private void Awake() {
        uiController = transform.parent.GetComponentInParent<UIController>();
    }

    public void SetAttackToLeft() {
        uiController.player.SetAttackToLeft();
    }

    public void SetAttackToRight() {
        uiController.player.SetAttackToRight();
    }

    public void SetAttackToAlternating() {
        uiController.player.SetAttackToAlternating();
    }

    public void UnequipWeaponLeftHand() {
        uiController.player.UnequipWeaponLeftHand();
    }

    public void EquipAxeLeftHand() {
        uiController.player.EquipAxeLeftHand();
    }

    public void EquipSwordLeftHand() {
        uiController.player.EquipSwordLeftHand();
    }

    public void UnequipWeaponRightHand() {
        uiController.player.UnequipWeaponRightHand();
    }

    public void EquipAxeRightHand() {
        uiController.player.EquipAxeRightHand();
    }

    public void EquipSwordRightHand() {
        uiController.player.EquipSwordRightHand();
    }

    public void IncreaseHandDamage() {
        uiController.player.IncreaseHandDamage();
        uiController.UpdateStats();
    }

    public void IncreaseAxeDamage() {
        uiController.player.IncreaseAxeDamage();
        uiController.UpdateStats();
    }

    public void IncreaseSwordDamage() {
        uiController.player.IncreaseSwordDamage();
        uiController.UpdateStats();
    }

    public void IncreaseMagicDamage() {
        uiController.player.IncreaseMagicDamage();
        uiController.UpdateStats();
    }

    public void IncreaseMagicDuration() {
        uiController.player.IncreaseMagicDuration();
        uiController.UpdateStats();
    }

    public void IncreaseMagicFreq() {
        uiController.player.IncreaseMagicFreq();
        uiController.UpdateStats();
    }

    public void IncreaseMagicSpeed() {
        uiController.player.IncreaseMagicSpeed();
        uiController.UpdateStats();
    }

    public void IncreaseAttackSpeed() {
        uiController.player.IncreaseAttackSpeed();
        uiController.UpdateStats();
    }

    public void IncreaseMoveSpeed() {
        uiController.player.IncreaseMoveSpeed();
        uiController.UpdateStats();
    }

    public void IncreaseMaxHealth() {
        uiController.player.IncreaseMaxHealth();
        uiController.UpdateStats();
    }

    public void IncreaseHealthRegen() {
        uiController.player.IncreaseHealthRegen();
        uiController.UpdateStats();
    }

    public void IncreaseLifeOnHit() {
        uiController.player.IncreaseLifeOnHit();
        uiController.UpdateStats();
    }

    public void IncreaseLifeSteal() {
        uiController.player.IncreaseLifeSteal();
        uiController.UpdateStats();
    }

    public void IncreaseCritChance() {
        uiController.player.IncreaseCritChance();
        uiController.UpdateStats();
    }

    public void IncreaseCritDamage() {
        uiController.player.IncreaseCritDamage();
        uiController.UpdateStats();
    }

    public void IncreaseMaxMana() {
        uiController.player.IncreaseMaxMana();
        uiController.UpdateStats();
    }

    public void IncreaseManaRegen() {
        uiController.player.IncreaseManaRegen();
        uiController.UpdateStats();
    }
}
