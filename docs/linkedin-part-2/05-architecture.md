```mermaid
flowchart LR
    U[User / Operations Team] --> UI[ASP.NET Core MVC<br/>Razor UI]

    UI --> C[Controllers]

    C --> Q[QuoteToWorkOrderService]
    C --> W[WorkOrderWorkflowService]
    C --> I[InventoryStockService]

    Q --> DB[(EF Core / SQLite)]
    W --> DB
    I --> DB

    W --> H[Status Timeline<br/>Audit Trail]
    I --> T[Inventory Transactions<br/>Usage / Reversal / Correction]

    DB --> D[Operational Dashboard]
    DB --> P[Quote & Work Order PDFs]

    G[Git Push] --> CI[GitHub Actions]
    CI --> TEST[Automated Tests]
    TEST --> DOCKER[Docker Build]
    DOCKER --> HEALTH[/Health Check/]