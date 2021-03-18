using System.Collections.Generic;
using Marten.Events.Daemon;

namespace Marten.Events.Aggregation
{
    internal class TenantSliceRange<TDoc, TId>: EventRangeGroup
    {
        public TenantSliceRange(EventRange range, IReadOnlyList<TenantSliceGroup<TDoc, TId>> groups) : base(range)
        {
            Groups = groups;
        }

        public IReadOnlyList<TenantSliceGroup<TDoc, TId>> Groups { get; }


        protected override void reset()
        {
            foreach (var group in Groups) group.Reset();
        }

        public override void Dispose()
        {
            foreach (var group in Groups) group.Dispose();
        }

        public override string ToString()
        {
            return $"Aggregate for {Range}, {Groups.Count} slices";
        }
    }
}
