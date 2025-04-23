namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAdminRepository AdminRepository { get; }
        IStationRepository StationRepository { get; }
        IStationDistanceRepository StationDistanceRepository { get; }
        ISystemSettingsRepository SystemSettingsRepository { get; }
        IUserRepository UserRepository { get; }
        IUserTokenRepository UserTokenRepository { get; }
        IWalletRepository WalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        ITicketRepository TicketRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
