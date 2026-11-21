using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Thiên thạch: rơi xuống, xoay tròn, nảy ở mép màn hình. Loại lớn bị phá sẽ vỡ đôi.</summary>
    public class Asteroid : Enemy
    {
        const float BigSize = 1.05f;
        float size;
        float spin;
        Transform visual;

        protected override Color DebrisColor => new Color(0.75f, 0.7f, 0.65f);

        public static Asteroid Create(Transform parent, Vector2 pos, SpaceShooterGame game, float size, Vector2? velocity)
        {
            var go = Shape.Root("Asteroid", parent, pos);
            var visual = new GameObject("Visual").transform;
            visual.SetParent(go.transform, false);

            var rockArt = size > BigSize
                ? Art.Get("space/meteor_big_" + Random.Range(1, 5))
                : Art.Get("space/meteor_small_" + Random.Range(1, 3));
            if (rockArt != null)
            {
                Art.Add(visual, rockArt, Vector2.zero, size * 0.9f, 3, 0f, "Rock");
            }
            else
            {
                Shape.Add(visual, SpriteFactory.Circle, Shape.Hex("#8a7f78"), Vector2.zero, new Vector2(size, size * 0.9f), 3);
                for (int i = 0; i < 3; i++)
                {
                    var off = Random.insideUnitCircle * size * 0.25f;
                    float s = size * Random.Range(0.18f, 0.3f);
                    Shape.Add(visual, SpriteFactory.Circle, Shape.Hex("#5e5550"), off, new Vector2(s, s), 4);
                }
            }

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = size * 0.45f;
            col.isTrigger = true;

            var rb = Phys.Body(go);
            rb.SetVel(velocity ?? new Vector2(Random.Range(-0.6f, 0.6f), -Random.Range(1.8f, 3.2f) * game.SpeedMul));

            var a = go.AddComponent<Asteroid>();
            a.size = size;
            a.spin = Random.Range(-120f, 120f);
            a.visual = visual;
            bool big = size > BigSize;
            a.Setup(game, big ? 2 : 1, big ? 20 : 10);
            return a;
        }

        protected override void Update()
        {
            base.Update();
            visual.Rotate(0f, 0f, spin * Time.deltaTime);

            // Nảy lại khi chạm mép trái / phải
            Vector2 v = rb.GetVel();
            float x = transform.position.x;
            if ((x > game.HalfW - size * 0.5f && v.x > 0f) || (x < -game.HalfW + size * 0.5f && v.x < 0f))
                rb.SetVel(new Vector2(-v.x, v.y));
        }

        // ===== TƯƠNG TÁC: thiên thạch lớn vỡ thành 2 thiên thạch nhỏ =====
        protected override void OnDie(bool byPlayer)
        {
            if (!byPlayer || size <= BigSize) return;
            Vector2 p = transform.position;
            float fall = Mathf.Abs(rb.GetVel().y) + 0.5f;
            Create(game.World, p + new Vector2(-0.3f, 0f), game, size * 0.55f, new Vector2(-1.6f, -fall));
            Create(game.World, p + new Vector2(0.3f, 0f), game, size * 0.55f, new Vector2(1.6f, -fall));
        }
    }
}
