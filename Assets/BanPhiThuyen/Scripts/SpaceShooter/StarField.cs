using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>Nền sao trôi xuống tạo cảm giác phi thuyền đang bay (3 lớp tốc độ = hiệu ứng parallax).</summary>
    public class StarField : MonoBehaviour
    {
        MiniGameBase game;
        Transform[] stars;
        float[] speeds;

        public static void Create(Transform parent, MiniGameBase game)
        {
            var go = new GameObject("StarField");
            go.transform.SetParent(parent, false);
            var sf = go.AddComponent<StarField>();
            sf.game = game;

            // Ảnh nền vũ trụ lát kín màn hình, nằm sau các ngôi sao
            var bg = Art.Get("space/background");
            if (bg != null)
                Art.AddTiled(go.transform, bg, Vector2.zero, new Vector2(game.HalfW * 2f + 2f, game.HalfH * 2f + 2f), 5f, -30, 0f, "Background");

            const int n = 70;
            sf.stars = new Transform[n];
            sf.speeds = new float[n];
            for (int i = 0; i < n; i++)
            {
                int layer = i % 3;
                float s = 0.04f + layer * 0.03f;
                float a = 0.3f + layer * 0.3f;
                var pos = new Vector2(Random.Range(-game.HalfW, game.HalfW), Random.Range(-game.HalfH, game.HalfH));
                sf.stars[i] = Shape.Add(go.transform, SpriteFactory.Circle, new Color(1f, 1f, 1f, a), pos, Vector2.one * s, -10).transform;
                sf.speeds[i] = 0.6f + layer * 1.2f;
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < stars.Length; i++)
            {
                var p = stars[i].localPosition;
                p.y -= speeds[i] * dt;
                if (p.y < -game.HalfH - 0.2f)
                {
                    p.y = game.HalfH + 0.2f;
                    p.x = Random.Range(-game.HalfW, game.HalfW);
                }
                stars[i].localPosition = p;
            }
        }
    }
}
