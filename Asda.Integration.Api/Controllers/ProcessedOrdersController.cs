using Microsoft.AspNetCore.Mvc;
using System;
using Asda.Integration.Domain.Models;
using Asda.Integration.Domain.Models.PostSale;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Api.Controllers;

public class ProcessedOrdersController : ControllerBase
{
    private readonly IUserConfigAdapter _userConfigAdapter;

    public ProcessedOrdersController(IUserConfigAdapter userConfigAdapter)
    {
        this._userConfigAdapter = userConfigAdapter;
    }

    /// <summary>
    /// This call is made when creating an RMA or refund request for orders from this channel.
    /// It allows you to determine what kinds of refunds and returns are supported on the channel,
    /// so that the UI is presented accordingly.
    /// </summary>
    /// <param name="request"><see cref="BaseRequest"/></param>
    /// <returns><see cref="PostSaleOptionsResponse"/></returns>
    [HttpPost]
    public PostSaleOptionsResponse PostSaleOptions([FromBody] BaseRequest request)
    {
        try
        {
            var user = this._userConfigAdapter.Load(request.AuthorizationToken);

            var response = new PostSaleOptionsResponse();

            // Add the correct values to the response here.

            return response;
        }
        catch (Exception ex)
        {
            return new PostSaleOptionsResponse { Error = ex.Message };
        }
    }
}