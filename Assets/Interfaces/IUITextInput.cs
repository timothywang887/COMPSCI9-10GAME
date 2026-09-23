public interface IUITextInput
{
    float width { get; set; }
    float height { get; set; }
    float positionX { get; set; }
    float positionY { get; set; }
    float rotation { get; set; }
    string text { get; set; }
    string placeholderText { get; set; }
    bool isInteractable { get; set; }
    
    void OnClickStart();
    void OnClickEnd();
    void OnHoverStart();
    void OnHoverEnd();
    void OnTextChanged(string _newText);
    void Click(); // call this to simulate a click on the button
}