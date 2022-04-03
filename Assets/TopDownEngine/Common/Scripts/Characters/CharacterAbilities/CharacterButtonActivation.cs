using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to activate button zones
    /// Animator parameters : Activating (bool)
    /// </summary>
    [MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Button Activation")] 
	public class CharacterButtonActivation : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to interact with button powered objects (dialogue zones, switches...). "; }
        /// true if the character is in a dialogue zone
        public bool InButtonActivatedZone {get;set;}
        /// true if the zone is automated	
        public bool InButtonAutoActivatedZone { get; set; }
        /// the current button activated zone
        [MMReadOnly]
        [Tooltip("the current button activated zone this character is in")]
        public ButtonActivated ButtonActivatedZone;

		protected bool _activating = false;
        protected const string _activatingAnimationParameterName = "Activating";
        protected int _activatingAnimationParameter;
        
		/// <summary>
		/// Gets and stores components for further use
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			InButtonActivatedZone = false;
			ButtonActivatedZone = null;
            InButtonAutoActivatedZone = false;

        }

		/// <summary>
		/// Every frame, we check the input to see if we need to pause/unpause the game
		/// </summary>
		protected override void HandleInput()
		{
			if (!AbilityAuthorized)
			{
				return;
			}
            if (InButtonActivatedZone && (ButtonActivatedZone != null))
            {
                bool buttonPressed = false;
                switch (ButtonActivatedZone.InputType)
                {
                    case ButtonActivated.InputTypes.Default:
                        buttonPressed = (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
                        break;
                    case ButtonActivated.InputTypes.Button:
                        buttonPressed = (Input.GetButtonDown(_character.PlayerID + "_" + ButtonActivatedZone.InputButton));
                        break;
                    case ButtonActivated.InputTypes.Key:
                        buttonPressed = (Input.GetKeyDown(ButtonActivatedZone.InputKey));
                        break;
                }

                if (buttonPressed)
                {
                    ButtonActivation();
                }
            }
		}
        
		/// <summary>
		/// Tries to activate the button activated zone
		/// </summary>
		protected virtual void ButtonActivation()
		{
			// if the player is in a button activated zone, we handle it
			if ((InButtonActivatedZone)
			    && (ButtonActivatedZone!=null)
				&& (_condition.CurrentState == CharacterStates.CharacterConditions.Normal || _condition.CurrentState == CharacterStates.CharacterConditions.Frozen)
			    && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				// if the button can only be activated while grounded and if we're not grounded, we do nothing and exit
				if (ButtonActivatedZone.CanOnlyActivateIfGrounded && !_controller.Grounded)
				{
					return;
				}

                // if it's an auto activated zone, we do nothing	
                if (ButtonActivatedZone.AutoActivation)
                {
                    return;
                }

                // we trigger a character event
                MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.ButtonActivation);

				ButtonActivatedZone.TriggerButtonAction();
                PlayAbilityStartFeedbacks();
                _activating = true;
			}
		}

        /// <summary>	
        /// On Death we lose any connection we may have had to a button activated zone	
        /// </summary>	
        protected override void OnDeath()
        {
            base.OnDeath();
            InButtonActivatedZone = false;
            ButtonActivatedZone = null;
            InButtonAutoActivatedZone = false;
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_activatingAnimationParameterName, AnimatorControllerParameterType.Bool, out _activatingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _activatingAnimationParameter, _activating, _character._animatorParameters, _character.RunAnimatorSanityChecks);
            if (_activating && (ButtonActivatedZone != null) && (ButtonActivatedZone.AnimationTriggerParameterName != ""))
            {
                _animator.SetTrigger(ButtonActivatedZone.AnimationTriggerParameterName);
            }
            _activating = false;
        }
	}
}
