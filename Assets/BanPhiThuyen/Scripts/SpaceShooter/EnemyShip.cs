using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Tàu địch: bay xuống theo đường lượn sóng, định kỳ bắn đạn về phía người chơi. 3 máu.</summary>
    public class EnemyShip : Enemy
    {
        float baseX, t, speed, swayAmp, swayFreq, shootTimer;

        protected override Color DebrisColor => new Color(1f, 0.4f, 0.3f);
        protected override float DropChance => 0.3f;

        public static EnemyShip Create(Transform parent, Vector2 pos, SpaceShooterGame game)
        {
            var go = Shape.Root("EnemyShip", parent, pos);
            var t = go.transform;
            var shipArt = Art.Get("space/enemy_" + Random.Range(1, 4));
            if (shipArt != null)
            {
                Art.Add(t, shipArt, Vector2.zero, 0.9f, 4, 0f, "Body");
            }
            else
            {
                Shape.Add(t, SpriteFactory.Triangle, Shape.Hex("#8f1d2c"), new Vector2(0f, 0.18f), new Vector2(1.4f, 0.55f), 3, 180f, "Wings");
                Shape.Add(t, SpriteFactory.Triangle, Shape.Hex("#ff4d5e"), Vector2.zero, new Vector2(0.75f, 1.05f), 4, 180f, "Body");
                Shape.Add(t, SpriteFactory.Circle, Shape.Hex("#ffe28a"), new Vector2(0f, 0.12f), new Vector2(0.24f, 0.24f), 5, 0f, "Eye");
            }

            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.1f, 0.8f);
            col.isTrigger = true;
            Phys.Body(go);

            var e = go.AddComponent<EnemyShip>();
            e.baseX = pos.x;
            e.speed = 1.4f * game.SpeedMul;
            e.swayAmp = Random.Range(0.8f, 2f);
            e.swayFreq = Random.Range(1.2f, 2.2f);
            e.shootTimer = Random.Range(0.8f, 1.6f);
            e.Setup(game, 3, 50);
            return e;
        }

        void FixedUpdate()
        {
            t += Time.fixedDeltaTime;
            float x = baseX + Mathf.Sin(t * swayFreq) * swayAmp;
            x = Mathf.Clamp(x, -game.HalfW + 0.7f, game.HalfW - 0.7f);
            float y = rb.position.y - speed * Time.fixedDeltaTime;
            rb.MovePosition(new Vector2(x, y));
        }

        protected override void Update()
        {
            base.Update();
            if (!game.IsPlaying) return;
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f && transform.position.y < game.HalfH - 0.5f && transform.position.y > -game.HalfH + 2f)
            {
                Vector2 from = (Vector2)transform.position + Vector2.down * 0.6f;
                Vector2 dir = Vector2.down;
                if (game.player != null && game.player.gameObject.activeSelf)
                    dir = ((Vector2)game.player.transform.position - from).normalized;
                EnemyBullet.Create(game.World, from, dir, game);
                shootTimer = Random.Range(1.4f, 2.6f) / game.SpeedMul;
            }
        }
    }
}
