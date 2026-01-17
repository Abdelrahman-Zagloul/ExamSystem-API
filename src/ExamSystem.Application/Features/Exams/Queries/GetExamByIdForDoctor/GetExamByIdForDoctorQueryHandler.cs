using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor
{
    public class GetExamByIdForDoctorQueryHandler : IRequestHandler<GetExamByIdForDoctorQuery, Result<ExamDetailsForDoctorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamByIdForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ExamDetailsForDoctorDto>> Handle(GetExamByIdForDoctorQuery request, CancellationToken cancellationToken)
        {
            var examQuery = _unitOfWork.Repository<Exam>().GetAsQuery(true)
                .Where(x => x.Id == request.ExamId);

            var examDto = await examQuery.ProjectTo<ExamDetailsForDoctorDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (examDto == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (examDto.DoctorId != request.DoctorId)
                return Error.Forbidden(description: "You don't have permission to access this exam");

            return examDto;
        }
    }
}
