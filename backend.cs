using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

enum BookingStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}

class Booking
{
    public int Id { get; set; } 
    public string CustomerName { get; set; } 
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Service { get; set; } = "";
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [JsonIgnore]
    public bool IsActive => Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;

    public Booking() { } // needed for JSON deserialization

    // Updated by Chuong
    public string ServiceName { get; set; }

    public DateTime BookingDate { get; set; } 
    public string Status { get; set; } 
    public Booking(int id, string customerName, string service,
                   DateTime bookingDate, BookingStatus status)
    {
        Id = id;
        CustomerName = customerName;
        Service = service;
        BookingDate = bookingDate;
        Status = status;
    }

    public void Display()
    {
        Console.WriteLine($"ID:       {Id}");
        Console.WriteLine($"Customer: {CustomerName}");
        Console.WriteLine($"Service:  {Service}");
        Console.WriteLine($"Date:     {BookingDate:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Status:   {Status}");
        Console.WriteLine("----------------------------");
    }
}

class BookingManager
{
    private readonly List<Booking> bookings = new();
    private int nextId = 1;

    private const int SlotMinutes = 60;
    private const string DataFile = "bookings.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public bool HasData => bookings.Count > 0;

    // ---------- Create ----------

    public Booking? AddBooking(string customerName, string service, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            Console.WriteLine("Customer name is required.");
            return null;
        }
        if (string.IsNullOrWhiteSpace(service))
        {
            Console.WriteLine("Service is required.");
            return null;
        }
        if (date <= DateTime.Now)
        {
            Console.WriteLine("Booking date must be in the future.");
            return null;
        }
        if (HasConflict(service, date, null))
        {
            Console.WriteLine($"That time slot is already taken for {service}.");
            return null;
        }

