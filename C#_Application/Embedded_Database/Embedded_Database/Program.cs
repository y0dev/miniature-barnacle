using System;
using System.IO;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
struct Record
{
	public int Id;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
	public string Name;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
	public string Email;
}

namespace Embedded_Database
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create an array of records
			Record[] database = new Record[]
			{
			new Record { Id = 1, Name = "John", Email = "john@example.com" },
			new Record { Id = 2, Name = "Jane", Email = "jane@example.com" },
			new Record { Id = 3, Name = "Bob", Email = "bob@example.com" }
			};

			// Write the database to a binary file
			string filename = "database.bin";
			//WriteDatabaseToFile(database, filename);
			//Console.WriteLine("Database written to file: " + filename);

			// Clear the database
			database = null;

			// Read the database from the binary file
			database = ReadDatabaseFromFile(filename);
			Console.WriteLine("Database loaded from file:");

			// Print the records
			foreach (Record record in database)
			{
				Console.WriteLine("ID: " + record.Id);
				Console.WriteLine("Name: " + record.Name);
				Console.WriteLine("Email: " + record.Email);
				Console.WriteLine();
			}
		}

		static void WriteDatabaseToFile(Record[] database, string filename)
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Create))
			using (BinaryWriter writer = new BinaryWriter(fileStream))
			{
				foreach (Record record in database)
				{
					byte[] idBytes = BitConverter.GetBytes(record.Id);
					byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(record.Name.PadRight(20, '\0'));
					byte[] emailBytes = System.Text.Encoding.ASCII.GetBytes(record.Email.PadRight(50, '\0'));

					writer.Write(idBytes);
					writer.Write(nameBytes);
					writer.Write(emailBytes);
				}
			}
		}

		static Record[] ReadDatabaseFromFile(string filename)
		{
			Record[] database;
			using (FileStream fileStream = new FileStream(filename, FileMode.Open))
			using (BinaryReader reader = new BinaryReader(fileStream))
			{
				int recordSize = Marshal.SizeOf<Record>();
				int numRecords = (int)(fileStream.Length / recordSize);

				database = new Record[numRecords];

				for (int i = 0; i < numRecords; i++)
				{
					byte[] recordBytes = reader.ReadBytes(recordSize);
					GCHandle handle = GCHandle.Alloc(recordBytes, GCHandleType.Pinned);
					database[i] = Marshal.PtrToStructure<Record>(handle.AddrOfPinnedObject());
					handle.Free();
				}
			}
			return database;
		}
	}
}
