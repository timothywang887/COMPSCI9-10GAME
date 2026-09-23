public interface IHealth
{
    float currentHealth { get; set; }
    float maxHealth { get; set; }
    bool isAlive { get; set; }

    void ChangeHealth(float _amount);
}