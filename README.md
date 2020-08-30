# The Restaurant Kata

A Kata for building and learning about distributed systems, inspired by [Greg Young](https://twitter.com/gregyoung)s restaurant example.

Imagine the following: you are entering a restaurant to order some food and drinks.
You pick up the menu from the table, study it and after you found your meal you contact the waitress to order a Wiener Schnitzel and a soda.
After a while the assistant manager brings you the food, which was deliciously prepared by the cook.
Once you are finished you ask for the bill, pay in cash and leave this nice place.

In this setting we have a user, the guest, who is interacting with a system.
This system consists of a lot of different actors, who are working together to fulfill the users needs.
Each actor executes several tasks within their job, while they interact with other actors.

## Actors within our system

TODO: grafik

### The guest - representing a system user

The customer - the guest - is hungry and thirsty and visits the restaurant for food and drinks.
Guests represent the outside users who are interacting with the system:

- request the menu, containing products with nutrition information
- issues orders with the waiter for the selected products from the menu
- receive food and drinks
- requests the bill and pays

### Guest experience - creating a perfect guest experience

A manager designs the menu, which describes all the items served by the restaurant.

- the menu with all of the items is presented to a customer. A item of the menu has a name, a nutrition information and a price.
- to ensure the nutrition information is up-to-date with the ingredients served, details about them have to be retrieved from the cook.
- the cashier asks for item prices

### Table service - helping the guest at the table

A waitress/waiter interacts with the guest, who orders items from the menu.

- takes the order for several menu items from guests
- forwards the food order to food preparation and responds to the customer with an estimated waiting time
- forwards drink orders to delivery

### Food preparation - cooking a delicious meal

A cook prepares meals in the order they are requested.

- order for single menu items can be placed
- estimated time for preparation is returned (calculated by the number of ingredients)
- when a meal is prepared it is placed on the counter and delivery is notified
- provides a list of cookable meals with name and nutrition information

### Delivery - serving items

In delivery an assistant manager ensures that all ordered items are delivered on time to the guest and that they are registered for billing.

- watches for prepared meals from the cook and picks them up
- prepares drinks
- delivers items to the guest
- registers delivered items with the cashier

### Billing - managing payment and money

The cashier keeps track of items a guest ordered and generates a bill.

- knows food prices from the managers menu
- keeps track of delivered items for a customer
- generates a bill for the guest
- marks bill as payed

## Part 1 - Building the system in a naive way

The goal of the first exercise is to build each actor as a standalone application and connect them together to form a system.
We focus on bringing value to the guest, while learning about the behavior of the system.

To keep things simple from a technical point of view, the following guidelines should be respected:

- each actor is implemented as independent runnable application, and it can be started with a simple CLI command
- configuration parameters of an application can be set/overridden via environment variables
- all communication is done via unsecured HTTP and the exposed port can be configured via a dedicated environment variable (e.g. `PORT` or `ASPNETCORE_URLS`)
- logs messages are written to the console / STDOUT

### Starting points

The guest is simulated and therefor the following contracts should be implemented by the following services

#### Customer

- Service
  Offers interaction with the guest
- Contract
  Expects food & drinks to be delivered
- Endpoint
  configurable via the `CUSTOMER_ENDPOINT` variable
- Messages
  HTTP-POST with JSON body to `GUEST_ENDPOINT/guest/<<guest id>>/order/<<order id>>`, expecting the following request body and no response body.
  
  ```json
  {
      "foodIDs" : [
            <<food identifier>>,
            <<food identifier>>
      ],
      "drinkIDs": [
            <<drink identifier>>,
            <<drink identifier>>
      ]
  }
  ```

  Example requests with HTTP-POST to `GUEST_ENDPOINT/guest/1/order/1`
  
  ```json
  {
      "foodIDs" : [],
      "drinkIDs": [ 1, 2, 2 ]
  }
  ```

  After a while we have another request:

  ```json
  {
      "foodIDs" : [
            1,
            1
      ],
      "drinkIDs": []
  }
  ```
  
  After a while we have another request:

  ```json
  {
      "foodIDs" : [ 2 ],
      "drinkIDs": []
  }
  ```
  
- Policies
  Expected SLA of < 1 second
- Consumers
  Guest

#### Guest experience

- Service
  Creates a menu with daily offers for customers which is compliant to legal rules.
- Contract
  - Returns a guest identifier together with a menu structure containing the menu-ids together with nutrition and price information
- Endpoint
  configurable via the `GUEST_ENDPOINT` variable
- Messages
  HTTP-GET with empty body to `GUEST_ENDPOINT/menu`, expecting the following JSON payload structure in the body as response:
  
  ```json
  {
      "guestID": <<guest identifier>>,
      "food" : [
            {
                "foodId": <<food identifier>>,
                "name": <<food item name>>,
                "nutrition": [ <<nutrition numbers>>],
                "price": <<price>>
            }
      ],
      "drink": [
            {
                "drinkId": <<drink identifier>>,
                "name": <<dink name>>,
                "nutrition": [ <<nutrition numbers>>],
                "price": <<price>>
            }
      ]
  }
  ```

  Example request:
  
  ```json
  {
      "guestID": 2,
      "food" : [
            {
                "foodId": 1,
                "name": "Burger",
                "nutrition": [ "10a", "1", "8a"],
                "price": 20.2
            },
            {
                "foodId": 2,
                "name": "Wiener Schnitzel",
                "nutrition": [ "10a", "3"],
                "price": 10.2
            }
      ],
      "drink": [
            {
                "drinkId": 1,
                "name": "Soda",
                "nutrition": [ "11a"],
                "price": 4.4
            },
            {
                "drinkId": 2,
                "name": "Beer",
                "nutrition": [],
                "price": 5.4
            }
      ]
  }
  ```
  
- Policies
  Expected SLA of < 1 second
- Consumers
  Guest

#### Table service

- Service
  
- Contract
  Issues an order with the waiter with the guest id and a list of food and drink items, while awaiting an estimated waiting time.
- Endpoint
  configurable via the `TABLE_ENDPOINT` variable
- Messages
  HTTP-POST to `TABLE_ENDPOINT/orders` with the following JSON payload structure in the body:
  
  ```json
  {
      "guestID": <<guest identifier>>,
      "food" : [
            <<food identifier>>,
            <<food identifier>>
      ],
      "drink": [
            <<drink identifier>>,
            <<drink identifier>>
      ]
  }
  ```

  Expected response with the following JSON payload structure in the body:

  ```json
  {
      "orderID": <<order identifier>>,
      "waitingTime" : <<time in minutes>>
  }
  ```

  Example request:

  HTTP-POST to `TABLE_ENDPOINT/orders`

  ```json
  {
      "guestID": 2,
      "food" : [
            1,
            2,
            1
      ],
      "drink": [
            1,
            2,
            2
      ]
  }
  ```

  Example response:

  ```json
  {
      "orderID": 1,
      "waitingTime": 20.0
  }
  ```
  
- Policies
  Expected SLA of < 2 second
- Consumers
  Guest

#### Billing

- Contract
  Returns current bills for customer and offers them a possibility to pay bills with different payment methods
- Endpoint
  configurable via the `BILLING_ENDPOINT` variable
- Messages
  
  - Retrieve bill for guest
    HTTP-GET to `BILLING_ENDPOINT/billing/<<guest id>>` with an empty body will return the following JSON encoded body:
  
    ```json
    {
        "billID": <<guest identifier>>,
        "orderIDs": [
            <<order identifier>>
        ],
        "totalSum": <<sum in euro>>
    }
    ```

    Example request:

    HTTP-GET to `BILLING_ENDPOINT/billing/1` with response:

    ```json
    {
        "billID": 1,
        "orderIDs": [
            1
        ],
        "totalSum": 65.8
    }
    ```

  - Pay bill
    HTTP-POST to `BILLING_ENDPOINT/payment/<<bill id>>` with the following JSON encoded body:

    ```json
    {
        "amount": <<amount in euro>>,
        "paymentMethod": <<method identifier>>
    }
    ```

    Example request:

    HTTP-POST to `BILLING_ENDPOINT/payment/1`

    ```json
    {
        "amount": 69.0,
        "paymentMethod": "cash"
    }
    ```


- Policies
  Expected SLA of < 1 second
- Consumers
  Guest