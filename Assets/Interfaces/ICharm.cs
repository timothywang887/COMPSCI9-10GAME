public interface ICharm
{
    string name { get; set; }
    string description { get; set; }
    string charmType { get; set; }
    string iconPath { get; set; }

    void activate();
}