using System;

namespace Prometheus
{
    public sealed class Counter : Collector<Counter.Child>, ICounter
    {
        public sealed class Child : ChildBase, ICounter
        {
            internal Child(Collector parent, Labels labels, bool publish)
                : base(parent, labels, publish)
            {
                _identifier = CreateIdentifier();
            }

            private readonly byte[] _identifier;

            private ThreadSafeDouble _value;

            private protected override void CollectAndSerializeImpl(IMetricsSerializer serializer)
            {
                if (this._timestamp == 0)
                {
                    serializer.WriteMetric(_identifier, Value);
                }
                else
                {
                    serializer.WriteMetric(_identifier, Value, this._timestamp);
                }
            }

            public void Inc(double increment = 1.0)
            {
                if (increment < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(increment), "Counter value cannot decrease.");

                _value.Add(increment);
                Publish();
            }

            public double Value => _value.Value;
        }

        private protected override Child NewChild(Labels labels, bool publish)
        {
            return new Child(this, labels, publish);
        }

        internal Counter(string name, string help, string[] labelNames, bool suppressInitialValue)
            : base(name, help, labelNames, suppressInitialValue)
        {
        }

        public void Inc(double increment = 1) => Unlabelled.Inc(increment);
        public double Value => Unlabelled.Value;

        public void Publish() => Unlabelled.Publish();

        private protected override MetricType Type => MetricType.Counter;
    }
}