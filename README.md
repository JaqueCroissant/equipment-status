# equipment-status
A service to track current and historical statuses of pieces of equipment and machinery

It has an API with 4 lovely endpoints, and a Swagger UI for ease of use.

It also has an in-memory database via EF Core, with some seeded data
to give you an impression of what can be done via the API.

While generally forgiving in the formats of various inputs,
one must adhere strictly to the "State" parameter, as it only has 3 valid inputs;
"Stopped", "Transitioning" and "Running"

Enjoy!
