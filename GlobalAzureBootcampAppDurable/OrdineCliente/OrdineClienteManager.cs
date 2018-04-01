using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GlobalAzureBootcampAppDurable.OrdineCliente.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Serilog;

namespace GlobalAzureBootcampAppDurable.OrdineCliente
{
    public static class OrdineClienteManager
    {
        [FunctionName("OrdineClienteManager")]
        [return: Table("OrdiniCliente")]
        public static async Task<OrdiniAcquistoTable> Run(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            OrdiniAcquisto ordineAcquisto = context.GetInput<OrdiniAcquisto>();
            ordineAcquisto.IdOrdine = Guid.NewGuid().ToString("N");

            // TODO: Salva l'ordine in un DB.
            string mailInstance;
            string smsInstance = "";
            try
            {
               
                //smsInstance = await context.CallActivityAsync<string>(Workflow.NotificaSmsOrdineCliente, ordineAcquisto);
                //Log.Information($"OrdineClienteManager: SmsInstance {smsInstance}");
                mailInstance = await context.CallActivityAsync<string>(Workflow.InviaMailOrdineCliente, ordineAcquisto);
                Log.Information($"OrdineClienteManager: MailInstance {mailInstance}");
            }
            catch (Exception ex)
            {
                throw;
            }

            return new OrdiniAcquistoTable
            {
                PartitionKey = ordineAcquisto.IdOrdine,
                RowKey = $"{smsInstance}-{mailInstance}",
                Ordine = ordineAcquisto,
                NotificaSmsOrdineCliente = smsInstance,
                InviaMailOrdineCliente = mailInstance
            };
        }
    }
}
