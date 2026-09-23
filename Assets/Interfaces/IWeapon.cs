public interface IWeapon
{
    int damage { get; set; }
    float range { get; set; }
    float attackSpeed { get; set; }
    string weaponType { get; set; }

    void Attack();
}