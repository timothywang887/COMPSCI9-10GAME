public interface IUITextInput
{
    float Width { get; set; }
    float Height { get; set; }
    float PositionX { get; set; }
    float PositionY { get; set; }
    float Rotation { get; set; }
    string Text { get; set; }
    string PlaceholderText { get; set; }
    bool IsInteractable { get; set; }
    
    void OnClickStart();
    void OnClickEnd();
    void OnHoverStart();
    void OnHoverEnd();
    void OnTextChanged(string newText);
    void Click(); // call this to simulate a click on the button
}