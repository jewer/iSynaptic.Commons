﻿using System;

namespace iSynaptic.Commons.Data.Syntax
{
    public interface IPredicateScopeToBinding<TMetadata, TSubject> : IScopeToBinding<TMetadata, TSubject>
    {
        IScopeToBinding<TMetadata, TSubject> When(Func<IMetadataRequest<TMetadata, TSubject>, bool> predicate);
    }
}