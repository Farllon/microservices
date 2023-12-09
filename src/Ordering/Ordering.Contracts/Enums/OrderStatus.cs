namespace Ordering.Contracts.Enums;

public enum OrderStatus
{
    WaitingPaymentRegister,
    Registered,
    Started,
    WaitingPayment, 
    Shipped,
    Completed,
}