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
    
    void onClickStart();
    void onClickEnd();
    void onHoverStart();
    void onHoverEnd();
    void onTextChanged(string newText);
    void click(); // call this to simulate a click on the button
}