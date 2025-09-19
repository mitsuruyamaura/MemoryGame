using R3;

public static class ReactivePropertyExtensions
{
    public static void SetValueForceNotify<T>(this ReactiveProperty<T> reactiveProperty) {
        reactiveProperty.OnNext(reactiveProperty.Value);
    }
}