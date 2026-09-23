public interface IUIButton
{
    float Width { get; set; }
    float Height { get; set; }
    float PositionX { get; set; }
    float PositionY { get; set; }
    float Rotation { get; set; }
    string Text { get; set; }
    bool IsInteractable { get; set; }
    
    void OnClickStart();
    void OnClickEnd();
    void OnHoverStart();
    void OnHoverEnd();
    void Click(); // call this to simulate a click on the button
}