namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStationRepository StationRepository { get; }
        IStationDistanceRepository StationDistanceRepository { get; }
        ISystemSettingsRepository SettingsRepository { get; }
        IUserRepository UserRepository { get; }
        IUserTokenRepository UserTokenRepository { get; }
        IWalletRepository WalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
