namespace Vocab.Models
{
    public class TranslationItem
    {
        public required string Word { get; set; }
        public required string Translation { get; set; }
        public bool IsGuessed { get; set; } = false;

        public TranslationItem Clone()
        {
            return new TranslationItem()
            {
                Word = Word,
                Translation = Translation,
                IsGuessed = IsGuessed
            };
        }
    }
}
