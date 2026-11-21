using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>
    /// GAME 1 – BẮN PHI THUYỀN
    /// Điều khiển cả màn chơi: sinh quái theo đợt (Spawner), đợt sau đông và nhanh hơn.
    ///
    /// Đối tượng: PlayerShip, Bullet, Asteroid, EnemyShip, EnemyBullet, PowerUp, StarField
    /// Tương tác chính (xem OnTriggerEnter2D trong từng lớp):
    ///   Bullet   chạm Enemy      → Enemy mất máu / nổ, cộng điểm (Bullet.cs)
    ///   Enemy    chạm PlayerShip → phi thuyền mất 1 máu, Enemy nổ (PlayerShip.cs)
    ///   EnemyBullet chạm PlayerShip → mất 1 máu (PlayerShip.cs)
    ///   PlayerShip chạm PowerUp  → hồi máu hoặc bắn đạn đôi (PlayerShip.cs → PowerUp.Apply)
    ///   Thiên thạch lớn bị phá   → vỡ thành 2 thiên thạch nhỏ (Asteroid.cs)
    /// </summary>
    public class SpaceShooterGame : MiniGameBase
    {
        public override string Title => "Bắn Phi Thuyền";
        protected override string SaveKey => "shooter";
        protected override Color Background => Shape.Hex("#0b0d1f");
        protected override string HelpText =>
            "← → ↑ ↓ hoặc W A S D: di chuyển     SPACE / J / chuột trái: bắn\n\n" +
            "Đạn trúng địch → địch nổ, cộng điểm.  Thiên thạch lớn vỡ thành 2 mảnh.\n" +
            "Địch hoặc đạn địch chạm phi thuyền → mất 1 máu.\n" +
            "Nhặt <color=#ff77aa>♥</color> để hồi máu, <color=#ffdd33>ngôi sao vàng</color> để bắn đạn đôi trong 8 giây.\n" +
            "Mỗi đợt quái đông và nhanh hơn đợt trước!";

        public PlayerShip player;
        public readonly List<Enemy> enemies = new List<Enemy>();

        int wave;
        int toSpawn;
        float spawnTimer;
        float bannerTimer;

        float SpawnInterval => Mathf.Max(0.3f, 1.1f - wave * 0.08f);
        public float SpeedMul => 1f + (wave - 1) * 0.12f;
        float ShipChance => Mathf.Min(0.5f, 0.05f + wave * 0.07f);

        protected override void BuildWorld()
        {
            enemies.Clear();
            StarField.Create(world, this);
            player = PlayerShip.Create(world, new Vector2(0f, -halfH + 1.3f), this);
            wave = 0;
            StartNextWave();
        }

        void StartNextWave()
        {
            wave++;
            toSpawn = 5 + wave * 3;
            spawnTimer = 2f;
            bannerTimer = 2f;
        }

        protected override void Tick()
        {
            float dt = Time.deltaTime;
            if (bannerTimer > 0f) bannerTimer -= dt;

            if (toSpawn > 0)
            {
                spawnTimer -= dt;
                if (spawnTimer <= 0f)
                {
                    SpawnEnemy();
                    toSpawn--;
                    spawnTimer = SpawnInterval * Random.Range(0.7f, 1.3f);
                }
            }
            else if (enemies.Count == 0)
            {
                // Hết đợt: thưởng điểm rồi sang đợt mới
                AddScore(wave * 25, new Vector3(0f, 1f, 0f), new Color(0.5f, 1f, 0.6f));
                StartNextWave();
            }
        }

        void SpawnEnemy()
        {
            var pos = new Vector2(Random.Range(-halfW + 0.9f, halfW - 0.9f), halfH + 1f);
            if (Random.value < ShipChance)
                EnemyShip.Create(world, pos, this);
            else
                Asteroid.Create(world, pos, this, Random.Range(0.6f, 1.4f), null);
        }

        protected override void DrawHud()
        {
            UI.Text(new Rect(0, 14, UI.W, 40), "Đợt " + wave, 28, new Color(0.6f, 0.85f, 1f), TextAnchor.UpperCenter);
            if (player != null && player.doubleShotTime > 0f)
                UI.Text(new Rect(0, 50, UI.W, 30), "Đạn đôi: " + Mathf.CeilToInt(player.doubleShotTime) + "s", 20, Color.yellow, TextAnchor.UpperCenter);
            if (bannerTimer > 0f && state == State.Playing)
                UI.Center(300, "ĐỢT " + wave, 72, new Color(1f, 1f, 1f, Mathf.Clamp01(bannerTimer)), 100);
        }
    }
}
