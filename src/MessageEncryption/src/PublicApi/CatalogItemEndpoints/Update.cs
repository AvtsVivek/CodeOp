using System;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace Microsoft.eShopWeb.PublicApi.CatalogItemEndpoints
{
    //[Authorize(Roles = BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Update : BaseAsyncEndpoint
        .WithRequest<UpdateCatalogItemRequest>
        .WithResponse<UpdateCatalogItemResponse>
    {
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IFileSystem _webFileSystem;
        private readonly IMessageSession _messageSession;

        public Update(IAsyncRepository<CatalogItem> itemRepository, IUriComposer uriComposer, IFileSystem webFileSystem, IMessageSession messageSession)
        {
            _itemRepository = itemRepository;
            _uriComposer = uriComposer;
            _webFileSystem = webFileSystem;
            _messageSession = messageSession;
        }

        [HttpPut("api/catalog-items")]
        [SwaggerOperation(
            Summary = "Updates a Catalog Item",
            Description = "Updates a Catalog Item",
            OperationId = "catalog-items.update",
            Tags = new[] { "CatalogItemEndpoints" })
        ]
        public override async Task<ActionResult<UpdateCatalogItemResponse>> HandleAsync(UpdateCatalogItemRequest request, CancellationToken cancellationToken)
        {
            var response = new UpdateCatalogItemResponse(request.CorrelationId());

            var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);

            existingItem.UpdateDetails(request.Name, request.Description, request.Price);
            existingItem.UpdateBrand(request.CatalogBrandId);
            existingItem.UpdateType(request.CatalogTypeId);

            if (string.IsNullOrEmpty(request.PictureBase64) && string.IsNullOrEmpty(request.PictureUri))
            {
                existingItem.UpdatePictureUri(string.Empty);
            }
            else
            {
                var picName = $"{existingItem.Id}{Path.GetExtension(request.PictureName)}";
                if (await _webFileSystem.SavePicture($"{picName}", request.PictureBase64, cancellationToken))
                {
                    existingItem.UpdatePictureUri(picName);
                }
            }

            await _itemRepository.UpdateAsync(existingItem, cancellationToken);

            if (string.IsNullOrEmpty(request.PictureBase64) == false)
            {
                await _messageSession.Publish(new CatalogItemImageUpdated
                {
                    CatalogItemId = existingItem.Id,
                    Image = new DataBusProperty<byte[]>(Encoding.UTF8.GetBytes(request.PictureBase64))
                });
            }

            var dto = new CatalogItemDto
            {
                Id = existingItem.Id,
                CatalogBrandId = existingItem.CatalogBrandId,
                CatalogTypeId = existingItem.CatalogTypeId,
                Description = existingItem.Description,
                Name = existingItem.Name,
                PictureUri = _uriComposer.ComposePicUri(existingItem.PictureUri),
                Price = existingItem.Price
            };
            response.CatalogItem = dto;
            return response;
        }
    }

    public class CatalogItemImageUpdated : IEvent
    {
        public int CatalogItemId { get; set; }
        public DataBusProperty<byte[]> Image { get; set; }
    }

    public class ResizeCatalogItemHandler : IHandleMessages<CatalogItemImageUpdated>
    {
        public Task Handle(CatalogItemImageUpdated message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.Image.Value);
            return Task.CompletedTask;
        }
    }

}
