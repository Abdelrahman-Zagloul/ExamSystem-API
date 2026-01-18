using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam
{
    public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitExamCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.EndAt < DateTime.UtcNow)
                return Error.Conflict("ExamTimeFinished", "You cannot submit the exam after it has finished");


            var examResultRepo = _unitOfWork.Repository<ExamResult>();
            var alreadySubmitted = await examResultRepo
                        .AnyAsync(x => x.ExamId == request.ExamId && x.StudentId == request.StudentId, cancellationToken);

            if (alreadySubmitted)
                return Error.Conflict("ExamAlreadySubmitted", "You have already submitted this exam");


            var examQuestions = await _unitOfWork.Repository<Question>()
                        .GetAsQuery(true).Where(x => x.ExamId == exam.Id)
                        .Select(x => new ExamQuestionSnapshot(x.Id, x.CorrectOptionId, x.QuestionMark))
                        .ToListAsync(cancellationToken);


            var result = CalculateResult(examQuestions, request);
            var examResult = new ExamResult
            {
                Score = result.score,
                ExamId = exam.Id,
                StudentId = request.StudentId,
                TotalMark = examQuestions.Sum(x => x.QuestionMark),
            };


            await examResultRepo.AddAsync(examResult);
            await _unitOfWork.Repository<StudentAnswer>().AddRangeAsync(result.answers, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok("Exam Submit Successfully");
        }

        private (double score, List<StudentAnswer> answers) CalculateResult(
            List<ExamQuestionSnapshot> examQuestions, SubmitExamCommand request)
        {
            double score = 0;
            var studentAnswers = new List<StudentAnswer>();

            foreach (var answer in request.Answers)
            {
                var question = examQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (question is null)
                    continue;

                var isCorrect = question.CorrectOptionId == answer.SelectedOptionId;
                if (isCorrect)
                    score += question.QuestionMark;

                studentAnswers.Add(new StudentAnswer
                {
                    IsCorrect = isCorrect,
                    ExamId = request.ExamId,
                    StudentId = request.StudentId,
                    QuestionId = answer.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId
                });
            }
            return (score, studentAnswers);
        }

    }
}
