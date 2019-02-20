using System.Threading.Tasks;

namespace CPR.Core.Interfaces
{
    public interface IClaimPaymentReport
    {
        Task Generator(string sourceFileName, string targetFileName);
    }
}