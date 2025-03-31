# equipment-status
A service to track current and historical statuses of pieces of equipment and machinery

It has an API with 4 lovely endpoints, and a Swagger UI for ease of use.

It also has an in-memory database via EF Core, with some seeded data
to give you an impression of what can be done via the API.

The endpoints are:

`GET /equipment`
Gets the latest equipment states

`POST /equipment`
Adds a new equipment state record,
with one of the three valid states;
"**Stopped**", "**Transitioning**" or "**Running**"

`GET /equipment/history/from/{from}/to/{to}`
Gets all equipment states within the given time period

`GET /equipment/{id}/history/from/{from}/to/{to}`
Gets all equipment states for a given equipment ID within the given time period

Enjoy!
