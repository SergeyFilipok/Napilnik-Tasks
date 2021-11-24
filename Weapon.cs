using System;

public class Weapon {
    public int Damage { get; private set; }
    public int Bullets { get; private set; }

    public bool HasBulletsInClip => Bullets > 0;

    public Weapon(int damage, int bullets) {
        Damage = damage;
        Bullets = bullets;
    }

    public void Fire(IDamageable damageable) {
        if (Bullets > 0) {
            damageable.TakeDamage(Damage);
            Bullets -= 1;
        }
        else {
            throw new InvalidOperationException("No bullets!");
        }
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

    public Bot(Weapon weapon) {
        Weapon = weapon;
    }

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
