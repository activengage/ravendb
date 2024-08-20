namespace Raven35.Abstractions.Smuggler.Data
{
    public class CounterOperationState
    {
        public long LastWrittenEtag { get; set; }

        public string CounterId { get; set; }
    }
}
