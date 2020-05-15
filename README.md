# Introduction

This was put together for showing a demo of how not to use ASP.NET Core WebAPI 3.1 and Entity Framework Core 3.1.
* Examples.EFCore.DIY: The starting point for this journey. It shows some bad practices that are common with using these technologies.
  * Deferred execution
  * N+1 queries
  * Client execution
  * State management
  * Migrations
  * Initial database population
  * Swagger
  * Logging and log levels
* We then move on to adding new functionality.
  * Await / async
  * IActionResult / ActionResult&lt;T&gt;
  * CancellationToken
  * Global action filters
  * Transactions
  * AutoMapper
  * Code analysis
  * Health checks
  * Dynamic LINQ
  * Unit tests (with SpecFlow)
* Examples.RFCore.Complete: If you want to peek ahead, this is the end point of the journey. It has the full functionality.