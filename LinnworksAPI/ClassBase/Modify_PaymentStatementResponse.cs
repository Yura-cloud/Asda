using System.Collections.Generic;

namespace LinnworksAPI
{ 
    public class Modify_PaymentStatementResponse
	{
        /// <summary>
        /// List of modified items, added or updated. Each item has Id which was provided in the request 
        /// </summary>
		public List<ModifiedPaymentStatementItem> ModifiedItems { get; set; }
	} 
}