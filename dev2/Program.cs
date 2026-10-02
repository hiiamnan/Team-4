using System;
using System.Collections.Generic;
class Booking
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string Service { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; }
    public Booking(int id, string customerName, string service, DateTime bookingDate, status string)
    {
        Id = id; CustomerName = customerName; Service = service; BookingDate = bookingDate; Status = status;
    }
    public void Display()
    {
        Console.WriteLine($"[DEV 2 VIP] #{Id} | Client: {CustomerName.ToUpper()} | Status: [{Status}]");
    }
}
class BookingManager
{
    private List<Booking> bookings = new List<Booking>();
    public void AddBooking(Booking booking)
    {
        bookings.Add(booking);
        Console.WriteLine("[DEV 2 LOG] Booking inserted into system.");
    }
    public void ShowAllBookings()
    {
        if (bookings.Count == 0) return;
        foreach (var b in bookings) b.Display();
    }
}
class Program
{
    static void Main()
    {
        BookingManager manager = new BookingManager();
        manager.AddBooking(new Booking(1, "Nguyen Van A", "Fitness Training", DateTime.Now, "PENDING_APPROVAL"));
        Console.WriteLine("=== DEV 2 WORKSPACE ===");
        manager.ShowAllBookings();
    }
}