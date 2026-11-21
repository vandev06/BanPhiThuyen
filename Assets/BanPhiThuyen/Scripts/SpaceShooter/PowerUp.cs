using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Vật phẩm rơi ra khi bắn hạ địch: ♥ hồi máu, ★ bắn đạn đôi.</summary>
    public class PowerUp : MonoBehaviour
    {
        public enum Kind { Health, DoubleShot }
        public Kind kind;
        Transform visual;
        float t;
        bool taken;

        public static void Create(Transform parent, Vector2 pos, Kind kind)
        {
            var go = Shape.Root("PowerUp_" + kind, parent, pos);
            var visual = new GameObject("Visual").transform;
            visual.SetParent(go.transform, false);
            Color glow = kind == Kind.Health ? new Color(1f, 0.4f, 0.6f, 0.3f) : new Color(1f, 0.9f, 0.2f, 0.3f);
            Shape.Add(visual, SpriteFactory.Circle, glow, Vector2.zero, Vector2.one * 0.9f, 6);
            var icon = Art.Get(kind == Kind.Health ? "space/powerup_heart" : "space/powerup_star");
            if (icon != null)
                Art.Add(visual, icon, Vector2.zero, kind == Kind.Health ? 0.5f : 0.6f, 7, 0f, "Icon");
            else if (kind == Kind.Health)
                Shape.Add(visual, SpriteFactory.Heart, Shape.Hex("#ff5c8a"), Vector2.zero, Vector2.one * 0.55f, 7);
            else
                Shape.Add(visual, SpriteFactory.Star, Shape.Hex("#ffdd33"), Vector2.zero, Vector2.one * 0.65f, 7);

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;
            var rb = Phys.Body(go);
            rb.SetVel(Vector2.down * 2f);

            var pu = go.AddComponent<PowerUp>();
            pu.kind = kind;
            pu.visual = visual;
        }

        void Update()
        {
            t += Time.deltaTime;
            visual.localScale = Vector3.one * (1f + Mathf.Sin(t * 6f) * 0.12f);
            if (transform.position.y < -8f) Destroy(gameObject);
        }

        /// <summary>Gọi khi phi thuyền chạm vào vật phẩm.</summary>
        public void Apply(PlayerShip ship, SpaceShooterGame game)
        {
            if (taken) return;
            taken = true;
            if (kind == Kind.Health)
            {
                game.GainLife(transform.position);
            }
            else
            {
                ship.doubleShotTime = 8f;
                game.ShowPopup(transform.position, "ĐẠN ĐÔI!", Color.yellow);
            }
            SoundFx.Play(Sfx.PowerUp);
            Fx.Burst(transform.position, kind == Kind.Health ? Color.magenta : Color.yellow, 10, 3f, game.World, 0.4f, 0.12f, 0f, false);
            Destroy(gameObject);
        }
    }
}
