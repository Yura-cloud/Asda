using System.Collections.Generic;
using System;

namespace LinnworksAPI
{ 
    public interface IEmailController
	{
		GenerateAdhocEmailResponse GenerateAdhocEmail(GenerateAdhocEmailRequest request);
		GenerateFreeTextEmailResponse GenerateFreeTextEmail(GenerateFreeTextEmailRequest request);
		EmailTemplate GetEmailTemplate(Int32 pkEmailTemplateRowId);
		List<EmailTemplateHeader> GetEmailTemplates();
	} 
}