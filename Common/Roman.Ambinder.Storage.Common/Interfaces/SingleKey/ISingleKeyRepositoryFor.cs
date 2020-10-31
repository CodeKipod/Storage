﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Roman.Ambinder.DataTypes.OperationResults;
using Roman.Ambinder.Storage.Common.Interfaces.RespositoryOperations;
using Roman.Ambinder.Storage.Common.Interfaces.SingleKey.RespositoryOperations;

namespace Roman.Ambinder.Storage.Common.Interfaces.SingleKey
{
    public interface ISingleKeyRepositoryFor<TKey, TEntity> :
        ISingleKeyRepositoryGetOperationsFor<TKey, TEntity>,
        IRepositoryAddOperationsFor<TKey, TEntity>,
        ISingleKeyRepositoryUpdateOperationsFor<TKey, TEntity>,
        ISingleKeyRepositoryRemoveOperationsFor<TKey, TEntity>
        where TEntity : class, new()
    {
        Task<OperationResultOf<TEntity>> TryAddOrUpdateAsync(TKey key,
            Action<TEntity> updateOrInitAction,
            CancellationToken cancellationToken = default);
    }
}