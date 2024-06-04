## Tic-Tac-Toe Back-End

Our .NET 8.0 backend for the Tic-Tac-Toe game provides a robust and powerful foundation for your gaming experience. We have focused on creating an efficient architecture that ensures high performance and reliability for your application. With Quartz Scheduler, we have implemented scheduling and management of game sessions, ensuring precise execution of tasks and optimal utilization of server resources. We have also utilized Server-Sent Events (SSE) requests to deliver data instantly to clients, allowing them to receive updates about the current game state without the need for repeated requests. Our backend is a reliable cornerstone for your gaming platform, delivering smooth and engaging gameplay dynamics.The project utilizes Entity Framework Core 8.0 and is configured to work with a PostgreSQL database. Additionally, all database initialization (migrations) occurs on the server.

[Front-end](https://github.com/kylaran/TicTacToeFrontEnd/)

## Installation

To begin, you'll need to create an 'appsettings.ts' file. located at: TicTakToe/

_Example_
```sh
{
  "ConnectionStrings": {
    "TicTacToeConnection": "Host=yourHost; Port=yourPort; Database=DataBaseName;User ID=user; Password=password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.Hosting.Lifetime": "Error"
    }
  }
}
```

