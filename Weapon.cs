public class Weapon {
    private const int defaultClipSize = 10;

    public int Damage { get; private set; }
    public int Bullets { get; private set; }

    public bool HasBulletsInClip => Bullets > 0;

    public Weapon(int damage) {
        Damage = damage;

        //должно ли оружие быть заряженным после создания?
        Bullets = defaultClipSize;
    }

    public void Fire(IDamageable damageable) {
        damageable.TakeDamage(Damage);
        Bullets -= 1;

        //должно ли оружие перезаряжаться само?
        if (HasBulletsInClip == false) {
            Reload();
        }
    }

    public void Reload() {
        Bullets = defaultClipSize;
    }
}

public class Player : IDamageable {
    public int Health { get; private set; }
    public bool IsDead => Health <= 0;
    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage) {
        if (IsAlive && damage > 0) {
            Health -= damage;
        }

        if (Health < 0) {
            Health = 0;
        }
    }
}

public class Bot {
    private Weapon _weapon;

    public bool HasWeapon => Weapon != null;
    public Weapon Weapon {
        get => _weapon;
        set {
            _weapon = value;
            if (_weapon == null) {
                throw new NullReferenceException(nameof(_weapon));
            }
        }
    }


    public void OnSeePlayer(Player player) {
        if (HasWeapon && player.IsAlive) {
            Weapon.Fire(player);
        }
    }
}

public interface IDamageable {
    void TakeDamage(int damage);
}
    }