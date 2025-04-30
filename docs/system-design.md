
#  ⚙️System Design

The QR Code Based Metro Ticketing System is built to provide fast and secure metro entry/exit using QR codes, leveraging .NET 8, Redis, and SQL Server.

## 🎨System Architecture

![Alt text](images/system-architecture.svg)

## 📈 ER Diagram

```mermaid
erDiagram
    Admin ||--o{ Station : manages
    Station ||--o{ StationDistance : relates
    Station ||--o{ Trip : entry_exit
    User ||--|| Wallet : owns
    User ||--o{ Ticket : has
    Ticket ||--|| Trip : linked
    Wallet ||--o{ Transaction : records
    User ||--o{ Trip : makes
    Ticket ||--o{ Transaction : pays
    User ||--o{ UserToken : authenticates

    Admin {
        int Id PK
        string Email
        string PasswordHash
        datetime CreatedAt
        datetime LastLoginAt
    }
    Station {
        int Id PK
        string Name
        string Address
        decimal Latitude
        decimal Longitude
        int Order
        string Status
        datetime CreatedAt
        datetime UpdatedAt
    }
    StationDistance {
        int Id PK
        int Station1Id FK
        int Station2Id FK
        decimal Distance
        datetime CreatedAt
        datetime UpdatedAt
    }
    SystemSettings {
        int Id PK
        decimal MinFare
        decimal FarePerKm
        int RapidPassQrCodeValidityMinutes
        int QrTicketValidityMinutes
        int MaxTripDurationMinutes
        decimal TimeLimitPenaltyFee
        datetime CreatedAt
        datetime UpdatedAt
    }
    Ticket {
        int Id PK
        int UserId FK
        enum TicketType
        int OriginStationId FK
        int DestinationStationId FK
        decimal FareAmount
        string QRCodeData
        datetime CreatedAt
        datetime ExpiryTime
        enum TicketStatus
        string TransactionReference
    }
    Trip {
        int Id PK
        int UserId FK
        int TicketId FK
        int EntryStationId FK
        int ExitStationId FK
        datetime EntryTime
        datetime ExitTime
        decimal FareAmount
        enum TripStatus
        string TransactionReference
    }
    Transaction {
        int Id PK
        int WalletId FK
        decimal Amount
        enum TransactionType
        enum PaymentMethod
        enum PaymentItem
        enum TransactionStatus
        string TransactionReference
        string Description
        datetime CreatedAt
        datetime UpdatedAt
    }
    Wallet {
        int Id PK
        int UserId FK
        decimal Balance
        datetime CreatedAt
        datetime UpdatedAt
    }
    User {
        int Id PK
        string FullName
        string Email
        string PhoneNumber
        string NID
        string PasswordHash
        bool IsEmailVerified
        datetime CreatedAt
        datetime UpdatedAt
    }
    UserToken {
        int Id PK
        string Email
        string Token
        enum TokenType
        datetime ExpiryDate
        bool IsUsed
    }

```


### 🧩Database Table Descriptions

| Table           | Description                                                                                     |
|----------------|-------------------------------------------------------------------------------------------------|
| Users          | Stores passenger information including authentication and profile data        |
| Admins         | Stores admin credentials and access logs.                                              |
| Stations       | Stores metro station information like name, address, coordinates, and display order.             |
| StationDistances | Stores distance data between two stations for fare calculation.                               |
| Tickets        | Stores ticket details including journey info, fare, QR code metadata, and ticket status.         |
| Trips          | Stores trip records including entry/exit stations, timings, fare, and trip status.               |
| Wallets        | Stores user wallet balances for fare payment and ticket purchases.                               |
| Transactions   | Stores wallet transaction records like top-ups, fare deductions, and penalties.                  |
| SystemSettings | Stores system-wide settings like fare rates, ticket validity, and penalty fees.                  |



## 🎟️ State Diagram For Ticket

### 1. Purchasing QR Ticket

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> StationSelection: Passenger initiates
    
    state StationSelection {
        [*] --> OriginSelected
        OriginSelected --> DestinationSelected
        DestinationSelected --> FareCalculated
    }
    
    FareCalculated --> PaymentProcessing: Shows fare
    PaymentProcessing --> PaymentApproved: Success
    PaymentProcessing --> PaymentFailed: Declined
    
    state PaymentApproved {
        [*] --> TicketGeneration
        TicketGeneration --> DatabasePersisted
        DatabasePersisted --> RedisCached
        RedisCached --> ReadyForUse
    }
    ReadyForUse --> Active: Passenger views ticket
    Active --> InUse: Scanned at gate
    InUse --> Used: Exit scan completes
    Used --> [*]

    %% Error Handling
    PaymentFailed --> RetryPayment
    RetryPayment --> PaymentProcessing
    PaymentFailed --> Aborted
    
    state SystemErrors {
        DatabasePersisted --> Rollback: SQL failure
        RedisCached --> Regenerate: Cache miss
        Rollback --> RefundInitiated
    }
```
### 2. Rapid Pass QR Ticket

```mermaid
stateDiagram-v2
    [*] --> Idle

    Idle --> RapidPassEligibilityCheck: Passenger requests Rapid Pass
    
    state RapidPassEligibilityCheck {
        [*] --> WalletBalanceVerification
        WalletBalanceVerification --> ActiveRapidPassCheck
        ActiveRapidPassCheck --> CreditLimitApproval: Balance ≥ min_fare
    }
    
    CreditLimitApproval --> RapidPassIssued: Approved
    RapidPassIssued --> Active: QR code generated
    
    state Active {
        [*] --> ReadyForEntry
        ReadyForEntry --> InTransit: Scanned at entry gate
        InTransit --> ExitPending: On train
    }
    
    ExitPending --> JourneyComplete: Scanned at exit
    JourneyComplete --> FareCalculation: Trip data recorded
    
    state FareCalculation {
        [*] --> DistanceCompute
        DistanceCompute --> AutoDeduct: Fare calculated
        AutoDeduct --> [*]
    }
    
    %% Error/Alternative Flows
    RapidPassEligibilityCheck --> Rejected: Low balance/existing pass
    Rejected --> Idle
    
    InTransit --> Timeout: Duration exceeded 120 minutes
    Timeout --> PenaltyApplied
    PenaltyApplied --> FareCalculation
    
```
---
