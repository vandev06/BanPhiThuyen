using UnityEngine;

namespace MiniGames.SpaceShooter
{
    /// <summary>
    /// Lớp cha cho mọi kẻ địch (thiên thạch, tàu địch).
    /// Thể hiện tính KẾ THỪA: phần chung (máu, điểm, nổ, rơi đồ) viết một lần ở đây.
    /// </summary>
    public abstract class Enemy : MonoBehaviour
    {
        protected SpaceShooterGame game;
        protected Rigidbody2D rb;
        public int hp;
        public int scoreValue;

        SpriteRenderer[] renderers;
        Color[] baseColors;
        float flashTimer;
        bool dead;

        protected abstract Color DebrisColor { get; }
        protected virtual float DropChance => 0.1f;

        protected void Setup(SpaceShooterGame g, int hp, int scoreValue)
        {
            game = g;
            this.hp = hp;
            this.scoreValue = scoreValue;
            rb = GetComponent<Rigidbody2D>();
            renderers = GetComponentsInChildren<SpriteRenderer>();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) baseColors[i] = renderers[i].color;
            game.enemies.Add(this);
        }

        protected virtual void Update()
        {
            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0f)
                    for (int i = 0; i < renderers.Length; i++) renderers[i].color = baseColors[i];
            }
            if (transform.position.y < -game.HalfH - 1.5f) Destroy(gameObject); // lọt xuống dưới
        }

        public void TakeDamage(int dmg)
        {
            if (dead) return;
            hp -= dmg;
            if (hp <= 0) { Die(true); return; }
            // Nháy trắng khi trúng đạn
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = baseColors[i] == Color.white ? new Color(1f, 0.4f, 0.4f) : Color.white;
            flashTimer = 0.07f;
            SoundFx.Play(Sfx.Point, 0.4f);
        }

        /// <summary>Địch nổ. byPlayer = true nếu do người chơi bắn hạ (được điểm, có thể rơi đồ).</summary>
        public void Die(bool byPlayer)
        {
            if (dead) return;
            dead = true;
            Vector2 p = transform.position;
            if (byPlayer)
            {
                game.AddScore(scoreValue, p);
                if (Random.value < DropChance) PowerUp.Create(game.World, p, Random.value < 0.5f ? PowerUp.Kind.Health : PowerUp.Kind.DoubleShot);
            }
            OnDie(byPlayer);
            Fx.Burst(p, DebrisColor, 16, 4.5f, game.World);
            SoundFx.Play(Sfx.Explode, 0.8f);
            Destroy(gameObject);
        }

        protected virtual void OnDie(bool byPlayer) { }

        protected virtual void OnDestroy()
        {
            if (game != null) game.enemies.Remove(this);
        }
    }
}