        var booking = new Booking(nextId++, customerName.Trim(), service.Trim(),
                                  date, BookingStatus.Pending);
        bookings.Add(booking);
        Console.WriteLine($"Booking #{booking.Id} added successfully.");
        return booking;
    }

    // Same service, within one slot of another active booking
    private bool HasConflict(string service, DateTime date, int? excludeId) =>
        bookings.Any(b => b.Id != excludeId
                       && b.IsActive
                       && b.Service.Equals(service, StringComparison.OrdinalIgnoreCase)
                       && Math.Abs((b.BookingDate - date).TotalMinutes) < SlotMinutes);

    // ---------- Read ----------

    public void ShowAllBookings()
    {
        ShowList(bookings.OrderBy(b => b.BookingDate));
    }

    public Booking? FindBooking(int id) => bookings.FirstOrDefault(b => b.Id == id);

    public void SearchByCustomer(string name)
    {
        var results = bookings.Where(b =>
            b.CustomerName.Contains(name, StringComparison.OrdinalIgnoreCase));
        ShowList(results.OrderBy(b => b.BookingDate));
    }

    public void FilterByStatus(BookingStatus status)
    {
        ShowList(bookings.Where(b => b.Status == status).OrderBy(b => b.BookingDate));
    }

    public void ShowUpcoming(int days)
    {
        var now = DateTime.Now;
        var limit = now.AddDays(days);
        ShowList(bookings
            .Where(b => b.IsActive && b.BookingDate >= now && b.BookingDate <= limit)
            .OrderBy(b => b.BookingDate));
    }

    private static void ShowList(IEnumerable<Booking> list)
    {
        var items = list.ToList();
        if (items.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }
        foreach (var b in items) b.Display();
    }

    // ---------- Update ----------

    public void ConfirmBooking(int id)
    {
        var booking = FindBooking(id);
        if (booking == null) { Console.WriteLine("Booking not found."); return; }

        if (booking.Status != BookingStatus.Pending)
        {
            Console.WriteLine($"Only pending bookings can be confirmed (current: {booking.Status}).");
            return;
        }
        booking.Status = BookingStatus.Confirmed;
        Console.WriteLine("Booking confirmed.");
    }

    public void CancelBooking(int id)
    {
        var booking = FindBooking(id);
        if (booking == null) { Console.WriteLine("Booking not found."); return; }

        if (!booking.IsActive)
        {
            Console.WriteLine($"Cannot cancel a booking that is {booking.Status}.");
            return;
        }
        booking.Status = BookingStatus.Cancelled;
        Console.WriteLine("Booking cancelled.");
    }

    public void CompleteBooking(int id)
    {
        var booking = FindBooking(id);
        if (booking == null) { Console.WriteLine("Booking not found."); return; }

        if (booking.Status != BookingStatus.Confirmed)
        {
            Console.WriteLine("Only confirmed bookings can be marked as completed.");
            return;
        }
        booking.Status = BookingStatus.Completed;
        Console.WriteLine("Booking marked as completed.");
    }

    public void RescheduleBooking(int id, DateTime newDate)
    {
        var booking = FindBooking(id);
        if (booking == null) { Console.WriteLine("Booking not found."); return; }

        if (!booking.IsActive)
        {
            Console.WriteLine($"Cannot reschedule a {booking.Status} booking.");
            return;
        }
        if (newDate <= DateTime.Now)
        {
            Console.WriteLine("New date must be in the future.");
            return;
        }
        if (HasConflict(booking.Service, newDate, booking.Id))
        {
            Console.WriteLine("That time slot is already taken.");
            return;
        }
        booking.BookingDate = newDate;
        Console.WriteLine("Booking rescheduled.");
    }

    // ---------- Delete ----------

    public void DeleteBooking(int id)
    {
        var booking = FindBooking(id);
        if (booking == null) { Console.WriteLine("Booking not found."); return; }

        bookings.Remove(booking);
        Console.WriteLine("Booking deleted.");
    }

    // ---------- Statistics ----------

    public void ShowStatistics()
    {
        Console.WriteLine($"Total bookings: {bookings.Count}");

        foreach (BookingStatus status in Enum.GetValues(typeof(BookingStatus)))
        {
            Console.WriteLine($"  {status,-10}: {bookings.Count(b => b.Status == status)}");
        }

        var popular = bookings
            .GroupBy(b => b.Service)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        if (popular != null)
            Console.WriteLine($"Most popular service: {popular.Key} ({popular.Count()} bookings)");

        var next = bookings
            .Where(b => b.IsActive && b.BookingDate > DateTime.Now)
            .OrderBy(b => b.BookingDate)
            .FirstOrDefault();
        if (next != null)
            Console.WriteLine($"Next booking: #{next.Id} {next.CustomerName} on {next.BookingDate:dd/MM/yyyy HH:mm}");
    }

    // ---------- Persistence ----------

    public void Save()
    {
        File.WriteAllText(DataFile, JsonSerializer.Serialize(bookings, JsonOptions));
        Console.WriteLine($"Saved {bookings.Count} booking(s) to {DataFile}.");
    }

    public void Load()
    {
        if (!File.Exists(DataFile)) return;

        try
        {
            var loaded = JsonSerializer.Deserialize<List<Booking>>(
                File.ReadAllText(DataFile), JsonOptions);
            if (loaded == null) return;

            bookings.Clear();
            bookings.AddRange(loaded);
            nextId = bookings.Count == 0 ? 1 : bookings.Max(b => b.Id) + 1;
            Console.WriteLine($"Loaded {bookings.Count} booking(s) from {DataFile}.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Could not read saved bookings (file is corrupted). Starting fresh.");
        }
    }
}

class Program
{
    static readonly string[] Services =
    {
        "Fitness Training",
        "Nutrition Consultation",
        "Personal Training",
        "Yoga Class"
    };

