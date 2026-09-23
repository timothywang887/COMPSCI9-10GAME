public interface IUISlider
{
    float value { get; set; }
    float minValue { get; set; }
    float maxValue { get; set; }
    bool isInteractable { get; set; }

    void onValueChanged(float newValue);
    void onDragStart();
    void onDragEnd();
}