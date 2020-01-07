using System;
using System.Web;
using System.Web.Mvc;
using Vendr.Core.Models;
using Vendr.Core.Web.Api;

namespace Vendr.Core.Web.PaymentProviders
{
    [PaymentProvider("invoicing", "Invoicing", "Basic payment provider for payments that will be processed via an external invoicing system", Icon = "icon-invoice")]
    public class InvoicingPaymentProvider : PaymentProviderBase<InvoicingSettings>
    {
        public InvoicingPaymentProvider(VendrContext vendr)
            : base(vendr)
        { }

        public override bool CanCancelPayments => true;
        public override bool CanCapturePayments => true;
        public override bool FinalizeAtContinueUrl => true;

        public override PaymentForm GenerateForm(OrderReadOnly order, string continueUrl, string cancelUrl, string callbackUrl, InvoicingSettings settings)
        {
            return new PaymentForm(continueUrl, FormMethod.Post);
        }

        public override string GetCancelUrl(OrderReadOnly order, InvoicingSettings settings)
        {
            return string.Empty;
        }

        public override string GetErrorUrl(OrderReadOnly order, InvoicingSettings settings)
        {
            return string.Empty;
        }

        public override string GetContinueUrl(OrderReadOnly order, InvoicingSettings settings)
        {
            settings.MustNotBeNull("settings");
            settings.ContinueUrl.MustNotBeNull("settings.ContinueUrl");

            return settings.ContinueUrl;
        }

        public override CallbackResponse ProcessCallback(OrderReadOnly order, HttpRequestBase request, InvoicingSettings settings)
        {
            return new CallbackResponse
            {
                TransactionInfo = new TransactionInfo
                {
                    AmountAuthorized = order.TotalPrice.Value.WithTax,
                    TransactionFee = 0m,
                    TransactionId = Guid.NewGuid().ToString("N"),
                    PaymentStatus = PaymentStatus.Authorized
                }
            };
        }

        public override ApiResponse CancelPayment(OrderReadOnly order, InvoicingSettings settings)
        {
            return new ApiResponse(order.TransactionInfo.TransactionId, PaymentStatus.Cancelled);
        }

        public override ApiResponse CapturePayment(OrderReadOnly order, InvoicingSettings settings)
        {
            return new ApiResponse(order.TransactionInfo.TransactionId, PaymentStatus.Captured);
        }
    }
}
