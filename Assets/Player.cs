using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Manager.Instance.Score = (int)transform.position.x;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("KillZone"))
        {
            Manager.Instance.Kill();
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            Manager.Instance.NextCheckpoint(collision.gameObject);
        }
        else if (collision.CompareTag("Coin"))
        {
            Manager.Instance.AddCoin(collision.gameObject);
        }
    }
}