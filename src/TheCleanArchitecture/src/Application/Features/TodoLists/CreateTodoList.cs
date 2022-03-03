using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.WebUI.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    [Authorize]
    public class CreateTodoListController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
        {
            return await Mediator.Send(command);
        }
    }

    public class CreateTodoListCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateTodoListCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            return await _context.TodoLists
                .AllAsync(l => l.Title != title);
        }
    }

    public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateTodoListCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoList();

            entity.Title = request.Title;

            _context.TodoLists.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
