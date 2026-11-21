using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Đạn của người chơi: bay thẳng lên, trúng địch thì gây sát thương và biến mất.</summary>
    public class Bullet : MonoBehaviour
    {
        const float Speed = 15f;
        SpaceShooterGame game;
        bool used;

        public static void Create(Transform parent, Vector2 pos, SpaceShooterGame game)
        {
            var go = Shape.Root("Bullet", parent, pos);
            var laser = Art.Get("space/laser_player");
            if (laser != null)
            {
                Art.Add(go.transform, laser, Vector2.zero, 0.5f, 5, 0f, "Laser");
            }
            else
            {
                Shape.Add(go.transform, SpriteFactory.Square, new Color(1f, 0.95f, 0.4f), Vector2.zero, new Vector2(0.1f, 0.42f), 5);
                Shape.Add(go.transform, SpriteFactory.Circle, new Color(1f, 0.9f, 0.3f, 0.35f), new Vector2(0f, 0.12f), new Vector2(0.28f, 0.4f), 4);
            }

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.16f, 0.45f);
            col.isTrigger = true;

            var rb = Phys.Body(go);
            rb.SetVel(Vector2.up * Speed);

            go.AddComponent<Bullet>().game = game;
        }

        void Update()
        {
            if (transform.position.y > game.HalfH + 1f) Destroy(gameObject); // bay ra khỏi màn hình
        }

        // ===== TƯƠNG TÁC: đạn chạm địch =====
        void OnTriggerEnter2D(Collider2D other)
        {
            if (used) return;
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            used = true;
            enemy.TakeDamage(1);
            Fx.Burst(transform.position, new Color(1f, 0.9f, 0.4f), 5, 2f, game.World, 0.25f, 0.1f, 0f, false);
            Destroy(gameObject);
        }
    }
}
