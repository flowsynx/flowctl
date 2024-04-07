namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadRequest
{
    public required string Path { get; set; }
    public bool? Hashing { get; set; } = false;
}