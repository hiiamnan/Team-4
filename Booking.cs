#nullable enable
using System;
using System.Collections.Generic;

class Booking
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string Service { get; set; }
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
    private readonly List<Booking> bookings = new List<Booking>();

    public void AddBooking(Booking booking)
    {
        if (FindBooking(booking.Id) != null)
        {
            Console.WriteLine("Booking ID already exists.");
            return;
        }

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

    public Booking? FindBooking(int id)
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
        Booking? booking = FindBooking(id);
        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        booking.Status = "Cancelled";
        Console.WriteLine("Booking cancelled.");
    }

    // Both branches can edit this method to practice Git merge conflicts.
    public void ConfirmBooking(int id)
    {
        Booking? booking = FindBooking(id);
        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        if (booking.Status == "Cancelled")
        {
            Console.WriteLine("Cannot confirm a cancelled booking.");
            return;
        }

        if (booking.BookingDate <= DateTime.Now)
        {
            Console.WriteLine("Cannot confirm an expired booking.");
            return;
        }

        booking.Status = "Confirmed";
        Console.WriteLine($"[VUONG EDIT] Don hang #{id} da duoc xac nhan thanh cong luc {DateTime.Now:HH:mm}!");
    }

    public void UpdateService(int id, string newService)
    {
        Booking? booking = FindBooking(id);
        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        if (string.IsNullOrWhiteSpace(newService))
        {
            Console.WriteLine("Service name cannot be empty.");
            return;
        }

        booking.Service = newService.Trim();
        Console.WriteLine("Service updated successfully.");
    }

    public void RescheduleBooking(int id, DateTime newDate)
    {
        Booking? booking = FindBooking(id);
        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        if (newDate <= DateTime.Now)
        {
            Console.WriteLine("Booking date must be in the future.");
            return;
        }

        booking.BookingDate = newDate;
        Console.WriteLine("Booking rescheduled successfully.");
    }
}

class Program
{
    static void Main()
    {
        BookingManager manager = new BookingManager();

        manager.AddBooking(new Booking(1, "Nguyen Van A",
            "Fitness Training", DateTime.Now.AddDays(1), "Pending"));
        manager.AddBooking(new Booking(2, "Tran Thi B",
            "Nutrition Consultation", DateTime.Now.AddDays(2), "Pending"));
        manager.AddBooking(new Booking(3, "Le Van C",
            "Personal Training", DateTime.Now.AddDays(3), "Pending"));

        Console.WriteLine("\n=== ALL BOOKINGS ===");
        manager.ShowAllBookings();

        Console.WriteLine("\n=== CONFIRM BOOKING ===");
        manager.ConfirmBooking(1);

        Console.WriteLine("\n=== CANCEL BOOKING ===");
        manager.CancelBooking(2);

        Console.WriteLine("\n=== UPDATE SERVICE ===");
        manager.UpdateService(1, "Advanced Fitness Training");

        Console.WriteLine("\n=== RESCHEDULE BOOKING ===");
        manager.RescheduleBooking(3, DateTime.Now.AddDays(5));

        Console.WriteLine("\n=== FIND BOOKING ===");
        Booking? foundBooking = manager.FindBooking(3);
        if (foundBooking != null)
        {
            foundBooking.Display();
        }

        Console.WriteLine("\n=== TRY TO CONFIRM CANCELLED BOOKING ===");
        manager.ConfirmBooking(2);

        Console.WriteLine("\n=== UPDATED BOOKINGS ===");
        manager.ShowAllBookings();
    }
}
