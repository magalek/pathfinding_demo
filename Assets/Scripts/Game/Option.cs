namespace Game
{
    public class Option<T>
    {
        public readonly string DisplayName;
        
        public T Value { get; set; }

        public Option(string displayName, T value)
        {
            DisplayName = displayName;
            Value = value;
        }

    }
}