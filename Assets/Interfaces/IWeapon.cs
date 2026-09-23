public interface IWeapon
{
    int Damage { get; set; }
    float Range { get; set; }
    float AttackSpeed { get; set; }
    string WeaponType { get; set; }

    void Attack();
}