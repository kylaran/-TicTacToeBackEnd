using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TicTakToe.Entity.Models.Vm.SSE
{
    public class SSEResponseVm
    {
        public SSEStatusEnum status { get; set; }
        public object? data { get; set; }
        public SSEResponseVm() { }
        public SSEResponseVm(SSEResponseMethodVm obj)
        {
            status = obj.Status;
            data = obj.Data;
        }
    }
}
