using UnityEngine;
using Infra;
using Infra.Gameplay;
using Infra.Utils;

namespace Gadget {

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(OverlapChecker))]
/// <summary>
/// The player controller class.
/// </summary>
public class Player : MonoBehaviour {

		// Character parameteres
		public int health = 1;
		[Range(0f,40f)]
		[SerializeField] float jumpHeight = 15f;
		[SerializeField] float xSpeed = 7f;
		[SerializeField] float gravity = 2f;
		[Range(0f,360f)]
		[SerializeField] float shootAngle = 45f;
		[SerializeField] float bulletSpeed = 15f;

		// Character Controls
		KeyCode KEYRIGHT = KeyCode.D;
		KeyCode KEYLEFT = KeyCode.A;
		KeyCode KEYUP  = KeyCode.W;
		KeyCode KEYSHOOT = KeyCode.Space;

		[SerializeField] Transform arm; //set to BoneArm
		[SerializeField] Transform gun;
		[SerializeField] Rigidbody2D currBullet;

		private readonly int animatorIsAlive = Animator.StringToHash("Alive");
		private readonly int animatorJumpTrigger = Animator.StringToHash("Jump");

		private Animator animator;
			private Rigidbody2D playerBody;
			private OverlapChecker floorOverlapChecker;

	    private bool isOnFloor {
	        get {
				return floorOverlapChecker.isOverlapping;
	        }
	    }
				
	    protected void Awake() {
	        animator = GetComponent<Animator>();
			playerBody = GetComponent<Rigidbody2D>();
	        floorOverlapChecker = GetComponent<OverlapChecker>();
	        animator.SetBool(animatorIsAlive, true);
	    }
				
	    protected void Update() {

			// Set arm angle for shooting
			var armAngle = arm.eulerAngles;
	        armAngle.z = shootAngle;
	        arm.eulerAngles = armAngle;

	        playerBody.gravityScale = gravity;

			var currSpeed = playerBody.velocity;

			// Jump, move, shoot
	        if (Input.GetKeyDown(KEYUP) && isOnFloor) {
	            currSpeed.y = jumpHeight;
	            playerBody.velocity = currSpeed;
	            animator.SetTrigger(animatorJumpTrigger);
	        } else if (Input.GetKey(KEYRIGHT)) {
	            currSpeed.x = xSpeed;
	            playerBody.velocity = currSpeed;
	        } else if (Input.GetKey(KEYLEFT)) {
	            currSpeed.x = -xSpeed;
	            playerBody.velocity = currSpeed;
			} else if (Input.GetKey(KEYSHOOT)) {
	            if (!currBullet.gameObject.activeInHierarchy) {
	                currBullet.gameObject.SetActive(true);
	                currBullet.position = gun.position;
	                currBullet.velocity = Vector2.right.Rotate(Mathf.Deg2Rad * shootAngle) * bulletSpeed;
	            }
	        }
	    }
				
	    protected void OnCollisionEnter2D(Collision2D collision) {
	        if (health <= 0) return;

			// Reached yellow square
	        if (collision.gameObject.CompareTag("Victory")) {
	            DebugUtils.Log("Great Job!");
	            return;
	        }
				
	        if (!collision.gameObject.CompareTag("Enemy")) return;

			// Hit by enemy. 
	        --health;
	        if (health > 0) return;

			// Dead
	        animator.SetBool(animatorIsAlive, false);
	        playerBody.velocity = Vector2.zero;
	        playerBody.gravityScale = 4f;
	        enabled = false;
	    }
	}
}
