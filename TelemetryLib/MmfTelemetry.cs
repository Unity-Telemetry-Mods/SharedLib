namespace Sharedlib.Telemetry
{
    internal class MmfTelemetryConfig
    {
        public string Name { get; set; } = "MmfTelemetry";        
    }

    internal class MmfTelemetry<TData> : TelemetryBase<TData, MmfTelemetryConfig>
        where TData : struct
    {


        public MmfTelemetry(MmfTelemetryConfig config) : base(config)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Configure(MmfTelemetryConfig config)
        {

        }


        public override TData Receive()
        {
            return default;
        }



        public override int Send(TData data)
        {
            return 0;
        }

        public override void Dispose()
        {

        }

    }
}
