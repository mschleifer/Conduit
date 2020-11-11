[![RealWorld Frontend](https://img.shields.io/badge/realworld-frontend-%23783578.svg)](http://realworld.io)

# ![Blazor Example App](blazor-logo.png)

> ### .NET Blazor codebase containing real world examples (CRUD, auth, etc.) that adheres to the [RealWorld](https://github.com/gothinkster/realworld-example-apps) spec and API.

This codebase was created to demonstrate a fully fledged application built with Blazor that interacts with an actual backend server including CRUD operations, authentication, routing, pagination, and more.

# How it works

TBD


### Making requests to the backend API

There is a live API server running at https://conduit.productionready.io/api for the application to make requests against. You can view [the API spec here](https://github.com/GoThinkster/productionready/blob/master/api) which contains all routes & responses for the server.

The source code for the backend server (available for Node, Rails and Django) can be found in the [main RealWorld repo](https://github.com/gothinkster/realworld).

If you want to change the API URL to a local server, you can run one of those backend servers and in the Blazor project change the `client.BaseAddress` in Program.cs to the local server's URL (i.e. `localhost:3000/api`).


# Getting started

Until the release of .NET 5, you'll need .NET 5 RC2 SDK and optionally the latest preview of Visual Studio or the C# extension for Visual Studio Code. See "Get Started" here for details - https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-5-release-candidate-2/#get-started.

Once you have the required SDK and development tools you should be able to clone the repo, build the Conduit project, and run the app via IIS Express.


## Functionality overview

The example application is a social blogging site (i.e. a Medium.com clone) called "Conduit". It uses a custom API for all requests, including authentication. You can view a live demo over at https://demo.realworld.io

**General functionality:**

- Authenticate users via JWT (login/signup pages + logout button on settings page)
- CRU* users (sign up & settings page - no deleting required)
- CRUD Articles
- CR*D Comments on articles (no updating required)
- GET and display paginated lists of articles
- Favorite articles
- Follow other users
