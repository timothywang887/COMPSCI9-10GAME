public interface IItem
{
    string Name { get; set; }
    string Description { get; set; }
    string ItemType { get; set; }
    string IconPath { get; set; }

    void Use();
}