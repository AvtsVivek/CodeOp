using System;
using AutoFixture;
using Shouldly;
using Xunit;

namespace EventSourcing.Demo
{
    public class WarehouseProductAggregateRootTests : AggregateTests<WarehouseProduct>
    {
        private readonly Fixture _fixture;
        private readonly string _sku = "abc123";
        private readonly int _initialQuantity;

        public WarehouseProductAggregateRootTests() : base(new WarehouseProduct("abc123"))
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new Int32SequenceGenerator());
            _initialQuantity = (int)_fixture.Create<uint>();
        }

        [Fact]
        public void ShipProductShouldRaiseProductShipped()
        {
            Given(new ProductReceived(_sku, _initialQuantity, DateTime.UtcNow));

            var quantityToShip = _fixture.Create<int>();
            When(x => x.ShipProduct(quantityToShip));

            Then<ProductShipped>(
                x => x.Quantity.ShouldBe(quantityToShip),
                x => x.Sku.ShouldBe(_sku),
                x => x.EventType.ShouldBe("ProductShipped"));
        }

        [Fact]
        public void ShipProductShouldThrowIfNoQuantityOnHand()
        {
            Given();

            Throws<InvalidDomainException>(
                x => x.ShipProduct(1),
                x => x.Message.ShouldBe("Cannot Ship to a negative Quantity on Hand."));
        }

        [Fact]
        public void ReceiveProductShouldRaiseProductReceived()
        {
            Given(new ProductReceived(_sku, _initialQuantity, DateTime.UtcNow));

            var quantityToReceive = _fixture.Create<int>();
            When(x => x.ReceiveProduct(quantityToReceive));

            Then<ProductReceived>(
                x => x.Quantity.ShouldBe(quantityToReceive),
                x => x.Sku.ShouldBe(_sku),
                x => x.EventType.ShouldBe("ProductReceived"));
        }

        [Fact]
        public void AdjustInventoryShouldRaiseProductAdjusted()
        {
            Given(new ProductReceived(_sku, _initialQuantity, DateTime.UtcNow));

            var quantityToAdjust = _fixture.Create<int>();
            var reason = _fixture.Create<string>();

            When(x => x.AdjustInventory(quantityToAdjust, reason));

            Then<InventoryAdjusted>(
                x => x.Quantity.ShouldBe(quantityToAdjust),
                x => x.Sku.ShouldBe(_sku),
                x => x.Reason.ShouldBe(reason),
                x => x.EventType.ShouldBe("InventoryAdjusted"));
        }

        [Fact]
        public void AdjustInventoryShouldThrowIfNoQuantityOnHand()
        {
            Given();

            var reason = _fixture.Create<string>();

            Throws<InvalidDomainException>(
                x => x.AdjustInventory(-1, reason),
                x => x.Message.ShouldBe("Cannot adjust to a negative Quantity on Hand."));
        }
    }
}