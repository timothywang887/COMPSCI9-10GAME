public interface IUISlider
{
    float Value { get; set; }
    float MinValue { get; set; }
    float MaxValue { get; set; }
    bool IsInteractable { get; set; }

    void OnValueChanged(float newValue);
    void OnDragStart();
    void OnDragEnd();
}