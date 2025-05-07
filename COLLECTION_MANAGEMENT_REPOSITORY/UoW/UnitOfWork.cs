using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.Repository;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        Task ExecuteWithTransactionAsync(Func<Task> action);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void SetActiveContext(CommonEnum.ContextName contextName);
        Task<int> CompleteAsync();
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        IModuleRepository Modules { get; }
        IMenuRepository Menus { get; }
        IMenuPermissionRepository MenuPermissions { get; }
        
    }

    public class UnitOfWork : IUnitOfWork
    {
        #region Logics
        private readonly am_dbcontext _context;
        private readonly identity_dbcontext _identityContext;
        private DbContext _activeContext;
        private IDbContextTransaction? _currentTransaction;
        private readonly IExecutionStrategy _executionStrategy;

        public UnitOfWork(am_dbcontext context, identity_dbcontext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
            _activeContext = _context;
            _executionStrategy = _context.Database.CreateExecutionStrategy();
            Users = new UserRepository(_identityContext);
            Roles = new RoleRepository(_identityContext);
            UserRoles = new UserRoleRepository(_identityContext);
            Modules = new ModuleRepository(_context);
            Menus = new MenuRepository(_context);
            MenuPermissions = new MenuPermissionRepository(_context);

        }
        public void SetActiveContext(CommonEnum.ContextName contextName)
        {
            _activeContext = contextName == CommonEnum.ContextName.Identity ? _identityContext : _context;
        }


        public async Task<int> CompleteAsync()
        {
            return await _activeContext.SaveChangesAsync();
        }
        public void Dispose() => _context.Dispose();
        #endregion


        #region transaction support
        //public async Task<IDbContextTransaction> BeginTransactionAsync()
        //{
        //    if (_currentTransaction != null)
        //    {
        //        throw new InvalidOperationException("A transaction is already in progress.");
        //    }

        //    _currentTransaction = await _activeContext.Database.BeginTransactionAsync();
        //    return _currentTransaction;
        //}

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                _currentTransaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                _currentTransaction = null;
            }
            else
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                _currentTransaction = null;
            }
        }

        public async Task ExecuteWithTransactionAsync(Func<Task> action)
        {
            // Execute the operation with retry logic
            await _executionStrategy.ExecuteAsync(async () =>
            {
                await BeginTransactionAsync();
                try
                {
                    await action();
                    await CommitTransactionAsync(); // Commit after successful execution
                }
                catch (Exception)
                {
                    await RollbackTransactionAsync(); // Rollback in case of an error
                    throw;
                }
            });
        }
        #endregion


        #region Identity Repos
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IUserRoleRepository UserRoles { get; }
        public IModuleRepository Modules { get; }
        public IMenuRepository Menus { get; }
        public IMenuPermissionRepository MenuPermissions { get; }
        #endregion

        #region Normal repos

        #endregion

    }
}
