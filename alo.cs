
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

    // =========================================
    // NEW FEATURE 1: REMOVE BOOKING
    // =========================================
    public void RemoveBooking(int id)
    {
        Booking booking = FindBooking(id);

        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        bookings.Remove(booking);
        Console.WriteLine($"Booking {id} removed successfully.");
    }

    // =========================================
    // NEW FEATURE 2: UPDATE BOOKING DATE
    // =========================================
    public void UpdateBookingDate(int id, DateTime newDate)
    {
        Booking booking = FindBooking(id);

        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        booking.BookingDate = newDate;

        Console.WriteLine(
            $"Booking {id} date updated to {newDate:dd/MM/yyyy HH:mm}."
        );
    }

    // =========================================
    // NEW FEATURE 3: SEARCH CUSTOMER
    // =========================================
    public void SearchByCustomerName(string customerName)
    {
        bool found = false;

        Console.WriteLine(
            $"\n=== SEARCH RESULT FOR: {customerName} ==="
        );

        foreach (Booking booking in bookings)
        {
            if (booking.CustomerName.Contains(
                    customerName,
                    StringComparison.OrdinalIgnoreCase))
            {
                booking.Display();
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No booking found for this customer.");
        }
    }

    // =========================================
    // NEW FEATURE 4: COUNT BOOKINGS
    // =========================================
    public int GetTotalBookings()
    {
        return bookings.Count;
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

        // Test search function
        Console.WriteLine("\n=== SEARCH CUSTOMER ===");
        manager.SearchByCustomerName("Nguyen");

        // Test update booking date
        Console.WriteLine("\n=== UPDATE BOOKING DATE ===");
        manager.UpdateBookingDate(
            3,
            DateTime.Now.AddDays(5)
        );

        Console.WriteLine("\n=== UPDATED BOOKINGS ===");
        manager.ShowAllBookings();

        // Show total bookings
        Console.WriteLine("\n=== BOOKING STATISTICS ===");
        Console.WriteLine(
            $"Total bookings: {manager.GetTotalBookings()}"
        );

        // Test remove booking
        Console.WriteLine("\n=== REMOVE BOOKING ===");
        manager.RemoveBooking(2);

        Console.WriteLine("\n=== FINAL BOOKINGS ===");
        manager.ShowAllBookings();

        Console.WriteLine(
            $"Total bookings after removal: {manager.GetTotalBookings()}"
        );

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
