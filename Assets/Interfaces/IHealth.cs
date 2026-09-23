public interface IHealth
{
    float currentHealth { get; set; }
    float maxHealth { get; set; }
    bool isAlive { get; set; }

    void changeHealth(float amount);
}