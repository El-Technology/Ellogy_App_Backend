namespace PaymentManager.DAL.Enums;

public enum SubscriptionStatusEnum
{
    WithoutChanges = 0,
    PendingUpgrade = 1,
    PendingDowngrade = 2,
    PendingCancellation = 3,
    PaymentFailed = 4
}