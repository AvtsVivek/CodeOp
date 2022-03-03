using System;
using System.Linq;
using AutoFixture;
using Shouldly;
using Xunit;

namespace EventSourcing.Demo
{
    public class WarehouseProductTests
    {
        private readonly string _sku;
        private readonly int _initialQuantity;
        private readonly WarehouseProduct _sut;
        private readonly Fixture _fixture;

        public WarehouseProductTests()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new Int32SequenceGenerator());
            _sku = _fixture.Create<string>();
            _initialQuantity = (int)_fixture.Create<uint>();

            _sut = WarehouseProduct.Load(_sku, new [] {
                new ProductReceived(_sku, _initialQuantity, DateTime.UtcNow)
            });
        }

        [Fact]
        public void ShipProductShouldRaiseProductShipped()
        {
            var quantityToShip = _fixture.Create<int>();
            _sut.ShipProduct(quantityToShip);

            var outEvents = _sut.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);
            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<ProductShipped>();

            var productShipped = (ProductShipped)outEvent;
            productShipped.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityToShip),
                x => x.Sku.ShouldBe(_sku),
                x => x.EventType.ShouldBe("ProductShipped")
            );
        }

        [Fact]
        public void ShipProductShouldThrowIfNoQuantityOnHand()
        {
            var ex = Should.Throw<InvalidDomainException>(() => _sut.ShipProduct(_initialQuantity + 1));
            ex.Message.ShouldBe("Cannot Ship to a negative Quantity on Hand.");
        }

        [Fact]
        public void ReceiveProductShouldRaiseProductReceived()
        {
            var quantityToReceive = _fixture.Create<int>();
            _sut.ReceiveProduct(quantityToReceive);

            var outEvents = _sut.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);
            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<ProductReceived>();

            var productReceived = (ProductReceived)outEvent;
            productReceived.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityToReceive),
                x => x.Sku.ShouldBe(_sku),
                x => x.EventType.ShouldBe("ProductReceived")
            );
        }

        [Fact]
        public void AdjustInventoryShouldRaiseProductAdjusted()
        {
            var quantityToAdjust = _fixture.Create<int>();
            var reason = _fixture.Create<string>();
            _sut.AdjustInventory(quantityToAdjust, reason);

            var outEvents = _sut.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);
            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<InventoryAdjusted>();

            var productShipped = (InventoryAdjusted)outEvent;
            productShipped.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityToAdjust),
                x => x.Sku.ShouldBe(_sku),
                x => x.Reason.ShouldBe(reason),
                x => x.EventType.ShouldBe("InventoryAdjusted")
            );
        }

        [Fact]
        public void AdjustInventoryShouldThrowIfNoQuantityOnHand()
        {
            var ex = Should.Throw<InvalidDomainException>(() => _sut.AdjustInventory((_initialQuantity + 1) * -1, string.Empty));
            ex.Message.ShouldBe("Cannot adjust to a negative Quantity on Hand.");
        }
    }
}