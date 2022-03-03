using System;

namespace AggregateConsistencyBoundary
{
    public class OrderAggregateRoot
    {
        private readonly Address _restaurant;
        private readonly Address _customer;

        public OrderAggregateRoot(Address restaurant, Address customer)
        {
            _restaurant = restaurant;
            _customer = customer;
        }

        public void Pay()
        {

        }

        public void Cancel()
        {

        }

        public ShipmentAggregateRoot Ship(DateTime expectedPickup, DateTime expectedDelivery)
        {
            var pickup = new PickupStop(1, _restaurant, expectedPickup);
            var delivery = new DeliveryStop(2, _customer, expectedDelivery);
            return ShipmentAggregateRoot.Factory(pickup, delivery);
        }
    }
}