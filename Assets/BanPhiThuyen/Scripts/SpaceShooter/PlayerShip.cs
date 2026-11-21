using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Phi thuyền của người chơi: di chuyển, bắn đạn, nhận sát thương, nhặt vật phẩm.</summary>
    public class PlayerShip : MonoBehaviour
    {
        const float Speed = 7.5f;
        const float FireCooldown = 0.18f;
        const float InvulnTime = 1.5f;

        SpaceShooterGame game;
        Rigidbody2D rb;
        Transform flame;
        Vector3 flameScale;
        SpriteRenderer[] parts;
        Vector2 input;
        float fireTimer;
        float invuln;
        public float doubleShotTime;

        public static PlayerShip Create(Transform parent, Vector2 pos, SpaceShooterGame game)
        {
            var go = Shape.Root("PlayerShip", parent, pos);
            var t = go.transform;
            SpriteRenderer flame;
            var shipArt = Art.Get("space/player_ship");
            var fireArt = Art.Get("space/engine_fire");
            if (shipArt != null && fireArt != null)
            {
                // Hình thật: phi thuyền Kenney + lửa động cơ ở đuôi
                Art.Add(t, shipArt, Vector2.zero, 0.95f, 10, 0f, "Body");
                flame = Art.Add(t, fireArt, new Vector2(0f, -0.7f), 0.45f, 8, 0f, "Flame");
            }
            else
            {
                Shape.Add(t, SpriteFactory.Triangle, Shape.Hex("#2a6fdb"), new Vector2(0f, -0.22f), new Vector2(1.45f, 0.62f), 9, 0f, "Wings");
                Shape.Add(t, SpriteFactory.Triangle, Shape.Hex("#7fd4ff"), new Vector2(0f, 0.05f), new Vector2(0.72f, 1.25f), 10, 0f, "Body");
                Shape.Add(t, SpriteFactory.Circle, Shape.Hex("#ffffff"), new Vector2(0f, 0.02f), new Vector2(0.22f, 0.32f), 11, 0f, "Cockpit");
                flame = Shape.Add(t, SpriteFactory.Triangle, Shape.Hex("#ff9a2e"), new Vector2(0f, -0.66f), new Vector2(0.34f, 0.45f), 8, 180f, "Flame");
            }

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.38f;
            col.isTrigger = true;

            var ship = go.AddComponent<PlayerShip>();
            ship.game = game;
            ship.rb = Phys.Body(go);
            ship.flame = flame.transform;
            ship.flameScale = flame.transform.localScale;
            ship.parts = go.GetComponentsInChildren<SpriteRenderer>();
            return ship;
        }

        void Update()
        {
            if (!game.IsPlaying) { input = Vector2.zero; return; }
            float dt = Time.deltaTime;

            input = new Vector2(GameInput.Horizontal(), GameInput.Vertical());
            if (input.sqrMagnitude > 1f) input.Normalize();

            fireTimer -= dt;
            if (GameInput.FireHeld() && fireTimer <= 0f)
            {
                Fire();
                fireTimer = FireCooldown;
            }

            if (doubleShotTime > 0f) doubleShotTime -= dt;

            // Nhấp nháy khi đang bất tử tạm thời
            if (invuln > 0f) invuln -= dt;
            bool visible = invuln <= 0f || Mathf.Repeat(Time.time * 12f, 1f) < 0.5f;
            foreach (var p in parts) p.enabled = visible;

            // Lửa động cơ rung rinh
            flame.localScale = new Vector3(flameScale.x, flameScale.y * (0.9f + Mathf.PingPong(Time.time * 4f, 0.55f)), 1f);
        }

        void FixedUpdate()
        {
            Vector2 target = rb.position + input * Speed * Time.fixedDeltaTime;
            target.x = Mathf.Clamp(target.x, -game.HalfW + 0.7f, game.HalfW - 0.7f);
            target.y = Mathf.Clamp(target.y, -game.HalfH + 0.6f, -game.HalfH + 4.2f);
            rb.MovePosition(target);
        }

        void Fire()
        {
            Vector2 p = transform.position;
            if (doubleShotTime > 0f)
            {
                Bullet.Create(game.World, p + new Vector2(-0.3f, 0.45f), game);
                Bullet.Create(game.World, p + new Vector2(0.3f, 0.45f), game);
            }
            else
            {
                Bullet.Create(game.World, p + new Vector2(0f, 0.75f), game);
            }
            SoundFx.Play(Sfx.Shoot, 0.6f);
        }

        // ===== TƯƠNG TÁC: phi thuyền chạm vào đối tượng khác =====
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!game.IsPlaying) return;

            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die(false);   // địch đâm vào cũng nổ, nhưng không được điểm
                TakeHit();
                return;
            }

            var eb = other.GetComponent<EnemyBullet>();
            if (eb != null)
            {
                Destroy(eb.gameObject);
                TakeHit();
                return;
            }

            var pu = other.GetComponent<PowerUp>();
            if (pu != null) pu.Apply(this, game);
        }

        void TakeHit()
        {
            if (invuln > 0f) return;
            SoundFx.Play(Sfx.Hurt);
            Fx.Burst(transform.position, new Color(0.5f, 0.8f, 1f), 10, 3f, game.World);
            if (game.LoseLife())
            {
                invuln = InvulnTime;
            }
            else
            {
                // Hết máu: phi thuyền nổ tung
                Fx.Burst(transform.position, new Color(1f, 0.6f, 0.2f), 40, 6f, game.World, 0.9f, 0.25f);
                SoundFx.Play(Sfx.Explode);
                gameObject.SetActive(false);
            }
        }
    }
}
