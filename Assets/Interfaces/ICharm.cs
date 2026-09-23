public interface ICharm
{
    string Name { get; set; }
    string Description { get; set; }
    string CharmType { get; set; }
    string IconPath { get; set; }

    void Activate();
}