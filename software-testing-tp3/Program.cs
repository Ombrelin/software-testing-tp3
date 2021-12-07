using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace software_testing_tp3
{
    public class Program
    {
        private static readonly int BATCH_SIZE = 100000;
        private static readonly int[] DATABASE_SIZES =         
        {
            10,
            100,
            10000,
            50000,
            100000,
            200000, 
            300000,
            500000, 
            600000, 
            700000, 
            800000, 
            900000, 
            1000000
        };
        public static void Main()
        {
            List<string> csvLines = DATABASE_SIZES
                .Select(InsertContacts)
                .Select(size => (size: size, time: TimeQueryLast(size)))
                .Aggregate(new List<string> {"Records Count;Query Time (ms)"}, (acc, sizeAndTime) =>
                {
                    acc.Add($"{sizeAndTime.size};{sizeAndTime.time}");
                    return acc;
                });

            File.WriteAllLines(@"C:\Users\arsen\git\software-testing-tp3\software-testing-tp3\result-with-index.csv", csvLines);
        }

        public static long TimeQueryLast(int numberOfLinesInDb)
        {
            using var context = new ContactDbContext(numberOfLinesInDb);
            Console.WriteLine($"Mesuring querying the last of {context.Contacts.Count()} records");

            var stopwatch = new Stopwatch();
            stopwatch.Start();


            Contact last = context.Contacts
                .First(contact => contact.Email == $"email-{numberOfLinesInDb - 1}");

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private static int InsertContacts(int numberOfLinesToInsert)
        {
            using var context = new ContactDbContext(numberOfLinesToInsert);
            context.Database.Migrate();

            int insertedLines = 0;

            foreach (var chunk in GenerateContacts(numberOfLinesToInsert).Chunk(BATCH_SIZE))
            {
                context.Contacts.AddRange(chunk);
                context.SaveChanges();

                insertedLines += Math.Min(BATCH_SIZE, numberOfLinesToInsert);
                Console.WriteLine($"Inserted {insertedLines} / {numberOfLinesToInsert} lines");
            }

            return numberOfLinesToInsert;
        }

        public static IEnumerable<Contact> GenerateContacts(int numberOfRecordsToGenerate)
        {
            using var context = new ContactDbContext(numberOfRecordsToGenerate);
            for (int i = 0; i < numberOfRecordsToGenerate; ++i)
            {
                var contact = new Contact()
                {
                    Id = Guid.NewGuid(),
                    Email = $"email-{i}",
                    Name = $"name-{i}"
                };

                yield return contact;
            }
        }
    }
}