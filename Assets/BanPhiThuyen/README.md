# Bắn Phi Thuyền

Game 2D viết hoàn toàn bằng C# cho Unity. Bấm **Play** là vào thẳng game, không cần kéo thả gì.
Nhân vật và cảnh nền dùng ảnh thật CC0 (xem `ART_CREDITS.md`), nằm trong `Resources/MiniGamesArt`.

## Mở project

1. Unity Hub → **Projects** → **Add** → **Add project from disk** → chọn thư mục `BanPhiThuyen`.
2. Mở project (lần đầu Unity tạo thư mục `Library`, mất vài phút).
3. Mở `Assets/Scenes/SampleScene` rồi bấm **Play** ▶.

## Điều khiển

| Game | Di chuyển | Hành động |
|---|---|---|
| Bắn Phi Thuyền | ← → ↑ ↓ hoặc WASD | SPACE / J / chuột trái: bắn |

**Esc / P**: tạm dừng. Khi thua hoặc thắng: **R / Enter** chơi lại, **Esc** thoát game.

## Đối tượng và tương tác

| Đối tượng | Lớp | Thành phần Unity | Vai trò |
|---|---|---|---|
| Phi thuyền | `PlayerShip` | Rigidbody2D, CircleCollider2D (Trigger) | Người chơi điều khiển, bắn đạn, có 3 máu |
| Đạn | `Bullet` | Rigidbody2D, BoxCollider2D (Trigger) | Bay lên, trúng địch thì gây sát thương |
| Thiên thạch | `Asteroid : Enemy` | Rigidbody2D, CircleCollider2D (Trigger) | Rơi xuống, xoay, nảy ở mép; loại lớn có 2 máu |
| Tàu địch | `EnemyShip : Enemy` | Rigidbody2D, BoxCollider2D (Trigger) | Bay lượn sóng, bắn đạn về phía người chơi, 3 máu |
| Đạn địch | `EnemyBullet` | Rigidbody2D, CircleCollider2D (Trigger) | Bay về phía phi thuyền |
| Vật phẩm | `PowerUp` | Rigidbody2D, CircleCollider2D (Trigger) | ♥ hồi máu, ngôi sao vàng cho bắn đạn đôi 8 giây |
| Bộ sinh quái | `SpaceShooterGame` | (script) | Sinh quái theo đợt; đợt sau đông và nhanh hơn |

| Tương tác | Kết quả | Xử lý ở |
|---|---|---|
| Đạn chạm Địch | Địch mất máu; hết máu thì nổ, cộng điểm, có thể rơi vật phẩm | `Bullet.OnTriggerEnter2D` |
| Thiên thạch lớn bị phá | Vỡ thành 2 thiên thạch nhỏ | `Asteroid.OnDie` |
| Địch chạm Phi thuyền | Phi thuyền mất 1 máu, địch nổ | `PlayerShip.OnTriggerEnter2D` |
| Đạn địch chạm Phi thuyền | Phi thuyền mất 1 máu | `PlayerShip.OnTriggerEnter2D` |
| Phi thuyền chạm Vật phẩm | Hồi máu / bắn đạn đôi | `PowerUp.Apply` |
| Diệt hết quái trong đợt | Thưởng điểm, sang đợt mới | `SpaceShooterGame.Tick` |

## Hình ảnh

| Game | Nhân vật / đối tượng | Môi trường |
|---|---|---|
| Bắn Phi Thuyền | Phi thuyền xanh có lửa động cơ, 3 kiểu tàu địch, 6 kiểu thiên thạch, tia laser, vật phẩm tim / ngôi sao | Nền vũ trụ tím + sao trôi 3 lớp |

## Cấu trúc code

```
Scripts/
  Core/            Phần khung: MiniGameBase (điểm, mạng, HUD, tạm dừng), Launcher (tự khởi động game),
                   Art (nạp ảnh), SpriteFactory, SoundFx, Fx, UI, GameInput
  SpaceShooter/   Code riêng của game
```
