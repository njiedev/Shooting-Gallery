using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject fpsController;
    public GameObject terrain;

    SpawnPoint spawnPoint;
    void Start()
    {
        spawnPoint = new SpawnPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 position = spawnPoint.Generate(other.gameObject, terrain);
            MoveTo(other.gameObject, position);
        }
    }

    void MoveTo(GameObject player, Vector3 position)
    {
        // The character controller has it's own internal definition of what position it has,
        // and will set the transform's position to that every frame, so you can't move it
        // with transform.position.
        CharacterController controllerScript = player.GetComponent<CharacterController>();
        controllerScript.enabled = false;
        player.transform.position = position;
        controllerScript.enabled = true;
    }
}
