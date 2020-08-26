# The Restaurant Kata

A Kata for building and learning about distributed systems, inspired by [Greg Young](https://twitter.com/gregyoung)s restaurant example.

Imagine the following: you are entering a restaurant to order some food and drinks.
You pick up the menu from the table, study it and after you found your meal you contact the waiter to order a Wiener Schnitzel and a soda.
After a while the assistant manager brings you the food, that was deliciously prepared by the cook.
Once you are finished you ask for the bill, pay in cash and leave this nice place.

In this setting we have a user, the customer, who is interacting with a system.
This system consists of a lot of different actors, who are working together to fulfill the users needs.
Each actor executes several tasks while interacting with other actors.

## Actors within our system

TODO: grafik

### The Customer - representing a system user

The customer is hungry and thirsty and visits the restaurant for food and drinks.
It represents the outside user interacting with the system:

- requests the menu containing products with nutrition information
- issues orders with the waiter for the selected products
- receives food
- requests the bill and pays

### Manager - creating the guest experience with a menu

The manager designs the menu, which contains all the items served by the restaurant.

- the menu with all of the items is presented to a customer. A item of the menu has a name, a nutrition information and a price.
- to ensure the nutrition information is up-to-date with the ingredients served, details about them have to be retrieved from the cook.
- the cashier asks for item prices

### Waiter - serving the customer at the table

A waiter interacts with the customer, who orders items from the menu.

- take orders for several menu items
- forward the food order to the cook and respond to the customer with an estimated waiting time
- forwards drinks to the assistant manager

### Cook - preparing a delicious meal

The cook prepares meals in the order they appear.

- order for single menu items can be placed
- estimated time for preparation is returned (calculated by the number of ingredients)
- when a meal is prepared it is placed on the counter and the assistant manager is notified

### Assistant Manager - delivering items

The assistant manager ensures that all ordered items are delivered on time and that they are registered for billing.

- watches for prepared meals from the cook and picks them up
- prepares drinks
- delivers items to the customer
- registers delivered items with the cashier

### Cashier - managing payment and money

The cashier keeps track of items a customer ordered and generates a bill.

- knows food prices from the managers menu
- keeps track of delivered items for a customer
- generates bill for a customer
- marks bill as payed

