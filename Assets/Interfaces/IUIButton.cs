public interface IUIButton
{
    float width { get; set; }
    float height { get; set; }
    float positionX { get; set; }
    float positionY { get; set; }
    float rotation { get; set; }
    string text { get; set; }
    bool isInteractable { get; set; }
    
    void OnClickStart();
    void OnClickEnd();
    void OnHoverStart();
    void OnHoverEnd();
    void Click(); // call this to simulate a click on the button
}