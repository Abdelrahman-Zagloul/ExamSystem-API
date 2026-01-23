using ExamSystem.Application.Contracts.Jobs;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Jobs
{
    public class CalculateExamResultJob : ICalculateExamResultJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ExamResult> _examResultRepo;
        private readonly IGenericRepository<StudentAnswer> _studentAnswerRepo;
        private record ExamQuestionSnapshot(int QuestionId, int? CorrectOptionId, double QuestionMark);

        public CalculateExamResultJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _examResultRepo = _unitOfWork.Repository<ExamResult>();
            _studentAnswerRepo = _unitOfWork.Repository<StudentAnswer>();
        }

        public async Task ExecuteAsync(int examId, string studentId)
        {
            // for Idempotency
            var alreadyCalculated = await _examResultRepo.AnyAsync(x => x.ExamId == examId && x.StudentId == studentId);
            if (alreadyCalculated)
                return;

            var studentAnswers = await _studentAnswerRepo.GetAsQuery(false)
                    .Where(x => x.ExamId == examId && x.StudentId == studentId && x.EvaluationStatus == AnswerEvaluationStatus.Pending)
                    .ToListAsync();

            if (!studentAnswers.Any())
                return;

            var examQuestions = await _unitOfWork.Repository<Question>()
                    .GetAsQuery(true).Where(x => x.ExamId == examId)
                    .Select(x => new ExamQuestionSnapshot(x.Id, x.CorrectOptionId, x.QuestionMark))
                    .ToListAsync();


            double score = 0;
            var questionLookup = examQuestions.ToDictionary(q => q.QuestionId);
            foreach (var answer in studentAnswers)
            {
                if (!questionLookup.TryGetValue(answer.QuestionId, out var question))
                    continue;

                if (question.CorrectOptionId == answer.SelectedOptionId)
                {
                    score += question.QuestionMark;
                    answer.EvaluationStatus = AnswerEvaluationStatus.Correct;
                }
                else
                    answer.EvaluationStatus = AnswerEvaluationStatus.Wrong;

            }

            var examResult = new ExamResult
            {
                Score = score,
                ExamId = examId,
                StudentId = studentId,
                TotalMark = examQuestions.Sum(x => x.QuestionMark),
            };
            await _examResultRepo.AddAsync(examResult);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
