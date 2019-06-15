using UnityEngine;

// Standard AI enemy ship
public class ESFlanker : EnemyShip
{
    // Flanker GameObjects
    private GameObject FlankTeleportObject;

    // Ship Stats
    private float FlankTeleportRange;

    // Enemy ship standard constructor
    public ESFlanker(uint _id, Vector3 _startingPosition)
    {
        this.ID = _id;
        this.StartingPosition = _startingPosition;
        this.Type = EnemyShipType.Flanker;
        this.AItype = AIType.Flanker;
        this.IFF = GameController.IFF.Enemy;
        this.IsPlayer = false;
        // Ship stats
        // --Health/Armor/Shields
        this.Health = 50f;
        this.MaxHealth = 50f;
        this.Armor = 75f;
        this.Shields = 25f;
        this.MaxShields = 25f;
        this.ShieldRegenSpeed = 1f;
        this.ShieldCooldownTime = 3f;
        // --Current/Max energy
        this.Energy = 100f;
        this.MaxEnergy = 100f;
        this.EnergyRegenSpeed = 1.5f;
        // --Energy costs
        this.GunEnergyCost = 17f;
        // --Acceleration
        this.EngineCount = 1;
        this.ImpulseAcceleration = 60f;
        this.StrafeAcceleration = 20f;
        // --Max Speed
        this.MaxImpulseSpeed = 50f;
        this.MaxStrafeSpeed = 20f;
        this.MaxRotationSpeed = 0.1f;
        // --Weapon stats
        // ----Main gun
        this.GunBarrelCount = 2;
        this.GunShotProjectileType = 2;
        this.GunCooldownTime = 1f;
        this.GunShotAmount = 1;
        this.GunShotCurvature = 0f;
        this.GunShotSightCone = 0f;
        this.GunShotDamage = 2f;
        this.GunShotAccuracy = 95f;
        this.GunShotSpeed = 120f;
        this.GunShotLifetime = 2f;
        // Abilities
        this.FlankTeleportRange = 20f;
        // Ability Cooldowns
        this.AbilityDuration[0] = 1f;
        this.AbilityCooldownTime[0] = 5f;
        // AI fields
        this.AIAimAssist = true;
        this.MaxTargetAcquisitionRange = 80f;
        this.MaxOrbitRange = 40f;
        this.MaxWeaponsRange = 50f;
        this.MaxAbilityRange = 40f;
        // Experience
        this.XP = (uint)(this.MaxHealth + this.MaxShields);
        // GameObject Instantiation
        this.ShipObjectPrefab = Resources.Load<GameObject>(GameController.EnemyShipPrefabName + $@" {this.Type}");
        this.ShipObject = GameObject.Instantiate(this.ShipObjectPrefab, this.StartingPosition, Quaternion.Euler(0, GameController.r.Next(0, 360), 0));
        this.FlankTeleportObject = this.ShipObject.transform.Find(GameController.EMPObjectName).gameObject;
        this.Initialize();
    }


    public override void CheckAbility1()
    {
        if(this.IsEMPed == false)
        {
            Abilities.CheckFlankTeleport(this, 0, this.FlankTeleportObject, this.FlankTeleportRange);
        }
    }
}
