using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TicTakToe.Entity.Models.Vm.SSE
{
    public class SSEResponseMethodVm
    {
        public SSEStatusEnum Status { get; set; }
        public object? Data { get; set; }
        public bool NeedClose { get; set; }
        public bool Seen { get; set; }
    }
}
