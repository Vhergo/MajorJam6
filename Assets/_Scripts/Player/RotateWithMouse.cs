using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool stopRotation;
    [SerializeField] private float equipOffset;
    private Vector3 objectPosition;
    private bool isMovingRight;
    private Vector2 objectDirection;
    private Transform player;

    // Objects To Flip
    private SpriteRenderer playerSprite;
    private SpriteRenderer weaponSprite;
    [SerializeField] private SpriteRenderer muzzleFlashSprite;

    void Start() {
        player = GameObject.Find("Player").transform;
        playerSprite = GameObject.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        weaponSprite = GameObject.Find("WeaponSprite").GetComponent<SpriteRenderer>();
    }

    void Update() {
        SwitchHands();
        FollowPlayer();
        RotateObject();
    }

    void RotateObject() {
        objectDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(objectDirection.y, objectDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void SwitchHands() {
        if (MySceneManager.Instance.gameState == GameState.Pause) return;

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= player.position.x) {
            objectPosition = GetEquipPosition(equipOffset);
            weaponSprite.flipY = false;
            playerSprite.flipX = false;
            muzzleFlashSprite.flipY = false;
        }else {
            objectPosition = GetEquipPosition(-equipOffset);
            weaponSprite.flipY = true;
            playerSprite.flipX = true;
            muzzleFlashSprite.flipY = true;
        }
    }

    void FollowPlayer() {
        transform.position = objectPosition;
    }

    Vector3 GetEquipPosition(float offset = 0) {
        return new Vector3 (player.position.x + offset, player.position.y, 0f);
    }
}
