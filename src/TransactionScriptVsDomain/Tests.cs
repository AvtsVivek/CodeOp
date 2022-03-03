using System;
using System.Linq;
using Demo.EventSourced;
using Shouldly;
using Xunit;

namespace Demo
{
    public class Tests
    {
        private readonly ShipmentAggregateRoot _shipmentAggregateRoot;
        private readonly DateTime _arriveShipperDateTime;
        private readonly DateTime _pickupShipperDateTime;
        private readonly DateTime _arriveDestinationDateTime;
        private readonly DateTime _deliveryDestinationDateTime;

        public Tests()
        {
            var pickup = new PickupStop(1);
            var delivery = new DeliveryStop(2);
            _shipmentAggregateRoot = ShipmentAggregateRoot.Factory(pickup, delivery);

            _arriveShipperDateTime = new DateTime(2021, 1, 23, 13, 30, 00);
            _pickupShipperDateTime = new DateTime(2021, 1, 23, 13, 32, 00);
            _arriveDestinationDateTime = new DateTime(2021, 1, 23, 14, 05, 00);
            _deliveryDestinationDateTime = new DateTime(2021, 1, 23, 14, 07, 00);
        }

        [Fact]
        public void CompleteShipment()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);
            _shipmentAggregateRoot.Arrive(2, _arriveDestinationDateTime);
            _shipmentAggregateRoot.Deliver(2, _deliveryDestinationDateTime);
            _shipmentAggregateRoot.IsComplete().ShouldBeTrue();
        }

        [Fact]
        public void CanPickupWithoutArriving()
        {
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);
            var evnts = _shipmentAggregateRoot.GetUncommittedEvents();
            evnts.OfType<Arrived>().Count().ShouldBe(1);
        }

        [Fact]
        public void PickupWithoutArriving()
        {
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);

            var evnts = _shipmentAggregateRoot.GetUncommittedEvents();
            evnts.OfType<Arrived>().Count().ShouldBe(1);
            evnts.OfType<PickedUp>().Count().ShouldBe(1);
        }

        [Fact]
        public void CanDeliverWithoutArriving()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);
            _shipmentAggregateRoot.Deliver(2, _deliveryDestinationDateTime);

            var evnts = _shipmentAggregateRoot.GetUncommittedEvents();
            evnts.OfType<Arrived>().Count().ShouldBe(2);
            evnts.OfType<Delivered>().Count().ShouldBe(1);
        }

        [Fact]
        public void CannotPickupAtDelivery()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(2, _pickupShipperDateTime), "Stop is not a delivery.");
        }

        [Fact]
        public void CannotDeliverAtPickup()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(1, _deliveryDestinationDateTime), "Stop is not a pickup.");
        }

        [Fact]
        public void ArriveStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(0, _arriveShipperDateTime), "Stop does not exist.");
        }

        [Fact]
        public void PickupStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(0, _pickupShipperDateTime), "Stop does not exist.");
        }

        [Fact]
        public void DeliverStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(0, _deliveryDestinationDateTime), "Stop does not exist.");
        }

        [Fact]
        public void ArriveNonDepartedStops()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(2, _arriveDestinationDateTime), "Previous stops have not departed.");
        }

        [Fact]
        public void AlreadyArrived()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime), "Stop has already arrived.");
        }

        [Fact]
        public void AlreadyPickedUp()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime), "Stop has already departed.");
        }

        [Fact]
        public void AlreadyDelivered()
        {
            _shipmentAggregateRoot.Arrive(1, _arriveShipperDateTime);
            _shipmentAggregateRoot.Pickup(1, _pickupShipperDateTime);
            _shipmentAggregateRoot.Arrive(2, _arriveDestinationDateTime);
            _shipmentAggregateRoot.Deliver(2, _deliveryDestinationDateTime);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(2, _deliveryDestinationDateTime), "Stop has already departed.");
        }
    }
}