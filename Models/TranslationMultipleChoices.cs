namespace Vocab.Models
{
    public class TranslationMultipleChoices
    {
        public required string Answer { get; set; }
        public required string Word { get; set; }
        public required List<string> Choices { get; set; }
    }
}
