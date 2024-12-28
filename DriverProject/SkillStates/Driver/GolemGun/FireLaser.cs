using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.AddressableAssets;
using R2API;

namespace RobDriver.SkillStates.Driver.GolemGun
{
	public class FireLaser : BaseDriverSkillState
	{
		public static float damageCoefficient = 14f;
		public static float blastRadius = 5f;
		public static float force = 2000f;
		public static float recoil = 16f;

		public static float baseDuration = 0.7f;
		public Vector3 laserDirection;

		private float duration;
		private Ray modifiedAimRay;

		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireLaser.baseDuration / this.attackSpeedStat;
			this.modifiedAimRay = base.GetAimRay();
			this.modifiedAimRay.direction = this.laserDirection;
			Transform modelTransform = this.GetModelTransform();

			Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, this.gameObject);

			string text = "ShotgunMuzzle";
			this.characterBody.SetAimTimer(2f);

			base.PlayAnimation("Gesture, Override", "FireTwohand", "Shoot.playbackRate", this.duration);
			base.PlayAnimation("AimPitch", "Shoot");

			if (EntityStates.GolemMonster.FireLaser.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(EntityStates.GolemMonster.FireLaser.effectPrefab, base.gameObject, text, false);
			}

			if (base.isAuthority)
			{
				float num = 1000f;
				Vector3 vector = this.modifiedAimRay.origin + this.modifiedAimRay.direction * num;

				RaycastHit raycastHit;
				if (Physics.Raycast(this.modifiedAimRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
				{
					vector = raycastHit.point;
				}

				BlastAttack blastAttack = new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * FireLaser.damageCoefficient,
					damageType = iDrive.DamageType,
					baseForce = FireLaser.force * 0.2f,
					position = vector,
					radius = FireLaser.blastRadius,
					falloffModel = BlastAttack.FalloffModel.None,
					bonusForce = FireLaser.force * this.modifiedAimRay.direction
				};
				blastAttack.AddModdedDamageType(iDrive.ModdedDamageType);
				blastAttack.Fire();

				Vector3 origin = this.modifiedAimRay.origin;
				if (modelTransform)
				{
					ChildLocator component = modelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						int childIndex = component.FindChildIndex(text);
						if (EntityStates.GolemMonster.FireLaser.tracerEffectPrefab)
						{
							EffectData effectData = new EffectData
							{
								origin = vector,
								start = this.modifiedAimRay.origin
							};
							effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
							EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
							EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
						}
					}
				}
			}

			if (this.iDrive) this.iDrive.ConsumeAmmo(2f);

			float recoilAmplitude = FireLaser.recoil / this.attackSpeedStat;

			base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
			this.characterBody.AddSpreadBloom(4f);
		}

		public override void OnExit()
		{
			base.OnExit();

			this.GetModelAnimator().SetTrigger("endAim");
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (base.fixedAge >= (0.5f * this.duration) && this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
			{
				base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
				this.outer.SetNextStateToMain();
				return;
			}

			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}