namespace ExamSystem.Domain.Entities
{
    public class Option
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
