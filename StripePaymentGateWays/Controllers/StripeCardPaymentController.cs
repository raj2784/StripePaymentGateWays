using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripePaymentGateWays.Models;
using System.Linq.Expressions;
using System.Diagnostics;


//using SessionService = Stripe.Checkout.SessionService;

namespace StripePaymentGateWays.Controllers
{
    public class StripeCardPaymentController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        public StripeCardPaymentController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateCheckoutSession(decimal amount)
        {
            var currency = "aud";
            var successUrl = "http://localhost:5256/StripeCardPayment/Success";
            var cancelUrl = "http://localhost:5256/StripeCardPayment/Cancel";
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
                        

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            //UnitAmount= Convert.ToInt32(amount)*100  // amount in cents
                            UnitAmountDecimal = (amount)*100,// amount in cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Product Name",
                                Description = "Product Description"
                            }

                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,


            };

            var service = new SessionService();
            var session = service.Create(options);

            if (session != null)
            {
                return Redirect(session.Url);
            }
            return Redirect(cancelUrl);
            
        }

        public IActionResult Success()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Cancel()
        {
            return RedirectToAction("Index");
        }

        //public async Task RefundPaymentAsync(string chargeId, decimal amount)
        //{
        //    var refundOptions = new RefundCreateOptions
        //    {
        //        Amount = (long)(amount * 100) // Amount in cents
        //    };

        //    var refundService = new RefundService(_stripeSettings);
        //    return await refundService.CreateAsync(chargeId, refundOptions);
        //}

    }
}
