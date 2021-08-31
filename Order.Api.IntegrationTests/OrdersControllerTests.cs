using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Order.Api.IntegrationTests.Framework;
using Order.Api.IntegrationTests.ObjectMothers;
using Order.Api.Requests;
using Order.Domain.Enums;
using Order.Domain.ReadModels;
using Order.EventSubscribers.Subscribers;
using Xunit;

namespace Order.Api.IntegrationTests
{
    public class OrdersControllerTests
    {
        private readonly HttpClient _httpClient;
        private const string url = "/api/Orders/";
        private const int WaitForSubscribe = 3000;

        public OrdersControllerTests()
        {
            var testServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseConfiguration(
                    new ConfigurationBuilder()
                        .AddJsonFile(
                            "appsettings.Testing.json")
                        .Build()
                ));

            _httpClient = testServer.CreateClient();
        }

        [Fact]
        public async Task WHEN_GET_NOT_EXIST_ORDER_SHOULD_RETURN_SUCCESS()
        {
            var response = await _httpClient.GetAsync($"{url}{Guid.Empty.ToString()}");
            var expected = HttpStatusCode.NotFound;
            var actual = response.StatusCode;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task WHEN_CREATE_ORDER_THEN_GET_EXIST_ORDER_SHOULD_RETURN_SUCCESS()
        {
            var expectedCode = HttpStatusCode.OK;
            var request = RequestMothers.CreateOrderRequestMothers.Default();
            var newOrderId = request.OrderId;
            var addResult = await PostAsync<CreateOrderRequest>(request, url, Guid.NewGuid(), Guid.NewGuid());
            var addActual = addResult.StatusCode;
            Assert.Equal(expectedCode, addActual);
            var readModelSubscriber = TestFixture.Instance.GetRequiredService<ReadModelWriter>();

            // start subscriber for readDb insert
            var workerTask = readModelSubscriber.StartAsync(CancellationToken.None);

            //wait subscriber
            Thread.Sleep(WaitForSubscribe);

            var response = await _httpClient.GetAsync($"{url}{newOrderId.ToString()}");
            var actual = response.StatusCode;
            Assert.Equal(expectedCode, actual);
        }

        [Fact]
        public async Task WHEN_CREATE_ORDER_AND_CANCEL_ORDER_THEN_GET_ORDER_SHOULD_RETURN_EXPECTED_STATUS()
        {
            var readModelSubscriber = TestFixture.Instance.GetRequiredService<ReadModelWriter>();

            #region AddOrder

            var expectedAddOrderCode = HttpStatusCode.OK;
            var addRequest = RequestMothers.CreateOrderRequestMothers.Default();
            var newOrderId = addRequest.OrderId;
            var addResult = await PostAsync<CreateOrderRequest>(addRequest, url);
            var addOrderActual = addResult.StatusCode;
            Assert.Equal(expectedAddOrderCode, addOrderActual);
            var workerTaskForCreate = readModelSubscriber.StartAsync(CancellationToken.None);
            Thread.Sleep(WaitForSubscribe);

            #endregion

            #region CancelOrder

            var expectedDeleteOrderCode = HttpStatusCode.OK;
            var deleteRequest = RequestMothers.CancelOrderRequestMothers.Default();

            var deleteResult = await DeleteAsync(deleteRequest, $"{url}{newOrderId}");
            var deleteOrderActual = addResult.StatusCode;
            Assert.Equal(expectedDeleteOrderCode, addOrderActual);
            var workerTaskForCancel = readModelSubscriber.StartAsync(CancellationToken.None);
            Thread.Sleep(WaitForSubscribe);

            #endregion

            var expectedCode = HttpStatusCode.OK;
            var response = await _httpClient.GetAsync($"{url}{newOrderId.ToString()}");
            var actual = response.StatusCode;
            Assert.Equal(expectedCode, actual);

            var expectedOrderStatusId = OrderStatusEnum.Cancelled.Id;
            var actualOrderStatusId = JsonConvert
                .DeserializeObject<OrderReadModel>((await response.Content.ReadAsStringAsync()))?.OrderStatus?.Id;
            Assert.Equal(expectedOrderStatusId, actualOrderStatusId);
        }

        //happy path :) 
        [Fact]
        public async Task WHEN_ORDER_SUCCESSFULLY_DONE_THEN_TRUE_FILTERS_SHOULD_RETURN_ORDER()
        {
            var readModelSubscriber = TestFixture.Instance.GetRequiredService<ReadModelWriter>();
            var expectedProductCount = 5;
            
            #region AddOrder

            var expectedAddOrderCode = HttpStatusCode.OK;
            var addRequest = RequestMothers.CreateOrderRequestMothers.GenerateWithCustomerProductCount(expectedProductCount);
            var newOrderId = addRequest.OrderId;
            var addResult = await PostAsync<CreateOrderRequest>(addRequest, url);
            var addOrderActual = addResult.StatusCode;
            Assert.Equal(expectedAddOrderCode, addOrderActual);
            var workerTaskForCreate = readModelSubscriber.StartAsync(CancellationToken.None);
            Thread.Sleep(WaitForSubscribe);

            #endregion

            #region PaymentApproved

            var expectedPaymentApproved = HttpStatusCode.OK;
            var paymentApprovedRequest = RequestMothers.PaymentApproveRequestMothers.GenerateWithCustomAmount(addRequest.Products.Sum(x => x.UnitPrice * x.Quantity));
            
            var paymentApprovedResult = await PostAsync<PaymentApproveRequest>(paymentApprovedRequest, $"{url}{newOrderId.ToString()}/payments");
            var paymentApprovedActual = paymentApprovedResult.StatusCode;
            Assert.Equal(expectedPaymentApproved, paymentApprovedActual);

            #endregion

            #region ShipmentStarted

            
            var expectedShipmentStarted = HttpStatusCode.OK;
            var shipmentStartedRequest = RequestMothers.ShipmentStartRequestMothers.Default();
            var shipmentStartedResult = await PostAsync<ShipmentStartRequest>(shipmentStartedRequest, $"{url}{newOrderId.ToString()}/shipments");
            var shipmentStartedActual = shipmentStartedResult.StatusCode;
            Assert.Equal(expectedShipmentStarted, shipmentStartedActual);
            
            var workerTaskForShipmentStarted = readModelSubscriber.StartAsync(CancellationToken.None);
            Thread.Sleep(WaitForSubscribe);
            #endregion

            #region ShipmentFinished

            var expectedShipmentFinished = HttpStatusCode.OK;
            var shipmentFinishedRequest = RequestMothers.ShipmentFinishRequestMothers.Default();
            var shipmentFinishedResult = await PutAsync<ShipmentFinishRequest>(shipmentFinishedRequest, $"{url}{newOrderId.ToString()}/shipments");
            var shipmentFinishedActual = shipmentStartedResult.StatusCode;
            Assert.Equal(expectedShipmentFinished, shipmentFinishedActual);
            var workerTaskForShipmentFinished = readModelSubscriber.StartAsync(CancellationToken.None);
            Thread.Sleep(WaitForSubscribe);
            
            #endregion
            
            var expectedOrderCode = HttpStatusCode.OK;
            var response = await _httpClient.GetAsync($"{url}{newOrderId.ToString()}?CourierAssignedFilter=true");
            var actual = response.StatusCode;
            Assert.Equal(expectedOrderCode, actual);

            var expectedOrderStatusId = OrderStatusEnum.Cancelled.Id;
            var order = JsonConvert.DeserializeObject<OrderReadModel>((await response.Content.ReadAsStringAsync()));
            
            Assert.Equal(addRequest.OrderId, order.OrderId);
            Assert.Equal(addRequest.Products.Count, order.Products.Count);
            Assert.Equal(addRequest.CustomerId, order.CustomerId);
            Assert.Equal(addRequest.CustomerFullName, order.CustomerName);
            Assert.Equal(addRequest.DontRingBell, order.DontRingBell);
            Assert.Equal(addRequest.AddressDetail, order.AddressDetail);
            
            Assert.Equal(paymentApprovedRequest.PaymentId, order.PaymentTransactionId);

            Assert.Equal(shipmentStartedRequest.CourierId, order.CourierId);
            Assert.Equal(shipmentStartedRequest.CourierName, order.CourierName);
            
            Assert.True(order.IsPaid);
            Assert.True(order.IsShipmentFinished);
            Assert.Equal(OrderStatusEnum.Completed.Id, order.OrderStatus.Id);
            Assert.Equal(OrderStatusEnum.Completed.Name, order.OrderStatus.Description);
        }

        #region PostAsync

        public async Task<HttpResponseMessage> PostAsync<TRequest>(TRequest request, string postUrl,
            Guid? correlationId = null,
            Guid? transactionId = null)
        {
            _httpClient.DefaultRequestHeaders.Remove("correlationId");
            _httpClient.DefaultRequestHeaders.Remove("transactionId");
            _httpClient.DefaultRequestHeaders.Add("correlationId", (correlationId ?? Guid.NewGuid()).ToString());
            _httpClient.DefaultRequestHeaders.Add("transactionId", (transactionId ?? Guid.NewGuid()).ToString());
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(postUrl, content);

            return response;
        }

        #endregion

        #region PutAsync

        public async Task<HttpResponseMessage> PutAsync<TRequest>(TRequest request, string putUrl,
            Guid? correlationId = null,
            Guid? transactionId = null)
        {
            _httpClient.DefaultRequestHeaders.Remove("correlationId");
            _httpClient.DefaultRequestHeaders.Remove("transactionId");
            _httpClient.DefaultRequestHeaders.Add("correlationId", (correlationId ?? Guid.NewGuid()).ToString());
            _httpClient.DefaultRequestHeaders.Add("transactionId", (transactionId ?? Guid.NewGuid()).ToString());
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(putUrl, content);

            return response;
        }

        #endregion

        #region DeleteAsync

        public async Task<HttpResponseMessage> DeleteAsync<TRequest>(TRequest request, string deleteUrl,
            Guid? correlationId = null,
            Guid? transactionId = null)
        {
            var deleteRequest = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(deleteUrl, UriKind.Relative)
            };
            deleteRequest.Headers.Add("correlationId", (correlationId ?? Guid.NewGuid()).ToString());
            deleteRequest.Headers.Add("transactionId", (transactionId ?? Guid.NewGuid()).ToString());
            return await _httpClient.SendAsync(deleteRequest);
        }

        #endregion
    }
}