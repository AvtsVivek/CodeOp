using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace AggregateConsistencyBoundary.EventSourced
{
    public class Tests
    {
        private readonly ShipmentAggregateRoot _shipmentAggregateRootAggregateRoot;

        public Tests()
        {
            var stops = new List<ShipmentStop>
            {
                new ShipmentStop(1, StopType.Pickup, 1),
                new ShipmentStop(2, StopType.Delivery, 2)
            };
            _shipmentAggregateRootAggregateRoot = new ShipmentAggregateRoot();
            _shipmentAggregateRootAggregateRoot.Dispatch(1, stops);
        }

        [Fact]
        public void CompleteShipment()
        {
            _shipmentAggregateRootAggregateRoot.Arrive(1);
            _shipmentAggregateRootAggregateRoot.Pickup(1);
            _shipmentAggregateRootAggregateRoot.Arrive(2);
            _shipmentAggregateRootAggregateRoot.Deliver(2);
            _shipmentAggregateRootAggregateRoot.IsComplete().ShouldBeTrue();
        }

        [Fact]
        public void CannotPickupWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Pickup(1), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotDeliverWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Deliver(2), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotPickupAtDelivery()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Pickup(2), "Stop is not a delivery.");
        }

        [Fact]
        public void CannotDeliverAtPickup()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Deliver(1), "Stop is not a pickup.");
        }

        [Fact]
        public void ArriveStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Arrive(0), "Stop does not exist.");
        }

        [Fact]
        public void PickupStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Pickup(0), "Stop does not exist.");
        }

        [Fact]
        public void DeliverStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Deliver(0), "Stop does not exist.");
        }

        [Fact]
        public void ArriveNonDepartedStops()
        {
            _shipmentAggregateRootAggregateRoot.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Arrive(2), "Previous stops have not departed.");
        }

        [Fact]
        public void AlreadyArrived()
        {
            _shipmentAggregateRootAggregateRoot.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Arrive(1), "Stop has already arrived.");
        }

        [Fact]
        public void AlreadyPickedUp()
        {
            _shipmentAggregateRootAggregateRoot.Arrive(1);
            _shipmentAggregateRootAggregateRoot.Pickup(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Pickup(1), "Stop has already departed.");
        }

        [Fact]
        public void AlreadyDelivered()
        {
            _shipmentAggregateRootAggregateRoot.Arrive(1);
            _shipmentAggregateRootAggregateRoot.Pickup(1);
            _shipmentAggregateRootAggregateRoot.Arrive(2);
            _shipmentAggregateRootAggregateRoot.Deliver(2);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRootAggregateRoot.Deliver(2), "Stop has already departed.");
        }
    }
}