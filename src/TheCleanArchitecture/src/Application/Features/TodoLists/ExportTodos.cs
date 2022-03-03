using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.WebUI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Application.TodoLists.Queries.ExportTodos
{
    [Authorize]
    public class ExportTodosController : ApiControllerBase
    {
        [HttpGet("{id}")]
        public async Task<FileResult> Get(int id)
        {
            var vm = await Mediator.Send(new ExportTodosQuery { ListId = id });

            return File(vm.Content, vm.ContentType, vm.FileName);
        }
    }

    public class ExportTodosQuery : IRequest<ExportTodosVm>
    {
        public int ListId { get; set; }
    }

    public class ExportTodosVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }

    public class TodoItemRecord : IMapFrom<TodoItem>
        {
            public string Title { get; set; }

            public bool Done { get; set; }
        }

    public class ExportTodosQueryHandler : IRequestHandler<ExportTodosQuery, ExportTodosVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICsvFileBuilder _fileBuilder;

        public ExportTodosQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
        }

        public async Task<ExportTodosVm> Handle(ExportTodosQuery request, CancellationToken cancellationToken)
        {
            var vm = new ExportTodosVm();

            var records = await _context.TodoItems
                    .Where(t => t.ListId == request.ListId)
                    .ProjectTo<TodoItemRecord>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

            vm.Content = _fileBuilder.BuildTodoItemsFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = "TodoItems.csv";

            return await Task.FromResult(vm);
        }
    }
}
