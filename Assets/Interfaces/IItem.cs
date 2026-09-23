public interface IItem
{
    string name { get; set; }
    string description { get; set; }
    string itemType { get; set; }
    string iconPath { get; set; }

    void Use();
}