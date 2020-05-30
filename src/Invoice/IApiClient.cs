using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ivvy.API
{
    public partial interface IApiClient
    {
        /// <summary>
        /// Returns a specific invoice.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<ResultOrError<Invoice.Invoice>> GetInvoiceAsync(int id);

        /// <summary>
        /// Returns a collection of invoices.
        /// </summary>
        /// <param name="perPage">The per page.</param>
        /// <param name="start">The start.</param>
        /// <param name="filterRequest">The filter request.</param>
        /// <returns></returns>
        Task<ResultOrError<ResultList<Invoice.Invoice>>> GetInvoiceListAsync(
            int perPage, int start, Dictionary<string, object> filterRequest);

        /// <summary>
        /// Adds a payment to an invoice.
        /// </summary>
        /// <param name="invoicePayment">The invoice payment.</param>
        /// <returns>The result object.</returns>
        Task<ResultOrError<ResultObject>> AddInvoicePayment(Invoice.AddPayment invoicePayment);
    }
}