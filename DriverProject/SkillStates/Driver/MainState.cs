using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.Modules;
using RobDriver.SkillStates.Emote;
using BepInEx.Configuration;

namespace RobDriver.SkillStates.Driver
{
    public class MainState : GenericCharacterMain
    {
		public LocalUser localUser;
        private int layerIndex = -1;

        public override void OnEnter()
        {
            base.OnEnter();
			this.FindLocalUser();

            if (hasCharacterBody && hasModelAnimator)
            {
                layerIndex = base.modelAnimator.GetLayerIndex("Body");
                characterBody.onJump += OnJump;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (hasCharacterBody && hasModelAnimator)
                characterBody.onJump -= OnJump;
        }

        private void OnJump()
        {
            if (layerIndex >= 0)
            {
                if (this.characterBody.isSprinting)
                {
                    this.modelAnimator.CrossFadeInFixedTime("SprintJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                }
                // jumpCount has already been added to, used to be >=
                else if (base.characterMotor.jumpCount > base.characterBody.baseJumpCount)
                {
                    this.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                }
            }

            // fuck you fuck you fuck you
            // dont name the y variable x and the x variable y
            float right = this.animatorWalkParamCalculator.animatorWalkSpeed.y;
            float forward = this.animatorWalkParamCalculator.animatorWalkSpeed.x;

            // neutral jump
            if (Mathf.Abs(right) <= 0.45f && Mathf.Abs(forward) <= 0.45f || this.inputBank.moveVector == Vector3.zero)
            {
                right = 0f;
                forward = 0f;
            }

            if (Mathf.Abs(right) > Mathf.Abs(forward))
            {
                // side flip
                right = Mathf.Sign(right);
                forward = 0f;
            }
            else if (Mathf.Abs(right) < Mathf.Abs(forward))
            {
                // forward/backflips
                forward = Mathf.Sign(forward);
                right = 0f;
            }
            // eh this feels less dynamic. ignore the slight anim clipping issues ig and just blend them
            //  actualyl don't because the clipping issues are nightmarish

            // have to cache it at time of jump otherwise you can fuck up the jump anim in weird ways by turning during it
            this.modelAnimator.SetFloat("forwardSpeedCached", forward);
            this.modelAnimator.SetFloat("rightSpeedCached", right);
            // turns out this wasn't even used in the end. the animation didn't break at all in practice, only in theory
            // Fuck You rob you fucking moron

            //  update: this was actually used. what the hell are you doing?
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.modelAnimator)
            {
                bool cock = false;
                if (!this.characterBody.outOfDanger || !this.characterBody.outOfCombat) cock = true;
                this.modelAnimator.SetBool("inCombat", cock);

				if (this.isGrounded) this.modelAnimator.SetFloat("airBlend", 0f);
				else this.modelAnimator.SetFloat("airBlend", 1f);
            }

			//emotes
			if (base.isAuthority && base.characterMotor.isGrounded)
			{
				this.CheckEmote<Rest>(Config.restKey);
				this.CheckEmote<Taunt>(Config.tauntKey);
				this.CheckEmote<Dance>(Config.danceKey);
			}
		}

		private void CheckEmote(KeyCode keybind, EntityState state)
		{
			if (Input.GetKeyDown(keybind))
			{
				if (!localUser.isUIFocused)
				{
					outer.SetInterruptState(state, InterruptPriority.Any);
				}
			}
		}

		private void CheckEmote<T>(ConfigEntry<KeyboardShortcut> keybind) where T : EntityState, new()
		{
			if (Config.GetKeyPressed(keybind))
			{
				FindLocalUser();

				if (localUser != null && !localUser.isUIFocused)
				{
					outer.SetInterruptState(new T(), InterruptPriority.Any);
				}
			}
		}

		private void FindLocalUser()
		{
			if (this.localUser == null)
			{
				if (base.characterBody)
				{
					foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
					{
						if (lu.cachedBody == base.characterBody)
						{
							this.localUser = lu;
							break;
						}
					}
				}
			}
        }

        public override void ProcessJump()
        {
            if (!hasCharacterMotor)
            {
                return;
            }

            bool hopooFeather = false;
            bool waxQuail = false;
            bool canJump = base.characterMotor.jumpCount < base.characterBody.maxJumpCount;

            if (!(jumpInputReceived && (bool)base.characterBody && canJump))
            {
                return;
            }

            int itemCountEffective = base.characterBody.inventory.GetItemCountEffective(RoR2Content.Items.JumpBoost);
            float horizontalBonus = 1f;
            float verticalBonus = 1f;
            if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
            {
                hopooFeather = true;
                horizontalBonus = 1.5f;
                verticalBonus = 1.5f;
            }
            else if ((float)itemCountEffective > 0f && base.characterBody.isSprinting)
            {
                float num = base.characterBody.acceleration * base.characterMotor.airControl;
                if (base.characterBody.moveSpeed > 0f && num > 0f)
                {
                    waxQuail = true;
                    float num2 = Mathf.Sqrt(10f * (float)itemCountEffective / num);
                    float num3 = base.characterBody.moveSpeed / num;
                    horizontalBonus = (num2 + num3) / num3;
                }
            }

            ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus);

            if (sfxLocator && !string.IsNullOrEmpty(base.sfxLocator.jumpSound))
            {
                Util.PlaySound(base.sfxLocator.jumpSound, outer.gameObject);
            }

            if (hopooFeather)
            {
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                {
                    origin = base.characterBody.footPosition
                }, transmit: true);
            }
            else if (base.characterMotor.jumpCount > 0)
            {
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
                {
                    origin = base.characterBody.footPosition,
                    scale = base.characterBody.radius
                }, transmit: true);
            }

            if (waxQuail)
            {
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
                {
                    origin = base.characterBody.footPosition,
                    rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
                }, transmit: true);
            }

            base.characterMotor.jumpCount++;
            base.characterBody.TriggerJumpEventGlobally();
		}
    }
}