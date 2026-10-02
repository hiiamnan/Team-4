using System;
using System.Collections.Generic;

class Booking
{
    public int Id { get; set; } 
    public string CustomerName { get; set; } 

    // Updated by Chuong
    public string ServiceName { get; set; }

    public DateTime BookingDate { get; set; } 
    public string Status { get; set; } 
    public Booking(int id, string customerName, string service,
                   DateTime bookingDate, string status)
    {
        Id = id;
        CustomerName = customerName;
        Service = service;
        BookingDate = bookingDate;
        Status = status;
    }

    public void Display()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Customer: {CustomerName}");
        Console.WriteLine($"Service: {Service}");
        Console.WriteLine($"Date: {BookingDate:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Status: {Status}");
        Console.WriteLine("----------------------------");
    }
}

class BookingManager
{
    private List<Booking> bookings = new List<Booking>();

    public void AddBooking(Booking booking)
    {
        bookings.Add(booking);
        Console.WriteLine("Booking added successfully.");
    }

    public void ShowAllBookings()
    {
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        foreach (Booking booking in bookings)
        {
            booking.Display();
        }
    }

    public Booking FindBooking(int id)
    {
        foreach (Booking booking in bookings)
        {
            if (booking.Id == id)
            {
                return booking;
            }
        }

        return null;
    }

    public void CancelBooking(int id)
    {
        Booking booking = FindBooking(id);

        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        booking.Status = "Cancelled";
        Console.WriteLine("Booking cancelled.");
    }

    public void ConfirmBooking(int id)
    {
        Booking booking = FindBooking(id);

        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        booking.Status = "Confirmed";
        Console.WriteLine("Booking confirmed.");
    }
}

class Program
{
    static void Main()
    {
        BookingManager manager = new BookingManager();

        manager.AddBooking(
            new Booking(
                1,
                "Nguyen Van A",
                "Fitness Training",
                DateTime.Now.AddDays(1),
                "Pending"
            )
        );

        manager.AddBooking(
            new Booking(
                2,
                "Tran Thi B",
                "Nutrition Consultation",
                DateTime.Now.AddDays(2),
                "Pending"
            )
        );

        manager.AddBooking(
            new Booking(
                3,
                "Le Van C",
                "Personal Training",
                DateTime.Now.AddDays(3),
                "Pending"
            )
        );

        Console.WriteLine("\n=== ALL BOOKINGS ===");
        manager.ShowAllBookings();

        Console.WriteLine("\n=== CONFIRM BOOKING ===");
        manager.ConfirmBooking(1);

        Console.WriteLine("\n=== CANCEL BOOKING ===");
        manager.CancelBooking(2);

        Console.WriteLine("\n=== UPDATED BOOKINGS ===");
        manager.ShowAllBookings();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}