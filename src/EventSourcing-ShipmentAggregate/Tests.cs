using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace AggregateConsistencyBoundary
{
    public class Tests
    {
        private readonly ShipmentAggregateRoot _shipmentAggregateRoot;

        public Tests()
        {
            _shipmentAggregateRoot = new ShipmentAggregateRoot
            {
                Stops = new List<Stop>
                {
                    new()
                    {
                        StopId = 1,
                        Sequence = 1,
                        Type = StopType.Pickup
                    },
                    new()
                    {
                        StopId = 2,
                        Sequence = 2,
                        Type = StopType.Delivery
                    }
                }
            };
        }

        [Fact]
        public void CompleteShipment()
        {
            _shipmentAggregateRoot.Arrive(1);
            _shipmentAggregateRoot.Pickup(1);
            _shipmentAggregateRoot.Arrive(2);
            _shipmentAggregateRoot.Deliver(2);
            _shipmentAggregateRoot.IsComplete().ShouldBeTrue();
        }

        [Fact]
        public void CannotPickupWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(1), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotDeliverWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(2), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotPickupAtDelivery()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(2), "Stop is not a delivery.");
        }

        [Fact]
        public void CannotDeliverAtPickup()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(1), "Stop is not a pickup.");
        }

        [Fact]
        public void ArriveStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(0), "Stop does not exist.");
        }

        [Fact]
        public void PickupStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(0), "Stop does not exist.");
        }

        [Fact]
        public void DeliverStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(0), "Stop does not exist.");
        }

        [Fact]
        public void ArriveNonDepartedStops()
        {
            _shipmentAggregateRoot.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(2), "Previous stops have not departed.");
        }

        [Fact]
        public void AlreadyArrived()
        {
            _shipmentAggregateRoot.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Arrive(1), "Stop has already arrived.");
        }

        [Fact]
        public void AlreadyPickedUp()
        {
            _shipmentAggregateRoot.Arrive(1);
            _shipmentAggregateRoot.Pickup(1);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Pickup(1), "Stop has already departed.");
        }

        [Fact]
        public void AlreadyDelivered()
        {
            _shipmentAggregateRoot.Arrive(1);
            _shipmentAggregateRoot.Pickup(1);
            _shipmentAggregateRoot.Arrive(2);
            _shipmentAggregateRoot.Deliver(2);
            Should.Throw<InvalidOperationException>(() => _shipmentAggregateRoot.Deliver(2), "Stop has already departed.");
        }
    }
}