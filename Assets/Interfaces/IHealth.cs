public interface IHealth
{
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }
    bool IsAlive { get; set; }

    void ChangeHealth(float amount);
}