    static void Main()
    {
        var manager = new BookingManager();
        manager.Load();

        if (!manager.HasData)
            SeedSampleData(manager);

        bool running = true;
        while (running)
        {
            PrintMenu();
            string choice = Prompt("Choose an option");
            Console.WriteLine();

            switch (choice)
            {
                case "1": AddBookingFlow(manager); break;
                case "2": manager.ShowAllBookings(); break;
                case "3": FindFlow(manager); break;
                case "4": manager.SearchByCustomer(Prompt("Customer name")); break;
                case "5": FilterFlow(manager); break;
                case "6": manager.ShowUpcoming(7); break;
                case "7": manager.ConfirmBooking(ReadInt("Booking ID")); break;
                case "8": manager.CancelBooking(ReadInt("Booking ID")); break;
                case "9": manager.CompleteBooking(ReadInt("Booking ID")); break;
                case "10": RescheduleFlow(manager); break;
                case "11": DeleteFlow(manager); break;
                case "12": manager.ShowStatistics(); break;
                case "0":
                    manager.Save();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    // ---------- Menu & flows ----------

    static void PrintMenu()
    {
        Console.WriteLine("\n===== BOOKING SYSTEM =====");
        Console.WriteLine(" 1. Add booking");
        Console.WriteLine(" 2. View all bookings");
        Console.WriteLine(" 3. Find booking by ID");
        Console.WriteLine(" 4. Search by customer name");
        Console.WriteLine(" 5. Filter by status");
        Console.WriteLine(" 6. Upcoming bookings (next 7 days)");
        Console.WriteLine(" 7. Confirm booking");
        Console.WriteLine(" 8. Cancel booking");
        Console.WriteLine(" 9. Mark booking as completed");
        Console.WriteLine("10. Reschedule booking");
        Console.WriteLine("11. Delete booking");
        Console.WriteLine("12. Statistics");
        Console.WriteLine(" 0. Save & exit");
    }

    static void AddBookingFlow(BookingManager manager)
    {
        string name = Prompt("Customer name");

        Console.WriteLine("Available services:");
        for (int i = 0; i < Services.Length; i++)
            Console.WriteLine($"  {i + 1}. {Services[i]}");

        int index = ReadInt("Service number");
        if (index < 1 || index > Services.Length)
        {
            Console.WriteLine("Invalid service.");
            return;
        }

        DateTime date = ReadDate("Date and time (dd/MM/yyyy HH:mm)");
        manager.AddBooking(name, Services[index - 1], date);
    }

    static void FindFlow(BookingManager manager)
    {
        var booking = manager.FindBooking(ReadInt("Booking ID"));
        if (booking == null) Console.WriteLine("Booking not found.");
        else booking.Display();
    }

    static void FilterFlow(BookingManager manager)
    {
        Console.WriteLine("Statuses: " + string.Join(", ", Enum.GetNames(typeof(BookingStatus))));
        string input = Prompt("Status");

        if (Enum.TryParse(input, ignoreCase: true, out BookingStatus status)
            && Enum.IsDefined(typeof(BookingStatus), status))
            manager.FilterByStatus(status);
        else
            Console.WriteLine("Unknown status.");
    }

    static void RescheduleFlow(BookingManager manager)
    {
        int id = ReadInt("Booking ID");
        DateTime newDate = ReadDate("New date and time (dd/MM/yyyy HH:mm)");
        manager.RescheduleBooking(id, newDate);
    }

    static void DeleteFlow(BookingManager manager)
    {
        int id = ReadInt("Booking ID");
        string confirm = Prompt("Are you sure you want to delete this booking? (y/n)");
        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
            manager.DeleteBooking(id);
        else
            Console.WriteLine("Delete cancelled.");
    }

    // ---------- Input helpers ----------

    static string Prompt(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    static int ReadInt(string label)
    {
        while (true)
        {
            if (int.TryParse(Prompt(label), out int value))
                return value;
            Console.WriteLine("Please enter a valid number.");
        }
    }

    static DateTime ReadDate(string label)
    {
        while (true)
        {
            if (DateTime.TryParseExact(Prompt(label), "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return date;
            Console.WriteLine("Invalid format. Example: 25/12/2026 14:30");
        }
    }

    static void SeedSampleData(BookingManager manager)
    {
        manager.AddBooking("Nguyen Van A", "Fitness Training",       DateTime.Today.AddDays(1).AddHours(9));
        manager.AddBooking("Tran Thi B",   "Nutrition Consultation", DateTime.Today.AddDays(2).AddHours(10));
        manager.AddBooking("Le Van C",     "Personal Training",      DateTime.Today.AddDays(3).AddHours(15));
    }
}
