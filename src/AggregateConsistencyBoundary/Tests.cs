using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace AggregateConsistencyBoundary
{
    public class Tests
    {
        private readonly Shipment _shipment;

        public Tests()
        {
            var stops = new List<Stop>
            {
                new PickupStop
                {
                    StopId = 1,
                    Sequence = 1,
                },
                new DeliveryStop
                {
                    StopId = 2,
                    Sequence = 2,
                }
            };
            _shipment = new Shipment(stops);
        }

        [Fact]
        public void CompleteShipment()
        {
            _shipment.Arrive(1);
            _shipment.Pickup(1);
            _shipment.Arrive(2);
            _shipment.Deliver(2);
            _shipment.IsComplete().ShouldBeTrue();
        }

        [Fact]
        public void CannotPickupWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Pickup(1), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotDeliverWithoutArriving()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Deliver(2), "Stop hasn't arrived yet.");
        }

        [Fact]
        public void CannotPickupAtDelivery()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Pickup(2), "Stop is not a delivery.");
        }

        [Fact]
        public void CannotDeliverAtPickup()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Deliver(1), "Stop is not a pickup.");
        }

        [Fact]
        public void ArriveStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Arrive(0), "Stop does not exist.");
        }

        [Fact]
        public void PickupStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Pickup(0), "Stop does not exist.");
        }

        [Fact]
        public void DeliverStopDoesNotExist()
        {
            Should.Throw<InvalidOperationException>(() => _shipment.Deliver(0), "Stop does not exist.");
        }

        [Fact]
        public void ArriveNonDepartedStops()
        {
            _shipment.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipment.Arrive(2), "Previous stops have not departed.");
        }

        [Fact]
        public void AlreadyArrived()
        {
            _shipment.Arrive(1);
            Should.Throw<InvalidOperationException>(() => _shipment.Arrive(1), "Stop has already arrived.");
        }

        [Fact]
        public void AlreadyPickedUp()
        {
            _shipment.Arrive(1);
            _shipment.Pickup(1);
            Should.Throw<InvalidOperationException>(() => _shipment.Pickup(1), "Stop has already departed.");
        }

        [Fact]
        public void AlreadyDelivered()
        {
            _shipment.Arrive(1);
            _shipment.Pickup(1);
            _shipment.Arrive(2);
            _shipment.Deliver(2);
            Should.Throw<InvalidOperationException>(() => _shipment.Deliver(2), "Stop has already departed.");
        }
    }
}