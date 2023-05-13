using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;
		public bool shoot;
		public bool swapWeapon;
		public bool reload;
		public bool pauseGame;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		
		// Function we added to shoot
		public void OnShoot(InputValue value)
		{
			shoot = value.isPressed;
		}

		// Function we added to crouch
		public void OnCrouch(InputValue value)
		{
			crouch = value.isPressed;
		}

		// Function we added to swap weapons
		public void OnSwapWeapon(InputValue value)
		{
			swapWeapon = value.isPressed;
		}

		// Function we added to reload current weapon
		public void OnReload(InputValue value)
		{
			reload = value.isPressed;
		}

		// Function we added to pause the game
		public void OnPauseGame(InputValue value)
		{
			pauseGame = value.isPressed;
		}
#endif

		public void SwapWeaponInput(bool newSwapState)
		{
			swapWeapon = newSwapState;
		}

		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;	
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void PauseGameInput(bool newPauseState)
		{
			pauseGame = newPauseState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		public void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}