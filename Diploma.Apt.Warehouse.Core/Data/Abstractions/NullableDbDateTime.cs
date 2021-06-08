using System;
using EShop.Data.Helpers;

namespace Diploma.Apt.Warehouse.Core.Data.Abstractions
{
    public class NullableDbDateTime
    {
        private DateTime? _value;
        
        public DateTime? Value
        {
            get => _value.EnsureUniversalTime();
            set => _value = value.EnsureUniversalTime();
        }
        
        public NullableDbDateTime(){}

        public NullableDbDateTime(DateTime? value)
        {
            Value = value;
        }
    }
}