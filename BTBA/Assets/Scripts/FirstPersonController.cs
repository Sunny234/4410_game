using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{	
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 8.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 16.0f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 4.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		[Tooltip("Crouching / Standing Heights")]
		public float StandingHeight = 2.0f;
		public float CrouchingHeight = 1.0f;
		[Tooltip("Current stamina drain rate")]
		public float staminaDrainRate = 20.0f;
		[Tooltip("Current stamina regen rate")]
		public float staminaRegenRate = 10.0f;
		[Tooltip("Current Max Stamina")]
		public float maxPlayerStamina = 100.0f;
		[Tooltip("Health variables")]
		public int maxHealth = 100;
		public int currentHealth = 100;
		[Tooltip("Armor variables")]
		public int maxArmor = 100;
		public int currentArmor = 100;
		[Tooltip("Restore variables")]
		public float staminaRestoreAmount = 50.0f;
		public int armorRestoreAmount = 50;
		public int healthRestoreAmount = 50;

		[Header("UI")]
		[Tooltip("Stamina Bar")]
		[SerializeField] StaminaScript staminaBar;
		[Tooltip("Stamina Bar Number")]
		public TextMeshProUGUI staminaText;
		[Tooltip("Health Bar")]
		public Slider healthBar;
		[Tooltip("Stamina Bar Number")]
		public TextMeshProUGUI healthText;
		[Tooltip("Armor Bar")]
		public Slider armorBar;
		[Tooltip("Stamina Bar Number")]
		public TextMeshProUGUI armorText;
		[Tooltip("Current ammo / clip size text")]
		public TextMeshProUGUI ammoText;
		[Tooltip("Max ammo text")]
		public TextMeshProUGUI maxAmmoText;
		[Tooltip("Reloading text")]
		public GameObject reloadingText;
		[Tooltip("Death Text")]
		public GameObject deathText;
		[Tooltip("Pause Menu Stuff")]
		public GameObject pauseMenuUI;
    	public bool isPaused;
		private bool canPause = true;

		// GOTO: "SetWeaponActive()" for corresponding weapon indices
		[Header("Weapon variables")]
		[Tooltip("Array of booleans for weapons player currently has")]
		bool[] currentWeapons = {false, false, false, false, false, false, false, false, false, false};
		[Tooltip("Array of weapon ammo clip sizes")]
		int[] weaponClipSizes = {30, 15, 10, 0, 0, 0, 0, 0, 0, 0};
		[Tooltip("Array of current ammo values")]
		int[] currentAmmoCount = {30, 15, 10, 0, 0, 0, 0, 0, 0, 0};
		[Tooltip("Array of default max weapon ammo used for when restoring max ammo")]
		int[] defaultMaxWeaponAmmo = {180, 90, 40, 0, 0, 0, 0, 0, 0, 0};
		[Tooltip("Array of max weapon ammo")]
		int[] maxWeaponAmmo = {180, 90, 40, 0, 0, 0, 0, 0, 0, 0};
		[Tooltip("Integer for what will take place of max ammo after reloading")]
		int newMaxAmmo;
		[Tooltip("Boolean for Gun script for if we can still shoot - AKA if we still have ammo")]
		public bool canShoot;
		[Tooltip("Boolean for allowing player to switch weapons")]
		bool canSwap = true;
		[Tooltip("Boolean for allowing player to reload")]
		bool canReload = true;
		[Tooltip("Boolean for telling 'SetAmmoText' function if we just reloaded")]
		bool reloaded = false;
		bool reloading = false;
		[Tooltip("Index of currently active weapon")]
		private int activeWeaponIndex;
		[Tooltip("Weapon Prefabs")]
		public GameObject _rifleAK;
		public GameObject _pistol;
		public GameObject _sniperRifle;

		[Header("Gravity variables")]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.5f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -9.81f;
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Helper Variables")]

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			SetStarterWeapons();
			SetStaminaText();
			Init_Health_Armor_Bars();
			reloadingText.SetActive(false);
			pauseMenuUI.SetActive(false);
			canShoot = false;
			deathText.SetActive(false);
			//pauseMenuUI.SetActive(false);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
			staminaBar.SetAllValues(maxPlayerStamina, maxPlayerStamina, staminaRegenRate, false);
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			CheckIfPaused();

			if (!(isPaused))
			{
				Crouch();
				Move();
				JumpAndGravity();
				SwapWeapons();
				ReloadWeapon();

				if (_input.sprint && _input.move != Vector2.zero)
				{
					staminaBar.UseStamina(staminaDrainRate);
				}
				else
				{
					staminaBar.RegenerateStamina();
				}

				GroundedCheck();
				SetAmmoText();
				SetHealthText();
				SetArmorText();
				SetStaminaText();
			}
			
		}

		private void LateUpdate()
		{
			HandleGamePause();
			if (!(isPaused))
				CameraRotation();
		}

		private void OnCollisionEnter(Collision col) {
			if (col.gameObject.CompareTag("EnemyBullet"))
				TakeDamage(30);
		}


		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			//float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			float targetSpeed;
			if (_input.sprint)
			{
				if (staminaBar.GetCurrentStamina() > 0) 
				{
					targetSpeed = SprintSpeed;
				}
				else
				{
					targetSpeed = MoveSpeed;
				}
			}
			else if (_input.crouch && Grounded)
			{
				targetSpeed = CrouchSpeed;
			}
			else
			{
				targetSpeed = MoveSpeed;
			}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		// Function for setting canShoot variable
		private void CheckForAmmo()
		{
			if (currentAmmoCount[activeWeaponIndex] <= 0 || reloading)
			{
				canShoot = false;
			}
			else
			{
				canShoot = true;
			}
		}

		// Function for updating / setting the ammo number value
		private void SetAmmoText()
		{
			CheckForAmmo();
			if (_input.shoot && canShoot && !(reloaded))
			{
				currentAmmoCount[activeWeaponIndex]--;
			}
			if (reloaded)
			{
				int clip_ammo_difference = weaponClipSizes[activeWeaponIndex] - currentAmmoCount[activeWeaponIndex];
				if (maxWeaponAmmo[activeWeaponIndex] - clip_ammo_difference < 0)
				{
					currentAmmoCount[activeWeaponIndex] += maxWeaponAmmo[activeWeaponIndex];
					maxWeaponAmmo[activeWeaponIndex] = 0;
				}
				else
				{
					currentAmmoCount[activeWeaponIndex] = weaponClipSizes[activeWeaponIndex];
					maxWeaponAmmo[activeWeaponIndex] -= clip_ammo_difference; 
				}
				reloaded = false;
			}
			ammoText.text = currentAmmoCount[activeWeaponIndex].ToString() + " / " + weaponClipSizes[activeWeaponIndex].ToString();
			maxAmmoText.text = maxWeaponAmmo[activeWeaponIndex].ToString();
		}

		// Function for updating / setting the stamina number value
		private void SetStaminaText()
		{
			double roundedNum = Math.Round(staminaBar.GetCurrentStamina(), 0);
			double maxNum = staminaBar.GetMaximumStamina();
			staminaText.text = roundedNum.ToString() + " / " + maxNum.ToString();
		}

		private void Init_Health_Armor_Bars()
		{
			armorBar.maxValue = maxArmor;
			armorBar.value = maxArmor;
			healthBar.maxValue = maxHealth;
			healthBar.value = maxHealth;
			SetArmorText();
			SetHealthText();
		}

		// Function for updating / setting the stamina number value
		private void SetArmorText()
		{
			armorText.text = currentArmor.ToString() + " / " + maxArmor.ToString();
			armorBar.value = currentArmor;
		}

		// Function for updating / setting the stamina number value
		private void SetHealthText()
		{
			healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
			healthBar.value = currentHealth;
		}

		// Function for setting the player's starting weapons
		private void SetStarterWeapons()
		{
		/*
			- Gives players rifle and pistol
			- Sets their respective indices as true to indicate player has this in "inventory"
				- Since 0 and 1 true, means player has access to rifle and pistol but nothing else
			- activeWeaponIndex indicates which weapon is currently on screen
			- Players can only have 2 weapons at a time
			- See 'SetWeaponActive' function for which indices are for which weapon
		*/
			SetWeaponActive(0, true);
			SetWeaponActive(1, false);
			SetWeaponActive(2, false);
			currentWeapons[0] = true;
			currentWeapons[1] = true;
			activeWeaponIndex = 0;
		}

		// Function for swapping weapons
		private void SwapWeapons()
		{
			if (_input.swapWeapon && canSwap)
			{
				int totalPossibleWeapons = 10;
				for (int newWeaponIndex = 0; newWeaponIndex < totalPossibleWeapons; newWeaponIndex++)
				{
					// If player has this weapon in 'inventory', and this weapon is NOT current weapon on screen
					if ( (currentWeapons[newWeaponIndex] == true) && (newWeaponIndex != activeWeaponIndex) )
					{
						SetWeaponActive(activeWeaponIndex, false);
						SetWeaponActive(newWeaponIndex, true);
						activeWeaponIndex = newWeaponIndex;
						break;
					}
				}
				StartCoroutine(haltSwap());
			}
		}

		// Function for forcing the player to wait 0.25 seconds before being able to swap again
		// Without it, player can hold down 'Tab' and swap indefinitely as long as it's held down
		IEnumerator haltSwap()
		{
			canSwap = false;
			yield return new WaitForSecondsRealtime(0.50f);
			canSwap = true;
		}

		// Function for forcing the player to wait 0.25 seconds before being able to pause again
		// Without it, player can hold down 'Tab' and pause indefinitely as long as it's held down
		IEnumerator haltPause()
		{
			canPause = false;
			yield return new WaitForSecondsRealtime(0.50f);
			canPause = true;
		}

		IEnumerator reloadEnumerator()
		{
			canReload = false;
			canSwap = false;
			reloading = true;
			reloadingText.SetActive(true);
			yield return new WaitForSecondsRealtime(2.0f);
			reloadingText.SetActive(false);
			canReload = true;
			canSwap = true;
			reloaded = true;
			reloading = false;
			SetAmmoText();
		}

		// Function for setting certain weapons active / inactive (what shows up on screen)
		private void SetWeaponActive(int arrIndex, bool active)
		{
			if (active)
			{
				// AK-74 Rifle
				if (arrIndex == 0) {
					_rifleAK.SetActive(true);
				}

				// M1911 Pistol
				if (arrIndex == 1) {
					_pistol.SetActive(true);
				}

				// M4 Handguard Rifle
				if (arrIndex == 2) {
					_sniperRifle.SetActive(true);
				}
			}
			else
			{
				// AK-74 Rifle
				if (arrIndex == 0) {
					_rifleAK.SetActive(false);
				}

				// M1911 Pistol
				if (arrIndex == 1) {
					_pistol.SetActive(false);
				}

				// M4 Handguard Rifle
				if (arrIndex == 2) {
					_sniperRifle.SetActive(false);
				}
			}
		}

		private bool DidRestoreAmmo(int groundWeaponIndex)
		{
			bool clipRestored = false;
			bool maxAmmoRestored = false;
			if (currentAmmoCount[groundWeaponIndex] < weaponClipSizes[groundWeaponIndex])
			{
				currentAmmoCount[groundWeaponIndex] = weaponClipSizes[groundWeaponIndex];
				SetAmmoText();
				clipRestored = true;
			}
			if (maxWeaponAmmo[groundWeaponIndex] < defaultMaxWeaponAmmo[groundWeaponIndex])
			{
				maxWeaponAmmo[groundWeaponIndex] = defaultMaxWeaponAmmo[groundWeaponIndex];
				SetAmmoText();
				maxAmmoRestored = true;
			}
			if (clipRestored || maxAmmoRestored)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void PickupWeapon(int _pickupWeaponIndex, Collider other) {
		/*
			How switching weapons work:
			- Check if player had current weapon on ground in 'inventory'
			- If player has weapon:
				- Restore player's appropriate weapon ammo
			- If not:
				- Set current weapon to inactive
				- Set current weapon index to false in 'inventory' list
					- means we no longer have access to this weapon when swapping
				- Set the pickup gameObject to false so it goes away
				- Set picked up weapon to true in 'inventory' list
				- Set picked up weapon to active
				- Set activeWeaponIndex to picked up weapon index
		*/
		
			// Checks if player's inventory contains weapon on ground already
			bool _hasWeaponOnGround = currentWeapons[_pickupWeaponIndex];
			
			if (_hasWeaponOnGround)
			{
				if (DidRestoreAmmo(_pickupWeaponIndex))
				{
					other.gameObject.SetActive(false);
				}
			}
			else
			{
				SetWeaponActive(activeWeaponIndex, false);
				currentWeapons[activeWeaponIndex] = false;
				other.gameObject.SetActive(false);
				
				if (_pickupWeaponIndex == 0) 
				{
					SetWeaponActive(0, true);
					currentWeapons[0] = true;
					activeWeaponIndex = 0;
				}
				if (_pickupWeaponIndex == 1)
				{
					SetWeaponActive(1, true);
					currentWeapons[1] = true;
					activeWeaponIndex = 1;
				}
				if (_pickupWeaponIndex == 2)
				{
					SetWeaponActive(2, true);
					currentWeapons[2] = true;
					activeWeaponIndex = 2;
				}
			}
		}

		// Function for handling trigger events (entering)
		private void OnTriggerEnter(Collider other) {
			if (other.gameObject.CompareTag("AKRiflePickup")) 
				PickupWeapon(0, other);
			
			if (other.gameObject.CompareTag("PistolPickup")) 
				PickupWeapon(1, other);
			
			if(other.gameObject.CompareTag("SniperPickup")) 
				PickupWeapon(2, other);
			

			if (other.gameObject.CompareTag("HealthPickup")) 
				RestoreHealth(other);
			

			if (other.gameObject.CompareTag("StaminaPickup")) 
				RestoreStamina(other);
			
				
			if (other.gameObject.CompareTag("ArmorPickup")) 
				RestoreArmor(other);
			
		}

		private void RestoreHealth(Collider pickup)
		{
			if (currentHealth < maxHealth) {
				currentHealth += healthRestoreAmount;
				currentHealth = Math.Min(currentHealth, maxHealth); // Makes sure you never go over the maximum amount
				pickup.gameObject.SetActive(false);
			}
		}

		private void RestoreStamina(Collider pickup)
		{
			if (staminaBar.GetCurrentStamina() < maxPlayerStamina) {
				float updatedStamina = staminaBar.GetCurrentStamina();
				updatedStamina += staminaRestoreAmount;
				updatedStamina = Math.Min(updatedStamina, maxPlayerStamina); // Makes sure you never go over the maximum amount
				staminaBar.SetCurrentStamina(updatedStamina);
				pickup.gameObject.SetActive(false);
			}
		}

		private void RestoreArmor(Collider pickup)
		{
			if (currentArmor < maxArmor) {
				currentArmor += armorRestoreAmount;
				currentArmor = Math.Min(currentArmor, maxArmor); // Makes sure you never go over the maximum amount
				pickup.gameObject.SetActive(false);
			}
		}

		// Function for allowing player to crouch
		private void Crouch()
		{
			if (_input.crouch && Grounded)
			{
				_controller.height = CrouchingHeight;
			}
			else
			{
				_controller.height = StandingHeight;
			}
		}

		// Function for reloading current weapon
		private void ReloadWeapon()
		{
			int ammo = currentAmmoCount[activeWeaponIndex];
			int clip = weaponClipSizes[activeWeaponIndex];
			if (_input.reload && canReload && ammo < clip && maxWeaponAmmo[activeWeaponIndex] > 0)
			{
				StartCoroutine(reloadEnumerator());
			}
		}

		// Function for taking damage
		private void TakeDamage(int damageAmount)
		{
			/*
				How this works overall:
					- "DamageArmor" will return how much armor is remaining after our damage
					- If this number is negative, that means we lost more armor than we had
					- We have to add this negative number (subtract) to our health in order to account for it
			*/
			int damageOverflow = DamageArmor(damageAmount);
			if (damageOverflow < 0)
			{
				currentHealth += damageOverflow;
				CheckPlayerDeath();
				currentHealth = Math.Max(currentHealth, 0);	// Never let current health be negative numbers
			}
		}

		// Function for damaging armor
		private int DamageArmor(int damageAmount)
		{
			int damageOverflow;
			currentArmor -= damageAmount;
			damageOverflow = currentArmor;
			currentArmor = Math.Max(currentArmor, 0); // Never let current health be negative numbers
			return damageOverflow;
		}

		// Function for checking if player has died
		private void CheckPlayerDeath()
		{
			if (currentHealth <= 0)
				deathText.SetActive(true);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}

		// --------------------------------------------------
		// PAUSE MENU STUFF - Couldn't figure our how to use StarterAssetsInputs in other script

		// Need this check because player can unpause the game through pressing "Resume" when paused
		private void CheckIfPaused()
		{
			if(pauseMenuUI.activeInHierarchy)
				isPaused = true;
			else
				isPaused = false;
		}

		private void HandleGamePause()
		{
			if (_input.pauseGame && canPause)
			{
				if (isPaused) {
					StartCoroutine(haltPause());
					_input.SetCursorState(true); // Lock cursor so we can play game
					ResumeGame();
				}

				else {
					StartCoroutine(haltPause());
					_input.SetCursorState(false); // Unlock cursor so we can click menu
					PauseGame();
				}
			}
		}

		public void ResumeGame() 
		{
			pauseMenuUI.SetActive(false);
			Time.timeScale = 1f;
			isPaused = false;
		}
		
		public void PauseGame() 
		{
			pauseMenuUI.SetActive(true);
			Time.timeScale = 0f;
			isPaused = true;
		}

		public void MainMenu()
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene("MainMenu");
		}

		public void Exit()
		{
			Application.Quit();
		}

	}
}