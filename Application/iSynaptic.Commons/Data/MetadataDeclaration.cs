﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSynaptic.Commons.Data
{
    public class MetadataDeclaration<TMetadata> : IMetadataDeclaration<TMetadata>
    {
        private Maybe<TMetadata> _Default = Maybe<TMetadata>.NoValue;

        public MetadataDeclaration()
        {
            MetadataType = typeof(TMetadata);
        }

        public MetadataDeclaration(TMetadata @default) : this()
        {
            _Default = new Maybe<TMetadata>(@default);
        }

        protected virtual TMetadata GetDefault()
        {
            if (_Default.HasValue)
                return _Default.Value;

            return default(TMetadata);
        }

        public void CheckValue(TMetadata value)
        {
            OnCheckValue(value, "value");
        }

        protected virtual void OnCheckValue(TMetadata value, string valueName)
        {
        }

        public TMetadata Default
        {
            get
            {
                TMetadata defaultValue = GetDefault();

                try
                {
                    OnCheckValue(defaultValue, "default");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The default value defined was not valid. See the inner exception for details.", ex);
                }

                return defaultValue;
            }
        }

        public Type MetadataType { get; private set; }
    }
}
