using System;
using System.Collections.Generic;
using Asda.Integration.Api.Mappers;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Domain.Models.Order;
using Asda.Integration.Domain.Models.User;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;


namespace AsdaTests.OrderController
{
    public class DispatchActionShould
    {
        private readonly ITestOutputHelper _output;

        private const string AuthToken = "9d9213520aa041b082d5cb0f2c2c3684";

        public DispatchActionShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReturnProperErrorMessageWhenOrdersAreNull()
        {
            //Arrange
            var mock = new Mock<ILogger<Asda.Integration.Api.Controllers.OrderController>>();
            var logger = mock.Object;

            var controller = new Asda.Integration.Api.Controllers.OrderController(null, null, logger);

            //Act
            var sut = controller.Dispatch(new OrderDespatchRequest()
            {
                AuthorizationToken = AuthToken,
                Orders = null
            });

            //Assert
            Assert.Null(sut.Orders);
            Assert.NotNull(sut.Error);
            Assert.Contains("Orders are Null", sut.Error);
        }

        [Fact]
        public void ReturnProperErrorMessageWhenAuthTokenIsNotCorrect()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<Asda.Integration.Api.Controllers.OrderController>>();
            var mockUserAdapter = new Mock<IUserConfigAdapter>();
            mockUserAdapter.Setup(x => x.Load(AuthToken)).Returns((UserConfig) null);

            var controller =
                new Asda.Integration.Api.Controllers.OrderController(mockUserAdapter.Object, null, mockLogger.Object);

            //Act
            var sut = controller.Dispatch(new OrderDespatchRequest()
            {
                AuthorizationToken = AuthToken,
                Orders = new List<OrderDespatch>
                {
                    new OrderDespatch()
                }
            });

            //Assert
            Assert.Null(sut.Orders);
            Assert.NotNull(sut.Error);
            Assert.Contains($"User with AuthToken: {AuthToken} - not found", sut.Error);
        }

        [Fact]
        public void ReturnProperErrorMessageWhenThereIsAnErrorInOrderService()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<Asda.Integration.Api.Controllers.OrderController>>();

            var mockUserAdapter = new Mock<IUserConfigAdapter>();
            mockUserAdapter
                .Setup(x => x.Load(AuthToken))
                .Returns(new UserConfig());

            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(x => x.SendDispatchFiles(It.IsAny<List<ShipmentConfirmation>>()))
                .Throws(new Exception("Error in Order Service"));


            var controller = new Asda.Integration.Api.Controllers.OrderController(mockUserAdapter.Object,
                mockOrderService.Object, mockLogger.Object);

            //Act
            var sut = controller.Dispatch(new OrderDespatchRequest()
            {
                AuthorizationToken = AuthToken,
                Orders = new List<OrderDespatch>
                {
                    new OrderDespatch
                    {
                        Items = new[] {new OrderDespatchItem()}
                    }
                }
            });

            //Assert
            Assert.NotNull(sut.Orders);
            Assert.Contains("Error in Order Service ", sut.Error);
            Assert.Equal(1, sut.Orders.Count);
        }
    }
}