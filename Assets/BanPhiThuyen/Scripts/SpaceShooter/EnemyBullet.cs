using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Đạn của tàu địch. Va chạm với phi thuyền được xử lý trong PlayerShip.</summary>
    public class EnemyBullet : MonoBehaviour
    {
        const float Speed = 5.5f;
        SpaceShooterGame game;

        public static void Create(Transform parent, Vector2 pos, Vector2 dir, SpaceShooterGame game)
        {
            var go = Shape.Root("EnemyBullet", parent, pos);
            var shot = Art.Get("space/laser_enemy");
            if (shot != null)
            {
                Art.Add(go.transform, shot, Vector2.zero, 0.36f, 6, 0f, "Shot");
            }
            else
            {
                Shape.Add(go.transform, SpriteFactory.Circle, new Color(1f, 0.3f, 0.3f, 0.4f), Vector2.zero, Vector2.one * 0.42f, 5);
                Shape.Add(go.transform, SpriteFactory.Circle, new Color(1f, 0.85f, 0.6f), Vector2.zero, Vector2.one * 0.2f, 6);
            }

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.12f;
            col.isTrigger = true;

            var rb = Phys.Body(go);
            rb.SetVel(dir * Speed);
            go.AddComponent<EnemyBullet>().game = game;
        }

        void Update()
        {
            Vector3 p = transform.position;
            if (p.y < -game.HalfH - 1f || p.y > game.HalfH + 2f || Mathf.Abs(p.x) > game.HalfW + 1f)
                Destroy(gameObject);
        }
    }
}
