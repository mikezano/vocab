namespace Vocab.Models
{
    public class Answer
    {
        public required string Word { get; set; }
        public required string Translation { get; set; }
        public int ReplacementIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}
