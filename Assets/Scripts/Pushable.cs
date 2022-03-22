using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float pushSpeed = 1.0f;
    public float fallSpeed = 0.1f;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = this.gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.Controller?.CheckForInteraction(transform.position) ?? false)
		{
            Vector3 velocity = (transform.position - GameController.Controller.Player.transform.position).normalized * pushSpeed * Time.deltaTime;
            controller.Move(velocity);
        }

        controller.Move(Vector3.down * fallSpeed);
    }
}
