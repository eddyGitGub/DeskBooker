namespace DeskBooker.Core.Domain;
public class DeskBookingRequest: DeskBookingBase
{
}

public class DeskBooking : DeskBookingBase
{
    public int DeskId { get; set; }
}
