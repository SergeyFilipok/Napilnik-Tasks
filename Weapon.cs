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
        if (HasBulletsInClip == false) {
            throw new InvalidOperationException("No bullets!");
        }

        damageable.TakeDamage(Damage);
        Bullets -= 1;
    }
}

public class Player : IDamageable {
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage) {
        if (IsAlive == false) {
            throw new InvalidOperationException("This player is dead!");
        }

        Health -= damage;

        if (Health < 0) {
            Health = 0;
        }
    }
}

public class Bot {
    private Weapon _weapon;

    public Bot(Weapon weapon) {
        _weapon = weapon ?? throw new NullReferenceException("Weapon in NULL");
    }

    public void OnSeePlayer(Player player) {
        _weapon.Fire(player);
    }
}

public interface IDamageable {
    void TakeDamage(int damage);
}
